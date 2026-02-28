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
        VeryGood = 2,

        /// <summary>
        /// Indicates above average or good strength.
        /// </summary>
        Good = 1,

        /// <summary>
        /// Indicates a neutral or average strength.
        /// </summary>
        Neutral = 0,

        /// <summary>
        /// Indicates below average or poor strength.
        /// </summary>
        Bad = -1,

        /// <summary>
        /// Indicates very low or exceptionally poor strength.
        /// </summary>
        VeryBad = -2,

        /// <summary>
        /// Indicates the absence of a strength value.
        /// </summary>
        Empty = -99
    }
}