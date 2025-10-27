using HeroTableParser.Utils;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

namespace HeroTableParser.ViewModels
{
    /// <summary>
    /// ViewModel for managing a list of heroes to exclude from selection or processing.
    /// Handles loading and saving the excluded heroes list using <see cref="ConfigLoader"/>,
    /// and provides commands for adding and removing heroes from the exclusion list.
    /// </summary>
    public class MinusListViewModel
    {
        /// <summary>
        /// Gets the list of all available hero names.
        /// </summary>
        public List<string> HeroNames { get; }

        /// <summary>
        /// Gets the collection of hero names currently marked for exclusion.
        /// Changes to this collection are automatically saved.
        /// </summary>
        public ObservableCollection<string> HeroesToExclude { get; }

        /// <summary>
        /// Gets or sets the currently selected anti-hero (for exclusion operations).
        /// </summary>
        private string _selectedAntiHero = string.Empty;
        public string SelectedAntiHero
        {
            get => _selectedAntiHero;
            set
            {
                if (_selectedAntiHero != value)
                {
                    _selectedAntiHero = value;

                    // Ha a felhasználó kiválasztott egy érvényes hőst, adjuk automatikusan a listához
                    if (!string.IsNullOrWhiteSpace(_selectedAntiHero))
                    {
                        AddToExcludedHeroList();
                    }
                }
            }
        }

        /// <summary>
        /// Command to add the selected anti-hero to the exclusion list.
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddToList { get; }

        /// <summary>
        /// Command to clear the exclusion list entirely.
        /// </summary>
        public ReactiveCommand<Unit, Unit> ClearList { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MinusListViewModel"/> class.
        /// Loads the excluded heroes from configuration and sets up commands and event handlers.
        /// </summary>
        /// <param name="heroNames">The list of all available hero names.</param>
        public MinusListViewModel(List<string> heroNames)
        {
            HeroNames = heroNames;
            HeroesToExclude = new ObservableCollection<string>();
            AddToList = ReactiveCommand.Create(AddToExcludedHeroList);
            ClearList = ReactiveCommand.Create(() => HeroesToExclude.Clear());
        }


        /// <summary>
        /// Default constructor — used when hero names will be set later.
        /// </summary>
        public MinusListViewModel()
        {
            HeroNames = new List<string>();
            HeroesToExclude = new ObservableCollection<string>();

            AddToList = ReactiveCommand.Create(AddToExcludedHeroList);
            ClearList = ReactiveCommand.Create(() => HeroesToExclude.Clear());
        }

        /// <summary>
        /// Removes the currently selected anti-hero from the exclusion list.
        /// </summary>
        private void RemoveFromExcludedHeroList()
        {
            HeroesToExclude.Remove(SelectedAntiHero);
        }

        /// <summary>
        /// Adds the currently selected anti-hero to the exclusion list if it is not already present and is a valid hero name.
        /// </summary>
        private void AddToExcludedHeroList()
        {
            if (!HeroesToExclude.Contains(SelectedAntiHero) && HeroNames.Contains(SelectedAntiHero))
            {
                HeroesToExclude.Add(SelectedAntiHero);
            }
        }
    }
}
