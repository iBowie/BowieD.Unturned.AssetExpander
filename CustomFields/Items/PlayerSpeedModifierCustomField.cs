using BowieD.Unturned.AssetExpander.Models;
using Rocket.Unturned;
using SDG.Unturned;
using UnityEngine;

namespace BowieD.Unturned.AssetExpander.CustomFields.Items
{
    public class PlayerSpeedModifierCustomField : ICustomField
    {
        private const string fieldName = "Player_Speed";

        public string Name => fieldName;
        public string[] AdditionalFields => new string[0];
        public EAssetType Type => EAssetType.ITEM;
        public bool ShouldInit => true;

        public void Init()
        {
            if (Level.isLoaded)
            {
                foreach (var sp in Provider.clients)
                {
                    if (sp == null || sp.player == null)
                        continue;

                    sp.player.gameObject.getOrAddComponent<SpeedSetterComponent>();
                }
            }

            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
        }

        private void Events_OnPlayerConnected(Rocket.Unturned.Player.UnturnedPlayer player)
        {
            player.Player.gameObject.getOrAddComponent<SpeedSetterComponent>();
        }

        public void Stop()
        {
            if (Level.isLoaded)
            {
                foreach (var sp in Provider.clients)
                {
                    if (sp == null || sp.player == null)
                        continue;

                    var ssc = sp.player.GetComponent<SpeedSetterComponent>();

                    Object.Destroy(ssc);
                }
            }

            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
        }

        public class SpeedSetterComponent : MonoBehaviour
        {
            float currentMultiplier;
            Player player;

            void Awake()
            {
                currentMultiplier = 1f;

                player = GetComponent<Player>();
            }
            int frame;
            void FixedUpdate()
            {
                if (player.life.isDead)
                {
                    if (currentMultiplier != 1f)
                    {
                        stopSpeed();
                    }
                }
                else if (++frame % 25 == 0)
                {
                    if (player.equipment.asset != null)
                    {
                        if (Plugin.TryGetCustomDataFor<float>(player.equipment.asset.GUID, fieldName, out var value))
                        {
                            float trueValue = Mathf.Clamp(value, 0.01f, float.MaxValue);

                            if (!Mathf.Approximately(trueValue, currentMultiplier))
                            {
                                stopSpeed();

                                startSpeed(trueValue);
                            }
                        }
                    }
                    else if (currentMultiplier != 1f)
                    {
                        stopSpeed();
                    }
                }
            }
            void OnDestroy()
            {
                stopSpeed();
            }

            void startSpeed(float multiplier)
            {
                float old = player.movement.pluginSpeedMultiplier;

                player.movement.sendPluginSpeedMultiplier(old * multiplier);

                currentMultiplier = multiplier;
            }
            void stopSpeed()
            {
                float old = player.movement.pluginSpeedMultiplier;

                player.movement.sendPluginSpeedMultiplier(old / currentMultiplier);

                currentMultiplier = 1f;
            }
        }
    }
}
