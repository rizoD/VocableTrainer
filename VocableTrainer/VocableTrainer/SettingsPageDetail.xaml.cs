using System;
using System.IO;
using System.Linq;
using Android.App;
using Android.Content.PM;
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

			PlayChime.IsToggled = Settings.PlayRcChime;
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
			try
			{
				var file = await CrossFilePicker.Current.PickFile();
				if (file != null)
				{
					App.ShowLoading(() =>
					{
						using (Stream fileStream = File.OpenWrite(App.Data.DBPath))
						{
							file.GetStream().CopyTo(fileStream);
						}
						// Code to run on the main thread
						Device.BeginInvokeOnMainThread(App.Data.LoadFile);

					}, Toast.MakeText(Android.App.Application.Context, "DB import completed", ToastLength.Long).Show);
				}
			}
			catch (Exception ex)
			{
				DisplayAlert("Error", ex.Message, "OK");
			}
		}


		private async void ExportBtn_OnClickedBtn_OnClicked(object sender, EventArgs e)
		{
			await Permissions.RequestAsync<Permissions.StorageWrite>().ConfigureAwait(false);

			if(await Permissions.CheckStatusAsync<Permissions.StorageWrite>().ConfigureAwait(false) != PermissionStatus.Granted)
			{
				Toast.MakeText(Android.App.Application.Context, "No permission to write export file.", ToastLength.Long).Show();
				return;
			}
			App.ShowLoading(() =>
			{

				var download = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).ToString();
				if (!System.IO.Directory.Exists(download))
				{
					throw new ApplicationException("Download directory not existing");
				}

				var bytes = System.IO.File.ReadAllBytes(App.Data.DBPath);
				var fileCopyName = $"{ViewModel.DBFile}_{System.DateTime.Now:yyyy-MM-dd_HH-mm}.db3";
				string path = Path.Combine(download, fileCopyName);
				System.IO.File.WriteAllBytes(path, bytes);

			}, Toast.MakeText(Android.App.Application.Context, "DB export completed", ToastLength.Long).Show);
		}

		private void PlayChime_OnToggled(object sender, ToggledEventArgs e)
		{
			Settings.PlayRcChime = e.Value;
		}

		private void AddLang_OnClicked(object sender, EventArgs e)
		{
			App.Data.EditLanguage = new Language();
			Navigation.PushModalAsync(new EditLangPage());
		}

		private void EditLang_OnClicked(object sender, EventArgs e)
		{
			App.Data.EditLanguage = App.Data.CurrentLanguage;
			Navigation.PushModalAsync(new EditLangPage());
		}

	}
}