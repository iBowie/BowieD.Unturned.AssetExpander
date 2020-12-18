using BowieD.Unturned.AssetExpander.Models;
using Harmony;
using SDG.Unturned;
using System.Reflection;
using UnityEngine;

namespace BowieD.Unturned.AssetExpander.CustomFields.Vehicles
{
    public sealed class BatteryIDCustomField : ICustomField, IDependentField
    {
        private const string FieldName = "Battery_ID";
        private HarmonyInstance _harmony;

        private string _fullname = typeof(BatteryIDCustomField).FullName;

        private static MethodInfo original_giveBatteryItem, patch_giveBatteryItem;
        private static MethodInfo original_simulate, patch_simulate;

        public string[] Dependencies => new string[] { "0Harmony" };
        public string Name => FieldName;
        public string[] AdditionalFields => new string[0];
        public EAssetType Type => EAssetType.VEHICLE;
        public bool ShouldInit => true;
        public void Init()
        {
            _harmony = HarmonyInstance.Create(_fullname);

            original_giveBatteryItem = typeof(InteractableVehicle).GetMethod("giveBatteryItem", BindingFlags.NonPublic | BindingFlags.Instance);
            patch_giveBatteryItem = typeof(BatteryIDCustomField).GetMethod(nameof(giveBatteryItem_patch), BindingFlags.Static | BindingFlags.NonPublic);

            original_simulate = typeof(UseableVehicleBattery).GetMethod(nameof(UseableVehicleBattery.simulate), BindingFlags.Public | BindingFlags.Instance);
            patch_simulate = typeof(BatteryIDCustomField).GetMethod(nameof(simulate_patch), BindingFlags.Static | BindingFlags.NonPublic);

            _harmony.Patch(original_giveBatteryItem, new HarmonyMethod(patch_giveBatteryItem));
            _harmony.Patch(original_simulate, new HarmonyMethod(patch_simulate));
        }
        public void Stop()
        {
            _harmony.UnpatchAll(_fullname);
        }

        static bool simulate_patch(uint simulation, bool inputSteady, UseableVehicleBattery __instance, ref bool ___isUsing, ref bool ___isReplacing, ref float ___startedUse, ref float ___useTime, ref InteractableVehicle ___vehicle)
        {
            bool isReplaceable = Time.realtimeSinceStartup - ___startedUse > ___useTime * 0.75f;
            bool isUseable = Time.realtimeSinceStartup - ___startedUse > ___useTime;

            if (___isReplacing && isReplaceable)
            {
                ___isReplacing = false;
                if (___vehicle != null && ___vehicle.isBatteryReplaceable)
                {
                    if (Plugin.TryGetCustomDataFor<ushort>(___vehicle.asset.GUID, FieldName, out ushort requiredBatteryID))
                    {
                        if (requiredBatteryID != __instance.player.equipment.itemID)
                        {
                            return false;
                        }
                    }

                    ___vehicle.replaceBattery(__instance.player, __instance.player.equipment.quality);
                    ___vehicle = null;
                }
                if (Provider.isServer)
                {
                    __instance.player.equipment.useStepA();
                }
            }
            if (___isUsing && isUseable)
            {
                __instance.player.equipment.isBusy = false;
                ___isUsing = false;
                if (Provider.isServer)
                {
                    __instance.player.equipment.useStepB();
                }
            }

            return false;
        }
        static bool giveBatteryItem_patch(Player player, InteractableVehicle __instance, ref bool __result)
        {
            byte b = (byte)Mathf.FloorToInt(__instance.batteryCharge / 100f);
            if (b == 0)
            {
                __result = false;
                return false;
            }

            ushort itemID;
            
            if (!Plugin.TryGetCustomDataFor<ushort>(__instance.asset.GUID, FieldName, out itemID))
            {
                itemID = 1450;
            }

            if (itemID == 0)
            {
                __result = false;
                return false;
            }

            Item item = new Item(itemID, 1, b);
            player.inventory.forceAddItem(item, auto: false);
            __result = true;
            return false;
        }
    }
}
