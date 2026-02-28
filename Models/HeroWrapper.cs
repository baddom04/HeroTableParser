using ReactiveUI;
using HeroTableParser.Utils;
using System;
using System.Linq;

namespace HeroTableParser.Models
{
    public class HeroWrapper: ReactiveObject
    {
        private string _nameInput;
        public string NameInput
        {
            get { return _nameInput; }
            set { this.RaiseAndSetIfChanged(ref _nameInput, value); }
        }
        private Hero? _hero;

        public Hero? Hero
        {
            get { return _hero; }
            set { this.RaiseAndSetIfChanged(ref _hero, value); }
        }

        public HeroWrapper(string name)
        {
            _nameInput = name;
            this.WhenAnyValue(x => x.NameInput).Subscribe(name => Hero = HeroService.Instance.Heroes.FirstOrDefault(h => h.Name == name));
        }
    }
}
