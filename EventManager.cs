using SDG.Unturned;

namespace BowieD.Unturned.AssetExpander
{
    public static class EventManager
    {
        public static void Start()
        {
            UseableConsumeable.onConsumePerformed += UseableConsumeable_onConsumePerformed;
            UseableConsumeable.onPerformedAid += UseableConsumeable_onPerformedAid;
        }

        private static void UseableConsumeable_onPerformedAid(Player instigator, Player target)
        {
            var asset = instigator.equipment.asset;
            if (asset is ItemConsumeableAsset ica)
                consume(target, ica);
        }

        private static void UseableConsumeable_onConsumePerformed(Player instigatingPlayer, ItemConsumeableAsset consumeableAsset)
        {
            consume(instigatingPlayer, consumeableAsset);
        }

        private static void consume(Player player, ItemConsumeableAsset asset)
        {
            if (Plugin.CustomData.TryGetValue(asset.GUID, out var cData))
            {
                if (cData.TryGetValue("Thirst", out var thirstRaw) && byte.TryParse(thirstRaw, out byte thirst))
                {
                    player.life.serverModifyWater(-thirst);
                }
                if (cData.TryGetValue("Starve", out var starveRaw) && byte.TryParse(starveRaw, out byte starve))
                {
                    player.life.serverModifyFood(-starve);
                }
                if (cData.TryGetValue("Tire", out var tireRaw) && byte.TryParse(tireRaw, out byte tire))
                {
                    player.life.serverModifyStamina(-tire);
                }
            }
        }

        public static void Stop()
        {
            UseableConsumeable.onConsumePerformed -= UseableConsumeable_onConsumePerformed;
            UseableConsumeable.onPerformedAid -= UseableConsumeable_onPerformedAid;
        }
    }
}
