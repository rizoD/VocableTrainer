using System;
using System.Linq;

namespace VocableTrainer
{
	public partial class EditLangPage : ContentPage
	{
		public EditLangPage()
		{
			InitializeComponent();
			Disappearing += OnDisappearing;
			LangText.Focus();
		}

		private void OnDisappearing(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(App.Data.EditLanguage.Name) &&
			    !string.IsNullOrWhiteSpace(App.Data.EditLanguage.Voice))
			{
				App.Data.SaveLang(App.Data.EditLanguage);
				App.Data.CurrentLanguage = App.Data.EditLanguage;
			}
		}

		private async void DelBtn_OnClicked(object sender, EventArgs e)
		{
			bool answer = await DisplayAlert("Question?", "Would you like to delete this Language?", "Yes", "No");
			if (answer)
			{
				App.Data.DeleteLang(App.Data.EditLanguage);
				App.Data.CurrentLanguage = App.Data.Languages.FirstOrDefault();
				Navigation.PopModalAsync();
			}
		}

	}
}