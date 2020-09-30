using BowieD.Unturned.AssetExpander.Models;
using SDG.Unturned;

namespace BowieD.Unturned.AssetExpander.CustomFields.Items
{
    public class ProofAcidCustomField : ICustomField
    {
        public string Name => "Proof_Acid";
        public string[] AdditionalFields => new string[0];
        public EAssetType Type => EAssetType.ITEM;

        public bool ShouldInit => true;

        public void Init()
        {
            DamageTool.damagePlayerRequested += DamageTool_damagePlayerRequested;
        }

        private void DamageTool_damagePlayerRequested(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            if (shouldAllow)
            {
                if (parameters.player != null)
                {
                    switch (parameters.cause)
                    {
                        case EDeathCause.ACID:
                            {
                                ItemAsset top, pants;

                                var clothing = parameters.player.clothing;

                                if (clothing.shirt > 0)
                                    top = clothing.shirtAsset;
                                else
                                    top = null;

                                if (clothing.pants > 0)
                                    pants = clothing.pantsAsset;
                                else
                                    pants = null;

                                if (top != null && pants != null)
                                {
                                    if (Plugin.CustomData.TryGetValue(top.GUID, out var cDataTop) && Plugin.CustomData.TryGetValue(pants.GUID, out var cDataPants))
                                    {
                                        if (cDataTop.ContainsKey(Name) && cDataPants.ContainsKey(Name))
                                        {
                                            parameters.times *= 0f;
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }

        public void Stop()
        {
            DamageTool.damagePlayerRequested -= DamageTool_damagePlayerRequested;
        }
    }
}
