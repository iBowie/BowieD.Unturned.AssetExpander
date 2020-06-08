namespace BowieD.Unturned.AssetExpander
{
    public enum ESearchMode
    {
        OFF = 0,
        ASSET = 1 << 0,
        CONFIG = 1 << 1,
        FULL = ASSET | CONFIG
    }
}
