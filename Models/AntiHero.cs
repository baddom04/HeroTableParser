using ReactiveUI;
using System;
using System.Reactive;

namespace HeroTableParser.Models
{
    /// <summary>
    /// Represents an anti-hero entity with a selectable name.
    /// Provides property change notifications and a command to clear the selection.
    /// </summary>
    public class AntiHero : ReactiveObject
    {
        private string _selectedAntiHero = string.Empty;

        /// <summary>
        /// Gets or sets the currently selected anti-hero's name.
        /// Notifies listeners when the value changes and raises the <see cref="SelectedAntiHeroChanged"/> event.
        /// </summary>
        public string SelectedAntiHero
        {
            get { return _selectedAntiHero; }
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedAntiHero, value);
                SelectedAntiHeroChanged?.Invoke();
            }
        }

        /// <summary>
        /// Event triggered whenever <see cref="SelectedAntiHero"/> changes.
        /// </summary>
        public event Action? SelectedAntiHeroChanged;

        /// <summary>
        /// Command to clear the <see cref="SelectedAntiHero"/> property.
        /// </summary>
        public ReactiveCommand<Unit, string> DeleteTextCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AntiHero"/> class.
        /// Sets up the command to clear the selected anti-hero.
        /// </summary>
        public AntiHero()
        {
            DeleteTextCommand = ReactiveCommand.Create(() => SelectedAntiHero = string.Empty);
        }
    }
}