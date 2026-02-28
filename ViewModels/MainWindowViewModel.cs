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
    public class MainWindowViewModel : ViewModelBase
    {
        private KeyValuePair<string, int>? _selectedHero;
        public KeyValuePair<string, int>? SelectedHero
        {
            get { return _selectedHero; }
            set { this.RaiseAndSetIfChanged(ref _selectedHero, value); }
        }

        private ObservableCollection<HeroWrapper> _antiHeroes = [];
        public ObservableCollection<HeroWrapper> AntiHeroes
        {
            get { return _antiHeroes; }
            set { this.RaiseAndSetIfChanged(ref _antiHeroes, value); }
        }

        private ObservableCollection<KeyValuePair<string, int>> _bestToChoose = [];
        public ObservableCollection<KeyValuePair<string, int>> BestToChoose
        {
            get { return _bestToChoose; }
            set { this.RaiseAndSetIfChanged(ref _bestToChoose, value); }
        }

        public MinusListViewModel MinusListViewModel { get; private set; }
        public ObservableCollection<KeyValuePair<string, int>> HeroCounteredBy { get; set; } = [];
        public ObservableCollection<HeroWrapper> FriendlyHeroes { get; set; } = [];

        private readonly List<IDisposable> _subscriptions = [];

        public MainWindowViewModel()
        {
            MinusListViewModel = new MinusListViewModel();
            Init();

            MinusListViewModel = new MinusListViewModel([.. HeroService.Instance.HeroNames]);
            MinusListViewModel.HeroesToExclude.CollectionChanged += (_, __) => OnAntiHeroSelectionChanged();

            HeroService.Instance.HeroesUpdated += () =>
            {
                HeroCounteredBy.Clear();
                Init();
            };

            this.WhenAnyValue(vm => vm.SelectedHero).Subscribe(selectedHero => OnSelectedBestToChooseHeroChanged(selectedHero));
        }

        public void Init()
        {
            if (HeroService.Instance.Heroes.Count == 0) return;

            if (MinusListViewModel is not null)
            {
                MinusListViewModel.HeroNames.Clear();
                MinusListViewModel.HeroNames.AddRange([.. HeroService.Instance.HeroNames]);
            }

            _subscriptions.ForEach(subscription => subscription.Dispose());
            AntiHeroes.Clear();
            FriendlyHeroes.Clear();

            for (int i = 0; i < 5; i++)
            {
                AddHeroToObservedCollection(AntiHeroes);
                if (i != 0) AddHeroToObservedCollection(FriendlyHeroes);
            }
        }

        private void OnSelectedBestToChooseHeroChanged(KeyValuePair<string, int>? selectedBestToChooseHero)
        {
            HeroCounteredBy.Clear();

            if (selectedBestToChooseHero is null)
            {
                return;
            }

            foreach (var antiHero in AntiHeroes.Where(ah => ah.Hero is not null))
            {
                var strength = antiHero.Hero!.HeroColorPairs.FirstOrDefault(pair => pair.Value.Contains(selectedBestToChooseHero.Value.Key)).Key;

                if (strength != StrengthType.Empty)
                {
                    HeroCounteredBy.Add(new KeyValuePair<string, int>(antiHero.Hero!.Name, (int)strength));
                }
            }
        }

        private void AddHeroToObservedCollection(ObservableCollection<HeroWrapper> heroes)
        {
            HeroWrapper ah = new(string.Empty);
            _subscriptions.Add(ah.WhenAnyValue(x => x.Hero).Subscribe(_ => OnAntiHeroSelectionChanged()));
            heroes.Add(ah);
        }

        private void OnAntiHeroSelectionChanged()
        {
            BestToChoose.Clear();

            Dictionary<string, int> bestAgainst = [];

            foreach (var antihero in AntiHeroes.Where(ah => ah.Hero is not null))
            {
                foreach (StrengthType strength in Enum.GetValues(typeof(StrengthType)))
                {
                    var heroesForStrength = antihero.Hero!.HeroColorPairs[strength];
                    foreach (string inspectedHero in heroesForStrength)
                    {
                        bestAgainst.TryAdd(inspectedHero, 0);
                        bestAgainst[inspectedHero] += (int)strength;
                    }
                }
            }

            var excludedHeroes = new HashSet<string>(MinusListViewModel.HeroesToExclude);
            excludedHeroes.UnionWith(AntiHeroes.Where(ah => ah.Hero is not null).Select(ah => ah.Hero!.Name));
            excludedHeroes.UnionWith(FriendlyHeroes.Where(fh => fh.Hero is not null).Select(fh => fh.Hero!.Name));

            var filtered = bestAgainst
                    .Where(pair => !excludedHeroes.Contains(pair.Key))
                    .ToDictionary(pair => pair.Key, pair => pair.Value);

            BestToChoose.AddRange(filtered.OrderByDescending(pair => pair.Value).Take(5));
        }
    }
}
