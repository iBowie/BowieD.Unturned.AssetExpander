﻿using BowieD.Unturned.AssetExpander.Models;
using SDG.Unturned;

namespace BowieD.Unturned.AssetExpander.CustomFields.Items
{
    public class TireCustomField : ICustomField
    {
        public string Name => "Tire";
        public string[] AdditionalFields => new string[0];
        public EAssetType Type => EAssetType.ITEM;
        public void Init()
        {
            UseableConsumeable.onConsumePerformed += UseableConsumeable_onConsumePerformed;
            UseableConsumeable.onPerformedAid += UseableConsumeable_onPerformedAid;
        }

        private void UseableConsumeable_onPerformedAid(Player instigator, Player target)
        {
            var asset = instigator.equipment.asset;
            if (asset is ItemConsumeableAsset ica)
                consume(target, ica);
        }

        private void UseableConsumeable_onConsumePerformed(Player instigatingPlayer, ItemConsumeableAsset consumeableAsset)
        {
            consume(instigatingPlayer, consumeableAsset);
        }

        private void consume(Player player, ItemConsumeableAsset asset)
        {
            if (Plugin.CustomData.TryGetValue(asset.GUID, out var cData))
            {
                if (cData.TryGetValue(Name, out var raw) && byte.TryParse(raw, out byte parsed))
                {
                    player.life.serverModifyStamina(-parsed);
                }
            }
        }

        public void Stop()
        {
            UseableConsumeable.onConsumePerformed -= UseableConsumeable_onConsumePerformed;
            UseableConsumeable.onPerformedAid -= UseableConsumeable_onPerformedAid;
        }

        public bool ShouldInit => true;
    }
}
