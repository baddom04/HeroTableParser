namespace HeroTableParser.Models
{
    /// <summary>
    /// Represents the qualitative strength classification for a hero or entity.
    /// Used to indicate how strong or weak a particular character is.
    /// </summary>
    public enum StrengthType
    {
        /// <summary>
        /// Indicates a very high or exceptional strength.
        /// </summary>
        VeryGood,

        /// <summary>
        /// Indicates above average or good strength.
        /// </summary>
        Good,

        /// <summary>
        /// Indicates a neutral or average strength.
        /// </summary>
        Neutral,

        /// <summary>
        /// Indicates below average or poor strength.
        /// </summary>
        Bad,

        /// <summary>
        /// Indicates very low or exceptionally poor strength.
        /// </summary>
        VeryBad,

        /// <summary>
        /// Indicates the absence of a strength value.
        /// </summary>
        Empty
    }
}