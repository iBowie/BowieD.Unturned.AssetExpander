using Rocket.Core.Plugins;
using SDG.Unturned;
using System;
using System.Collections.Generic;

namespace BowieD.Unturned.AssetExpander
{
    public sealed class Plugin : RocketPlugin<PluginConfiguration>
    {
        internal static Plugin Instance { get; private set; }
        internal static Dictionary<Guid, Dictionary<string, string>> CustomData { get; } = new Dictionary<Guid, Dictionary<string, string>>();

        protected override void Load()
        {
            CustomData.Clear();
            LoadCustomData();
            EventManager.Start();

            Rocket.Core.Logging.Logger.Log("Plugin created by BowieD");
            Rocket.Core.Logging.Logger.Log(@"https://github.com/iBowie/BowieD.Unturned.AssetExpander");
        }
        protected override void Unload()
        {
            EventManager.Stop();
            CustomData.Clear();
        }

        void LoadCustomData()
        {
            if (Configuration.Instance.SearchMode == ESearchMode.OFF)
                return;

            if (Configuration.Instance.SearchMode.HasFlag(ESearchMode.ASSET))
            {
                Rocket.Core.Logging.Logger.Log("ASSET search is disabled");
            }

            if (Configuration.Instance.SearchMode.HasFlag(ESearchMode.CONFIG))
            {
                foreach (var kv in Configuration.Instance.CustomFields)
                {
                    Guid g = new Guid(kv.Key);
                    var asset = Assets.find(g);
                    if (asset is null)
                        continue;

                    if (!CustomData.ContainsKey(asset.GUID))
                    {
                        CustomData.Add(asset.GUID, new Dictionary<string, string>());
                    }

                    var dict = CustomData[asset.GUID];

                    foreach (var cf in kv.Value)
                    {
                        if (dict.ContainsKey(cf.Key))
                            dict[cf.Key] = cf.Value;
                        else
                            dict.Add(cf.Key, cf.Value);
                    }
                }
            }
        }
    }
}
