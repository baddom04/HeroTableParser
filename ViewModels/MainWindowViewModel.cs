using DynamicData;
using HeroTableParser.Models;
using HeroTableParser.Utils;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HeroTableParser.ViewModels
{
    /// <summary>
    /// The main ViewModel for the application's primary window.
    /// Manages hero data, anti-hero selections, sheet selection, and provides logic for determining
    /// the best heroes to choose based on user input and loaded Excel data.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private KeyValuePair<string, int>? _selectedHero;
        public KeyValuePair<string, int>? SelectedHero
        {
            get { return _selectedHero; }
            set { this.RaiseAndSetIfChanged(ref _selectedHero, value); }
        }

        private ObservableCollection<Hero> _antiHeroes = [];

        /// <summary>
        /// Gets or sets the collection of anti-hero selections.
        /// </summary>
        public ObservableCollection<Hero> AntiHeroes
        {
            get { return _antiHeroes; }
            set { this.RaiseAndSetIfChanged(ref _antiHeroes, value); }
        }

        private ObservableCollection<KeyValuePair<string, int>> _bestToChoose = [];

        /// <summary>
        /// Gets or sets the collection of best heroes to choose, ordered by calculated score.
        /// </summary>
        public ObservableCollection<KeyValuePair<string, int>> BestToChoose
        {
            get { return _bestToChoose; }
            set { this.RaiseAndSetIfChanged(ref _bestToChoose, value); }
        }

        // A MinusListViewModel tulajdonság
        public MinusListViewModel MinusListViewModel { get; private set; }

        public ObservableCollection<KeyValuePair<string, int>> HeroCounteredBy { get; set; } = [];

        public ObservableCollection<Hero> FriendlyHeroes { get; set; } = [];

        private List<string> _heroNames = [];
        public List<string> HeroNames
        {
            get { return _heroNames; }
            set { this.RaiseAndSetIfChanged(ref _heroNames, value); }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// Sets up initial state, subscribes to Excel path changes, and loads initial data.
        /// </summary>
        public MainWindowViewModel()
        {
            MinusListViewModel = new MinusListViewModel();

            Init();

            MinusListViewModel = new MinusListViewModel(HeroNames);
            MinusListViewModel.HeroesToExclude.CollectionChanged += (_, __) =>
            {
                OnAntiHeroSelectionChanged();
            };
            HeroService.Instance.HeroesUpdated += () => { HeroCounteredBy.Clear(); HeroNames = [.. HeroService.Instance.Heroes.Select(h => h.Name)]; Init(); };

            this.WhenAnyValue(vm => vm.SelectedHero).Subscribe(selectedHero => OnSelectedHeroChanged(selectedHero));
        }

        /// <summary>
        /// Initializes or refreshes hero and anti-hero data based on the current Excel file and selected sheet.
        /// </summary>
        public void Init()
        {
            if (HeroService.Instance.Heroes.Count == 0) return;

            // Ha van MinusListViewModel, frissítsük a hősneveket
            if (MinusListViewModel is not null)
            {
                MinusListViewModel.HeroNames.Clear();
                MinusListViewModel.HeroNames.AddRange(HeroNames);
            }

            AntiHeroes.ToList().ForEach(ah => ah.HeroNameChanged -= OnAntiHeroSelectionChanged);
            AntiHeroes.Clear();
            FriendlyHeroes.Clear();

            // Add default anti-hero slots
            for (int i = 0; i < 5; i++)
            {
                AddHeroToObservedCollection(AntiHeroes);
                if (i != 0) AddHeroToObservedCollection(FriendlyHeroes);
            }
        }

        private void OnSelectedHeroChanged(KeyValuePair<string, int>? selectedBestToChooseHero)
        {
            HeroCounteredBy.Clear();

            if (selectedBestToChooseHero is null)
            {
                return;
            }

            foreach (var antiHero in AntiHeroes.Where(ah => !string.IsNullOrWhiteSpace(ah.Name)))
            {
                var hero = HeroService.Instance.Heroes.SingleOrDefault(h => h.Name == antiHero.Name);

                if (hero is null) { continue; }

                var strength = hero.HeroColorPairs.FirstOrDefault(pair => pair.Value.Contains(selectedBestToChooseHero.Value.Key)).Key;

                if (strength != StrengthType.Empty)
                {
                    HeroCounteredBy.Add(new KeyValuePair<string, int>(antiHero.Name, (int)strength));
                }
            }
        }

        /// <summary>
        /// Adds a new anti-hero to the selection and updates the best-to-choose list.
        /// </summary>
        private void AddHeroToObservedCollection(ObservableCollection<Hero> heroes)
        {
            Hero ah = new(string.Empty);
            ah.HeroNameChanged += OnAntiHeroSelectionChanged;
            heroes.Add(ah);
            OnAntiHeroSelectionChanged();
        }

        /// <summary>
        /// Recalculates the best heroes to choose based on the current anti-hero selections.
        /// Updates the <see cref="BestToChoose"/> collection.
        /// </summary>
        private void OnAntiHeroSelectionChanged()
        {
            BestToChoose.Clear();

            Dictionary<string, int> bestAgainst = [];

            foreach (var antihero in AntiHeroes)
            {
                var hero = HeroService.Instance.Heroes.FirstOrDefault(h => h.Name == antihero.Name);

                if (hero is null) continue;

                foreach (StrengthType strength in Enum.GetValues(typeof(StrengthType)))
                {
                    var heroesForStrength = hero.HeroColorPairs[strength];
                    foreach (string inspectedHero in heroesForStrength)
                    {
                        bestAgainst.TryAdd(inspectedHero, 0);
                        bestAgainst[inspectedHero] += (int)strength;
                    }
                }
            }

            var excludedHeroes = new HashSet<string>(MinusListViewModel.HeroesToExclude);
            excludedHeroes.UnionWith(AntiHeroes.Where(ah => !string.IsNullOrWhiteSpace(ah.Name)).Select(ah => ah.Name));
            excludedHeroes.UnionWith(FriendlyHeroes.Select(fh => fh.Name));

            var filtered = bestAgainst
            .Where(pair => !excludedHeroes.Contains(pair.Key))
            .ToDictionary(pair => pair.Key, pair => pair.Value);

            BestToChoose.AddRange(filtered.OrderByDescending(pair => pair.Value).Take(5));
        }
    }
}
