using SDG.Unturned;

namespace BowieD.Unturned.AssetExpander
{
    public static class EventManager
    {
        public static void Start()
        {
            UseableConsumeable.onConsumePerformed += UseableConsumeable_onConsumePerformed;
        }

        private static void UseableConsumeable_onConsumePerformed(Player instigatingPlayer, ItemConsumeableAsset consumeableAsset)
        {
            if (Plugin.CustomData.TryGetValue(consumeableAsset.GUID, out var cData))
            {
                if (cData.TryGetValue("Thirst", out var thirstRaw) && byte.TryParse(thirstRaw, out byte thirst))
                {
                    instigatingPlayer.life.serverModifyWater(-thirst);
                }
                if (cData.TryGetValue("Starve", out var starveRaw) && byte.TryParse(starveRaw, out byte starve))
                {
                    instigatingPlayer.life.serverModifyFood(-starve);
                }
                if (cData.TryGetValue("Tire", out var tireRaw) && byte.TryParse(tireRaw, out byte tire))
                {
                    instigatingPlayer.life.serverModifyStamina(-tire);
                }
            }
        }

        public static void Stop()
        {
            UseableConsumeable.onConsumePerformed -= UseableConsumeable_onConsumePerformed;
        }
    }
}
