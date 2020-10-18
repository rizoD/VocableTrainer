using System;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VocableTrainer
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsPageDetail : ContentPage
	{
		public SettingsPageDetail()
		{
			InitializeComponent();
			AutoPause.IsToggled = Settings.PlayAnswer;
			UpdateSelected();
		}

		private void UpdateSelected()
		{
			Language.SelectedItem = App.Data.Languages.FirstOrDefault(item => item.Id == Settings.CurrentLanguage);
			if (Language.SelectedItem == null)
			{
				Language.SelectedItem = App.Data.Languages.First();
			}
		}

		private void Switch_OnToggled(object sender, ToggledEventArgs e)
		{
			Settings.PlayAnswer = e.Value;
		}

		private async void EditLang_OnClicked(object sender, EventArgs e)
		{
			Navigation.PushModalAsync(new EditLangPage());
		}

		private void Language_OnSelectedIndexChanged(object sender, EventArgs e)
		{
			var item = Language.SelectedItem as Language;
			if (item != null)
			{
				Settings.CurrentLanguage = item.Id;
			}
		}

		private void SettingsPageDetail_OnAppearing(object sender, EventArgs e)
		{
			UpdateSelected();
		}
	}
}