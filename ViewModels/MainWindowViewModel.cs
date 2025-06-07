using DynamicData;
using HeroTableParser.Models;
using HeroTableParser.Utils;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

namespace HeroTableParser.ViewModels
{
    /// <summary>
    /// The main ViewModel for the application's primary window.
    /// Manages hero data, anti-hero selections, sheet selection, and provides logic for determining the best heroes to choose
    /// based on user input and loaded Excel data.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// List of available sheet names for selection.
        /// </summary>
        public List<string> Sheets { get; } = ["All", "Gold", "Exp", "Mid", "Jungle", "Roam"];

        private string _selectedSheet;

        /// <summary>
        /// Gets or sets the currently selected sheet name.
        /// Changing this value re-initializes the hero data.
        /// </summary>
        public string SelectedSheet
        {
            get { return _selectedSheet; }
            set { this.RaiseAndSetIfChanged(ref _selectedSheet, value); Init(); }
        }

        /// <summary>
        /// Maps <see cref="StrengthType"/> to their corresponding point values for scoring.
        /// </summary>
        public Dictionary<StrengthType, int> StrengthPoints = new()
        {
            { StrengthType.VeryGood, 2 },
            { StrengthType.Good,     1 },
            { StrengthType.Neutral,  0 },
            { StrengthType.Bad,     -1 },
            { StrengthType.VeryBad, -2 },
            { StrengthType.Empty,    0 },
        };

        /// <summary>
        /// Provides application-level information and Excel file path management.
        /// </summary>
        public ApplicationInfo AppInfo { get; }

        private List<Hero> _heroes;

        /// <summary>
        /// Gets or sets the list of loaded heroes from the Excel file.
        /// </summary>
        public List<Hero> Heroes
        {
            get { return _heroes; }
            set { this.RaiseAndSetIfChanged(ref _heroes, value); }
        }

        /// <summary>
        /// Gets the collection of hero names for display or selection.
        /// </summary>
        public IReadOnlyCollection<string> HeroNames { get; private set; }

        private ObservableCollection<AntiHero> _antiHeroes = [];

        /// <summary>
        /// Gets or sets the collection of anti-hero selections.
        /// </summary>
        public ObservableCollection<AntiHero> AntiHeroes
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

        /// <summary>
        /// Command to remove an anti-hero from the selection.
        /// </summary>
        public ReactiveCommand<AntiHero, Unit> DeleteAntiHeroCommand { get; private set; }

        /// <summary>
        /// Command to add a new anti-hero to the selection.
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddAntiHeroCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// Sets up initial state, subscribes to Excel path changes, and loads initial data.
        /// </summary>
        public MainWindowViewModel()
        {
            _selectedSheet = Sheets[0];
            AppInfo = new ApplicationInfo();
            AppInfo.ExcelPathChanged += Init;

            Init();
        }

        /// <summary>
        /// Initializes or refreshes hero and anti-hero data based on the current Excel file and selected sheet.
        /// </summary>
        public void Init()
        {
            if (AppInfo.ExcelPath is null) return;

            Heroes = ExcelLoader.LoadTable(AppInfo.ExcelPath, Sheets.IndexOf(SelectedSheet));

            if (Heroes.Count == 0) return;

            HeroNames = Heroes.Select(x => x.Name).ToList();

            // Unsubscribe from previous anti-hero events and clear the collection
            AntiHeroes.ToList().ForEach(ah => ah.SelectedAntiHeroChanged -= OnAntiHeroSelectionChanged);
            AntiHeroes.Clear();

            // Add default anti-hero slots
            for (int i = 0; i < 5; i++)
            {
                AddAntiHero();
            }

            DeleteAntiHeroCommand = ReactiveCommand.Create<AntiHero>(RemoveAntiHero);
            AddAntiHeroCommand = ReactiveCommand.Create(AddAntiHero);
        }

        /// <summary>
        /// Removes the specified anti-hero from the selection and updates the best-to-choose list.
        /// </summary>
        /// <param name="ah">The anti-hero to remove.</param>
        private void RemoveAntiHero(AntiHero ah)
        {
            AntiHeroes.Remove(ah);
            ah.SelectedAntiHeroChanged -= OnAntiHeroSelectionChanged;
            OnAntiHeroSelectionChanged();
        }

        /// <summary>
        /// Adds a new anti-hero to the selection and updates the best-to-choose list.
        /// </summary>
        private void AddAntiHero()
        {
            AntiHero ah = new();
            ah.SelectedAntiHeroChanged += OnAntiHeroSelectionChanged;
            AntiHeroes.Add(ah);
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

            foreach (AntiHero antihero in AntiHeroes)
            {
                if (Heroes.Find(h => h.Name == antihero.SelectedAntiHero) is null) continue;

                foreach (StrengthType strength in Enum.GetValues(typeof(StrengthType)))
                {
                    var vmi = Heroes.Find(h => h.Name == antihero.SelectedAntiHero)!.HeroColorPairs[strength];
                    foreach (string hero in vmi)
                    {
                        if (!bestAgainst.ContainsKey(hero))
                        {
                            bestAgainst[hero] = 0;
                        }
                        bestAgainst[hero] += StrengthPoints[strength];
                    }
                }
            }

            BestToChoose.AddRange(bestAgainst);
            FilterAndOrder();
        }

        /// <summary>
        /// Orders the <see cref="BestToChoose"/> collection by score and keeps only the top 5 entries.
        /// </summary>
        private void FilterAndOrder()
        {
            BestToChoose = [.. BestToChoose.OrderByDescending(pair => pair.Value).Take(5)];
        }
    }
}