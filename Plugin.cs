using BowieD.Unturned.AssetExpander.Models;
using Newtonsoft.Json.Linq;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace BowieD.Unturned.AssetExpander
{
    public sealed class Plugin : RocketPlugin<PluginConfiguration>
    {
        internal static Plugin Instance { get; private set; }
        private static Dictionary<Guid, Dictionary<string, string>> CustomData { get; } = new Dictionary<Guid, Dictionary<string, string>>();
        internal static HashSet<ICustomField> Fields { get; } = new HashSet<ICustomField>();

        protected override void Load()
        {
            Instance = this;

            CustomData.Clear();
            RegisterCustomFields(Assembly);
            LoadCustomData();

            Rocket.Core.Logging.Logger.Log("Plugin created by BowieD");
            Rocket.Core.Logging.Logger.Log(@"https://github.com/iBowie/BowieD.Unturned.AssetExpander");

            switch (checkForUpdates())
            {
                case true:
                    {
                        Rocket.Core.Logging.Logger.LogWarning("Update available. Head to GitHub page to download it.");
                    }
                    break;
                case false:
                    {

                    }
                    break;
                default:
                    {
                        Rocket.Core.Logging.Logger.LogWarning("Could not get new version.");
                    }
                    break;
            }
        }
        protected override void Unload()
        {
            foreach (var f in Fields)
                f.Stop();

            Fields.Clear();
            CustomData.Clear();

            Instance = null;
        }

        public void RegisterCustomField<T>() where T : class, ICustomField, new()
        {
            RegisterCustomField(new T());
        }
        public void RegisterCustomField<T>(T instance) where T : class, ICustomField
        {
            RegisterCustomField((ICustomField)instance);
        }
        public void RegisterCustomFields(Assembly assembly)
        {
            foreach (var t in assembly.GetTypes())
            {
                if (t.IsClass && typeof(ICustomField).IsAssignableFrom(t))
                {
                    try
                    {
                        ICustomField instance = (ICustomField)Activator.CreateInstance(t);
                        if (instance == null)
                            continue;

                        RegisterCustomField(instance);
                    }
                    catch (Exception ex)
                    {
                        Rocket.Core.Logging.Logger.LogException(ex, $"Coult not register custom field '{t.FullName}'");
                    }
                }
            }
        }

        private void RegisterCustomField(ICustomField instance)
        {
            if (instance.ShouldInit)
            {
                string name = instance.Name;

                if (Configuration.Instance.DisabledCustomFields != null && Configuration.Instance.DisabledCustomFields.Contains(name))
                {
                    Rocket.Core.Logging.Logger.Log($"Field '{name}' is disabled in the configuration.");
                }
                else
                {
                    if (instance is IDependentField df)
                    {
                        foreach (var d in df.Dependencies)
                        {
                            if (IsDependencyLoaded(d))
                                continue;

                            Rocket.Core.Logging.Logger.LogWarning($"Field {instance.Name} requires {d} to work.");
                        }
                    }

                    Fields.Add(instance);
                    instance.Init();
                }
            }
        }
        private void LoadCustomData()
        {
            if (Configuration.Instance.SearchMode == ESearchMode.OFF)
                return;

            if (Configuration.Instance.SearchMode.HasFlag(ESearchMode.ASSET))
            {
                List<Asset> assets = new List<Asset>();
                Assets.find(assets);

                foreach (var asset in assets)
                {
                    var assetPath = asset.absoluteOriginFilePath;
                    var fields = Fields.Where(d => d.Type == asset.assetCategory);
                    if (fields.Any())
                    {
                        using (StreamReader sr = new StreamReader(assetPath))
                        {
                            while (!sr.EndOfStream)
                            {
                                string line = sr.ReadLine();
                                string[] split = line.Split(' ');

                                switch (split.Length)
                                {
                                    case 1:
                                        {
                                            string name = split[0];
                                            
                                            if (fields.Any(d => d.Name == name || d.AdditionalFields.Contains(name)))
                                            {
                                                addOrOverrideCustomData(asset.GUID, name, string.Empty);
                                            }
                                        }
                                        break;
                                    case 2:
                                        {
                                            string name = split[0];
                                            string value = string.Join(" ", split.Skip(1));

                                            if (fields.Any(d => d.Name == name || d.AdditionalFields.Contains(name)))
                                            {
                                                addOrOverrideCustomData(asset.GUID, name, value);
                                            }
                                        }
                                        break;
                                }

                            }
                        }
                    }
                }
            }

            if (Configuration.Instance.SearchMode.HasFlag(ESearchMode.CONFIG))
            {
                foreach (var kv in Configuration.Instance.CustomFields)
                {
                    Guid g = new Guid(kv.Key);
                    var asset = Assets.find(g);
                    if (asset is null)
                        continue;

                    foreach (var cf in kv.Value)
                    {
                        addOrOverrideCustomData(g, cf.Key, cf.Value);
                    }
                }
            }
        }
        void addOrOverrideCustomData(Guid g, string name, string value)
        {
            if (!CustomData.ContainsKey(g))
            {
                CustomData.Add(g, new Dictionary<string, string>());
            }

            var dict = CustomData[g];

            if (dict.ContainsKey(name))
                dict[name] = value;
            else
                dict.Add(name, value);
        }

        bool? checkForUpdates()
        {
            try
            {
                var v = Assembly.GetName().Version;

                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add(HttpRequestHeader.UserAgent, "Unturned");

                    var url = $"https://api.github.com/repos/iBowie/BowieD.Unturned.AssetExpander/releases/latest";

                    string data = wc.DownloadString(url);

                    JObject jobj = JObject.Parse(data);

                    if (jobj.TryGetValue("tag_name", out var tag_nameToken))
                    {
                        var nv = Version.Parse(tag_nameToken.Value<string>());

                        return nv > v ? true : false;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Rocket.Core.Logging.Logger.LogException(ex, "Could not check for new updates");
                return null;
            }
        }

        public static bool HasCustomData(Guid guid, string key)
        {
            if (CustomData.TryGetValue(guid, out var dict))
            {
                return dict.ContainsKey(key);
            }

            return false;
        }
        public static bool TryGetCustomDataFor(Guid guid, string key, out string value)
        {
            return TryGetCustomDataFor(guid, key, out _, out value);
        }
        public static bool TryGetCustomDataFor(Guid guid, string key, out Dictionary<string, string> dict, out string value)
        {
            if (CustomData.TryGetValue(guid, out dict))
            {
                if (dict.TryGetValue(key, out value))
                {
                    return true;
                }
            }

            value = default;
            return false;
        }
        public static bool TryGetCustomDataFor<T>(Guid guid, string key, out Dictionary<string, string> dict, out T value)
        {
            if (typeof(IConvertible).IsAssignableFrom(typeof(T)))
            {
                if (TryGetCustomDataFor(guid, key, out dict, out var raw))
                {
                    try
                    {
                        value = (T)Convert.ChangeType(raw, typeof(T), CultureInfo.InvariantCulture);
                        return true;
                    }
                    catch
                    {
                        value = default;
                        return false;
                    }
                }
            }
            else
            {
                Rocket.Core.Logging.Logger.LogError("Somehow i messed up that part");
            }

            dict = default;
            value = default;
            return false;
        }
        public static bool TryGetCustomDataFor<T>(Guid guid, string key, out T value)
        {
            return TryGetCustomDataFor<T>(guid, key, out _, out value);
        }

        void FixedUpdate()
        {
            if (Instance == null || !Level.isLoaded)
                return;

            foreach (var cf in Fields)
            {
                if (cf is IFixedUpdateable fixedUpdateable)
                {
                    fixedUpdateable.FixedUpdate();
                }
            }
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            {
                "DisableBackpackCustomField_takeOffBackpack",
                "You cannot wear backpack right now."
            }
        };
    }
}
