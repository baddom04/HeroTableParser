using ReactiveUI;
using System;
using System.Collections.Generic;

namespace HeroTableParser.Models
{
    /// <summary>
    /// Represents a hero entity with a name and categorized color associations.
    /// Inherits from <see cref="ReactiveObject"/> to support property change notifications for data binding.
    /// </summary>
    public class Hero(string name) : ReactiveObject
    {
        /// <summary>
        /// Gets or sets the name of the hero.
        /// </summary>
        private string _name = name;
        public string Name
        {
            get { return _name; }
            set { _name = value; HeroNameChanged?.Invoke(); }
        }

        public event Action? HeroNameChanged;

        /// <summary>
        /// Gets or sets the mapping between <see cref="StrengthType"/> and a list of color names or identifiers.
        /// Each strength category is associated with a list of colors relevant to the hero.
        /// </summary>
        public Dictionary<StrengthType, List<string>> HeroColorPairs { get; set; } = new()
        {
            { StrengthType.VeryGood, [] },
            { StrengthType.Good, [] },
            { StrengthType.Neutral, [] },
            { StrengthType.Bad, [] },
            { StrengthType.VeryBad, [] },
            { StrengthType.Empty, [] },
        };
    }
}