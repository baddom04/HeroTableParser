namespace HeroTableParser.Models
{
    public class HeroWrapper
    {
		private string _nameInput;

		public string NameInput
		{
			get { return _nameInput; }
			set { _nameInput = value; OnNameInputChanged(); }
		}

		public Hero Hero { get; set; }


		private void OnNameInputChanged()
		{

		}
    }
}
