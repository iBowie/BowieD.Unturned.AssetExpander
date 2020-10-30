using BowieD.Unturned.AssetExpander.Models;
using Harmony;
using SDG.Unturned;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BowieD.Unturned.AssetExpander.CustomFields.Vehicles
{
    public sealed class HookRadiusCustomField : ICustomField, IDependentField
    {
        private const string FieldName = "Hook_Radius";
        private HarmonyInstance _harmony;

        private string _fullname = typeof(HookRadiusCustomField).FullName;

        private static MethodInfo original_useHook, patch_useHook;
        private static FieldInfo original_hooked, original_hook, original_grab;

        static void prepareOriginalHooked()
        {
            if (original_hooked == null)
            {
                original_hooked = typeof(InteractableVehicle).GetField("hooked", BindingFlags.Instance | BindingFlags.NonPublic);
            }
        }

        private static List<HookInfo> get_priv_hooked(InteractableVehicle instance)
        {
            prepareOriginalHooked();
            return (List<HookInfo>)original_hooked.GetValue(instance);
        }
        private static void set_priv_hooked(List<HookInfo> hooks, InteractableVehicle instance)
        {
            prepareOriginalHooked();
            original_hooked.SetValue(instance, hooks);
        }
        private static Transform get_priv_hook(InteractableVehicle instance)
        {
            if (original_hook == null)
            {
                original_hook = typeof(InteractableVehicle).GetField("hook", BindingFlags.Instance | BindingFlags.NonPublic);
            }

            return (Transform)original_hook.GetValue(instance);
        }
        private static Collider[] get_priv_grab()
        {
            if (original_grab == null)
            {
                original_grab = typeof(InteractableVehicle).GetField("grab", BindingFlags.Static | BindingFlags.NonPublic);
            }

            return (Collider[])original_grab.GetValue(null);
        }

        public string[] Dependencies => new string[] { "0Harmony" };
        public string Name => FieldName;
        public string[] AdditionalFields => new string[0];
        public EAssetType Type => EAssetType.VEHICLE;
        public bool ShouldInit => true;
        public void Init()
        {
            _harmony = HarmonyInstance.Create(_fullname);

            original_useHook = typeof(InteractableVehicle).GetMethod(nameof(InteractableVehicle.useHook), BindingFlags.Public | BindingFlags.Instance);
            patch_useHook = typeof(HookRadiusCustomField).GetMethod(nameof(useHook_patch), BindingFlags.Static | BindingFlags.NonPublic);
            
            _harmony.Patch(original_useHook, new HarmonyMethod(patch_useHook));
        }
        public void Stop()
        {
            _harmony.UnpatchAll(_fullname);
        }

        static bool useHook_patch(InteractableVehicle __instance)
        {
            List<HookInfo> hooked = get_priv_hooked(__instance);

            if (hooked.Count > 0)
            {
                __instance.clearHooked();
                return false;
            }

            float radius;

            if (!Plugin.TryGetCustomDataFor<float>(__instance.asset.GUID, FieldName, out radius))
            {
                radius = 3f;
            }

            var hook = get_priv_hook(__instance);
            var grab = get_priv_grab();

            int num = Physics.OverlapSphereNonAlloc(hook.position, radius, grab, 67108864);
            for (int i = 0; i < num; i++)
            {
                InteractableVehicle vehicle = DamageTool.getVehicle(grab[i].transform);
                if (!(vehicle == null) && !(vehicle == __instance) && vehicle.isEmpty && !vehicle.isHooked && vehicle.asset.engine != EEngine.TRAIN)
                {
                    HookInfo hookInfo = new HookInfo
                    {
                        target = vehicle.transform,
                        vehicle = vehicle,
                        deltaPosition = hook.InverseTransformPoint(vehicle.transform.position),
                        deltaRotation = Quaternion.FromToRotation(hook.forward, vehicle.transform.forward)
                    };
                    hooked.Add(hookInfo);
                    vehicle.isHooked = true;
                    __instance.ignoreCollisionWith(vehicle.vehicleColliders, true);
                }
            }

            set_priv_hooked(hooked, __instance);

            return false;
        }
    }
}
