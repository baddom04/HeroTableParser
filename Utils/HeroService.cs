using DynamicData;
using HeroTableParser.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HeroTableParser.Utils
{
    public class HeroService : ReactiveObject
    {
        public ApplicationInfo AppInfo { get; } 
        public List<string> Sheets { get; } = ["All", "Gold", "Exp", "Mid", "Jungle", "Roam"];

        private string _selectedSheet;
        public string SelectedSheet
        {
            get { return _selectedSheet; }
            set { this.RaiseAndSetIfChanged(ref _selectedSheet, value); }
        }
        
        public ObservableCollection<Hero> Heroes { get; } = [];
        public ObservableCollection<string> HeroNames { get; } = [];
        
        public event Action? HeroesUpdated;
        
        private static HeroService? _instance;
        public static HeroService Instance => _instance ??= new HeroService();

        private HeroService()
        {
            _selectedSheet = Sheets[0];
            AppInfo = new ApplicationInfo();
            AppInfo.ExcelPathChanged += Init;
            this.WhenAnyValue(x => x.SelectedSheet).Subscribe(_ => Init());

            Init();
        }

        public void Init()
        {
            if (AppInfo.ExcelPath is null) return;

            Heroes.Clear();
            Heroes.AddRange(ExcelLoader.LoadTable(AppInfo.ExcelPath, Sheets.IndexOf(SelectedSheet)));

            HeroNames.Clear(); 
            HeroNames.AddRange(Heroes.Select(h => h.Name));
            
            HeroesUpdated?.Invoke();
        }
    }
}
