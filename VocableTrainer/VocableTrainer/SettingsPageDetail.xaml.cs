using System;
using System.IO;
using System.Linq;
using Android.Widget;
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
			NativeVoice.Text = Settings.NativeVoice;
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


		private async void ImportBtn_OnClicked(object sender, EventArgs e)
		{
			var file = await CrossFilePicker.Current.PickFile();

			if (file != null)
			{
				using (Stream fileStream = File.OpenWrite(App.Data.DBPath))
				{
					file.GetStream().CopyTo(fileStream);
				}
				App.Data.LoadFile();
			}
		}

		private async void ExportBtn_OnClickedBtn_OnClicked(object sender, EventArgs e)
		{
			try
			{
				var bytes = System.IO.File.ReadAllBytes(App.Data.DBPath);
				var fileCopyName = $"{ViewModel.DBFile}_{System.DateTime.Now:yyyy-MM-dd_HH-mm}.db3";
				var download = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
				string path = Path.Combine(download, fileCopyName);
				System.IO.File.WriteAllBytes(path, bytes);
			}
			catch (Exception ex)
			{
				DisplayAlert("Error", ex.Message, "OK");
			}
		}
	}
}