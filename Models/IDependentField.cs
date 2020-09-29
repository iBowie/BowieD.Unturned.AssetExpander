namespace BowieD.Unturned.AssetExpander.Models
{
    public interface IDependentField : ICustomField
    {
        /// <summary>
        /// Used to determine if this field can be initialized
        /// </summary>
        string[] Dependencies { get; set; }
    }
}
