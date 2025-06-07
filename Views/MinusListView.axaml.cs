using Avalonia.Controls;
using HeroTableParser.ViewModels;
using System.Collections.Generic;

namespace HeroTableParser.Views;

/// <summary>
/// Interaction logic for the MinusListView window.
/// This window allows the user to manage a list of heroes to exclude from selection or processing.
/// It initializes the data context with a <see cref="MinusListViewModel"/> using the provided list of hero names.
/// </summary>
public partial class MinusListView : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MinusListView"/> class.
    /// Sets up the data context with a <see cref="MinusListViewModel"/> constructed from the given hero names.
    /// </summary>
    /// <param name="heroNames">A list of all available hero names for exclusion management.</param>
    public MinusListView(List<string> heroNames)
    {
        InitializeComponent();
        DataContext = new MinusListViewModel(heroNames);
    }
}