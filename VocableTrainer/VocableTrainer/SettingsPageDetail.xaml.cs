using System;
using System.IO;
using System.Linq;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
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
			Update();
		}

		private void Update()
		{
			AutoPause.IsToggled = Settings.PlayAnswer;
			NativeVoice.Text = Settings.NativeVoice;
			Storage.SelectedItem = App.Data.DataStores.FirstOrDefault(item => item.Item2 == Settings.FilePath);
			UpdateSelected();
		}

		private void UpdateSelected()
		{
			App.Data.CurrentLanguage = App.Data.Languages.FirstOrDefault(item => item.Id == Settings.CurrentLanguage);
			if (App.Data.CurrentLanguage == null)
			{
				App.Data.CurrentLanguage = App.Data.Languages.First();
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

		private void NativeVoice_OnTextChanged(object sender, TextChangedEventArgs e)
		{
			Settings.NativeVoice = e.NewTextValue;
		}

		private void Storage_OnSelectedIndexChanged(object sender, EventArgs e)
		{
			var tmp = Storage.SelectedItem as Tuple<string, string>;
			if (tmp != null)
			{
				Settings.FilePath = tmp.Item2;
				App.Data.LoadFile(Settings.FilePath);
				Update();
			}
			
		}
	}
}