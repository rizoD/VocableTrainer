using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VocableTrainer
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
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