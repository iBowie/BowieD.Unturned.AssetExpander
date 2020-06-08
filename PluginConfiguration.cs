using BowieD.Unturned.AssetExpander.Collections;
using Rocket.API;

namespace BowieD.Unturned.AssetExpander
{
    public sealed class PluginConfiguration : IRocketPluginConfiguration, IDefaultable
    {
        public ESearchMode SearchMode { get; set; }
        public SerializableDictionary<string, SerializableDictionary<string, string>> CustomFields { get; set; }

        public void LoadDefaults()
        {
            SearchMode = ESearchMode.FULL;
            CustomFields = new SerializableDictionary<string, SerializableDictionary<string, string>>()
            {
                {
                    "2a1350f9ca41402fa0f10297b878cc3c",
                    new SerializableDictionary<string, string>()
                    {
                        {
                            "Thirst",
                            "10"
                        }
                    }
                }
            };
        }
    }
}
