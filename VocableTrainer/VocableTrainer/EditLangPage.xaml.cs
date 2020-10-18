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
		private Language selectedLanguage;
		public EditLangPage()
		{
			InitializeComponent();
			selectedLanguage = App.Data.Languages.FirstOrDefault(item => item.Id == Settings.CurrentLanguage);
			if (selectedLanguage == null || selectedLanguage.Id == 0)
			{
				selectedLanguage = new Language();
			}
			LangText.Text = selectedLanguage.Name;
			LangText.Focus();
		}

		private async void DelBtn_OnClicked(object sender, EventArgs e)
		{
			bool answer = await DisplayAlert("Question?", "Would you like to delete this Language?", "Yes", "No");
			if (answer)
			{
				if (selectedLanguage.Id > 0)
				{
					App.Data.DeleteLang(selectedLanguage);
				}

				Navigation.PopModalAsync();
			}
		}

		private void CancelBtn_OnClicked(object sender, EventArgs e)
		{
			Navigation.PopModalAsync();
		}

		private void OKBtn_OnClicked(object sender, EventArgs e)
		{
			selectedLanguage.Name = LangText.Text; 
			App.Data.SaveLang(selectedLanguage);
			Navigation.PopModalAsync();
		}

	}
}