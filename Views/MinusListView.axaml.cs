using Avalonia.Controls;
using HeroTableParser.ViewModels;
using System.Collections.Generic;

namespace HeroTableParser.Views
{
    /// <summary>
    /// Interaction logic for the MinusListView user control.
    /// This control allows the user to manage a list of heroes to exclude from selection or processing.
    /// </summary>
    public partial class MinusListView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MinusListView"/> class.
        /// The DataContext is expected to be provided externally (for example, from <see cref="MainWindowViewModel"/>).
        /// </summary>
        public MinusListView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Optional helper method to initialize the DataContext manually,
        /// if a list of hero names is provided outside of the main view model.
        /// </summary>
        /// <param name="heroNames">List of hero names available for exclusion.</param>
        public void InitializeWithHeroes(List<string> heroNames)
        {
            DataContext = new MinusListViewModel(heroNames);
        }
    }
}
