using BowieD.Unturned.AssetExpander.Models;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using UnityEngine;

namespace BowieD.Unturned.AssetExpander.CustomFields.Items
{
    public sealed class DisableBackpackCustomField : ICustomField, IFixedUpdateable
    {
        public string Name => "Disable_Backpack";

        private const string bypassField = "Disable_Backpack_Bypass";

        public string[] AdditionalFields => new string[1] { bypassField };
        public EAssetType Type => EAssetType.ITEM;
        public bool ShouldInit => true;
        public void Init()
        {
            lastFixedUpdate = Time.realtimeSinceStartup;
        }
        public void Stop()
        {

        }
        float lastFixedUpdate;
        public void FixedUpdate()
        {
            if (Time.realtimeSinceStartup - lastFixedUpdate >= 1f)
            {
                lastFixedUpdate = Time.realtimeSinceStartup;

                foreach (var sp in Provider.clients)
                {
                    if (sp == null || sp.player == null)
                        continue;

                    var clothing = sp.player.clothing;

                    bool bypass(ItemBackpackAsset asset)
                    {
                        if (asset == null)
                            return false;

                        if (Plugin.HasCustomData(asset.GUID, bypassField))
                        {
                            return true;
                        }
                        return false;
                    }

                    bool doAsset(ItemClothingAsset asset)
                    {
                        if (asset == null)
                            return false;

                        if (Plugin.HasCustomData(asset.GUID, Name))
                        {
                            takeOffBackpack(clothing);
                            return true;
                        }

                        return false;
                    }

                    if (clothing.backpack > 0 && bypass(clothing.backpackAsset))
                    {
                        break;
                    }

                    if (clothing.hat > 0 && doAsset(clothing.hatAsset))
                    {
                        break;
                    }
                    if (clothing.mask > 0 && doAsset(clothing.maskAsset))
                    {
                        break;
                    }
                    if (clothing.glasses > 0 && doAsset(clothing.glassesAsset))
                    {
                        break;
                    }
                    if (clothing.vest > 0 && doAsset(clothing.vestAsset))
                    {
                        break;
                    }
                    if (clothing.shirt > 0 && doAsset(clothing.shirtAsset))
                    {
                        break;
                    }
                    if (clothing.pants > 0 && doAsset(clothing.pantsAsset))
                    {
                        break;
                    }
                }
            }
        }
        
        void takeOffBackpack(PlayerClothing clothing)
        {
            if (clothing.backpack > 0)
            {
                clothing.askWearBackpack(0, 0, new byte[0], true);

                UnturnedChat.Say(clothing.player.channel.owner.playerID.steamID, Plugin.Instance.Translate("DisableBackpackCustomField_takeOffBackpack"), true);
            }
        }
    }
}
