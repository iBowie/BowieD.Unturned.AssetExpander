using SDG.Unturned;

namespace BowieD.Unturned.AssetExpander.Models
{
    public interface ICustomField
    {
        /// <summary>
        /// Field Name
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Asset Type
        /// </summary>
        EAssetType Type { get; }
        /// <summary>
        /// Used on start of the server to init all the custom fields
        /// </summary>
        void Init();
        /// <summary>
        /// Used to dispose of any custom fields
        /// </summary>
        void Stop();
    }
}
