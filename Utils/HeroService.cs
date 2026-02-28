using HeroTableParser.Models;
using System;
using System.Collections.Generic;

namespace HeroTableParser.Utils
{
    public class HeroService
    {
        /// <summary>
        /// Provides application-level information and Excel file path management.
        /// </summary>
        public ApplicationInfo AppInfo { get; } 
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
            set
            {
                _selectedSheet = value;
                Init();
            }
        }
        public List<Hero> Heroes { get; private set; } = [];
        public event Action? HeroesUpdated;
        private HeroService()
        {
            _selectedSheet = Sheets[0];
            AppInfo = new ApplicationInfo();
            AppInfo.ExcelPathChanged += Init;

            Init();
        }

        public void Init()
        {
            if (AppInfo.ExcelPath is null) return;

            Heroes = ExcelLoader.LoadTable(AppInfo.ExcelPath, Sheets.IndexOf(SelectedSheet));
            HeroesUpdated?.Invoke();
        }
        private static HeroService? _instance;
        public static HeroService Instance
        {
            get { return _instance ??= new HeroService(); }
            private set { _instance = value; }
        }
    }
}
