using BowieD.Unturned.AssetExpander.Models;
using Rocket.Core.Plugins;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BowieD.Unturned.AssetExpander
{
    public sealed class Plugin : RocketPlugin<PluginConfiguration>
    {
        internal static Plugin Instance { get; private set; }
        internal static Dictionary<Guid, Dictionary<string, string>> CustomData { get; } = new Dictionary<Guid, Dictionary<string, string>>();
        internal static HashSet<ICustomField> Fields { get; } = new HashSet<ICustomField>();

        protected override void Load()
        {
            CustomData.Clear();
            RegisterCustomFields(Assembly);
            LoadCustomData();

            Rocket.Core.Logging.Logger.Log("Plugin created by BowieD");
            Rocket.Core.Logging.Logger.Log(@"https://github.com/iBowie/BowieD.Unturned.AssetExpander");
        }
        protected override void Unload()
        {
            CustomData.Clear();
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
                    catch { }
                }
            }
        }

        private void RegisterCustomField(ICustomField instance)
        {
            if (instance.ShouldInit)
            {
                Fields.Add(instance);
                instance.Init();
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
                                string name = split[0];
                                string value = string.Join(" ", split.Skip(1));
                                
                                if (fields.Any(d => d.Name == name || d.AdditionalFields.Contains(name)))
                                {
                                    addOrOverrideCustomData(asset.GUID, name, value);
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
    }
}
