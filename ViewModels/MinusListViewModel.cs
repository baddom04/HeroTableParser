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
    internal class MinusListViewModel
    {
        /// <summary>
        /// Loader for persisting the list of excluded heroes to configuration storage.
        /// </summary>
        private readonly ConfigLoader _excludedHeroesLoader;

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
        public string SelectedAntiHero { get; set; } = string.Empty;

        /// <summary>
        /// Command to add the selected anti-hero to the exclusion list.
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddToList { get; }

        /// <summary>
        /// Command to remove the selected anti-hero from the exclusion list.
        /// </summary>
        public ReactiveCommand<Unit, Unit> RemoveFromList { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MinusListViewModel"/> class.
        /// Loads the excluded heroes from configuration and sets up commands and event handlers.
        /// </summary>
        /// <param name="heroNames">The list of all available hero names.</param>
        public MinusListViewModel(List<string> heroNames)
        {
            HeroNames = heroNames;
            _excludedHeroesLoader = new ConfigLoader("HeroesToExcludeLoader");
            string? heroList = _excludedHeroesLoader.Load();
            HeroesToExclude = heroList is null ? [] : [.. heroList.Split("\n").ToList()];
            AddToList = ReactiveCommand.Create(AddToExcludedHeroList);
            RemoveFromList = ReactiveCommand.Create(RemoveFromExcludedHeroList);
            HeroesToExclude.CollectionChanged += HeroesToExclude_CollectionChanged;
        }

        /// <summary>
        /// Handles changes to the <see cref="HeroesToExclude"/> collection by saving the updated list to configuration.
        /// </summary>
        private void HeroesToExclude_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _excludedHeroesLoader.Save(string.Join('\n', HeroesToExclude));
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
            if (!HeroesToExclude.Contains(SelectedAntiHero) && HeroNames.Contains(SelectedAntiHero)) HeroesToExclude.Add(SelectedAntiHero);
        }
    }
}