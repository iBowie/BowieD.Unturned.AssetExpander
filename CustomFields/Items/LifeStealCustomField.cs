using BowieD.Unturned.AssetExpander.Models;
using SDG.Unturned;
using Steamworks;
using System.Globalization;

namespace BowieD.Unturned.AssetExpander.CustomFields.Items
{
    public class LifeStealCustomField : ICustomField
    {
        private const string Field_Lifesteal_Min = "Lifesteal_Min";
        private const string Field_Lifesteal_Max = "Lifesteal_Max";

        public string Name => "Lifesteal";
        public string[] AdditionalFields => new string[] { Field_Lifesteal_Min, Field_Lifesteal_Max };
        public EAssetType Type => EAssetType.ITEM;

        public bool ShouldInit => true;

        public void Init()
        {
            DamageTool.damageAnimalRequested += DamageTool_damageAnimalRequested;
            DamageTool.damageZombieRequested += DamageTool_damageZombieRequested;
            DamageTool.damagePlayerRequested += DamageTool_damagePlayerRequested;
        }

        private void DamageTool_damagePlayerRequested(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            if (!shouldAllow)
                return;

            Player killer;

            if (parameters.killer != CSteamID.Nil)
            {
                killer = PlayerTool.getPlayer(parameters.killer);

                if (killer == null)
                    return;

                perform(killer, parameters.damage, parameters.times, killer.equipment.asset);
            }
        }

        private void DamageTool_damageZombieRequested(ref DamageZombieParameters parameters, ref bool shouldAllow)
        {
            if (!shouldAllow)
                return;

            if (parameters.instigator is Player killer && killer != null)
            {
                perform(killer, parameters.damage, parameters.times, killer.equipment.asset);
            }
        }

        private void DamageTool_damageAnimalRequested(ref DamageAnimalParameters parameters, ref bool shouldAllow)
        {
            if (!shouldAllow)
                return;

            if (parameters.instigator is Player killer && killer != null)
            {
                perform(killer, parameters.damage, parameters.times, killer.equipment.asset);
            }
        }

        void perform(Player player, float damage, float times, ItemAsset asset)
        {
            if (player is null || asset is null)
                return;

            if (asset is ItemMeleeAsset || asset is ItemGunAsset)
            {
                if (Plugin.TryGetCustomDataFor<float>(asset.GUID, Name, out var parsed))
                {
                    float ret = (damage * times) * parsed;

                    #region clamp
                    float min;
                    float max;

                    if (!Plugin.TryGetCustomDataFor<float>(asset.GUID, Field_Lifesteal_Min, out min))
                    {
                        min = 0f;
                    }
                    if (!Plugin.TryGetCustomDataFor<float>(asset.GUID, Field_Lifesteal_Max, out max))
                    {
                        max = 100f;
                    }

                    if (ret > max)
                        ret = max;
                    else if (ret < min)
                        ret = min;
                    #endregion

                    player.life.serverModifyHealth(ret);
                }
            }
        }

        public void Stop()
        {
            DamageTool.damageAnimalRequested -= DamageTool_damageAnimalRequested;
            DamageTool.damageZombieRequested -= DamageTool_damageZombieRequested;
            DamageTool.damagePlayerRequested -= DamageTool_damagePlayerRequested;
        }
    }
}
