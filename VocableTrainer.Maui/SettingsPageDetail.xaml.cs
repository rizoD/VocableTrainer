using Avalonia.Platform.Storage;

namespace VocableTrainer.Maui;
public partial class SettingsPageDetail : ContentPage
{
	public ViewModel Data { get; }
	public SettingsPageDetail()
	{
		Data = App.Data;
		InitializeComponent();

		foreach (var item in Enum.GetValues(typeof(ControlAction)))
		{
			PrevPicker.Items.Add(item.ToString());
			PlayPicker.Items.Add(item.ToString());
			NextPicker.Items.Add(item.ToString());

		}
		Update();
	}

	private void Update()
	{
		NativeVoice.Text = SettingsService.Settings.NativeVoice;
		UpdateSelected();
	}

	private void UpdateSelected()
	{
		if (App.Data.Languages != null && App.Data.Languages.Count > 0)
		{
			App.Data.CurrentLanguage =
				App.Data.Languages.FirstOrDefault(item => item.Id == SettingsService.Settings.CurrentLanguage);
			if (App.Data.CurrentLanguage == null)
			{
				App.Data.CurrentLanguage = App.Data.Languages.First();
			}
		}

		PrevPicker.SelectedIndex = (int)SettingsService.Settings.PrevAction;
		PlayPicker.SelectedIndex = (int)SettingsService.Settings.PlayPauseAction;
		NextPicker.SelectedIndex = (int)SettingsService.Settings.NextAction;

		PlayChime.IsToggled = SettingsService.Settings.PlayRcChime;
		AllwaysPlayChime.IsToggled = SettingsService.Settings.AllwaysPlayChime;
	}

	private void SettingsPageDetail_OnAppearing(object sender, EventArgs e)
	{
		UpdateSelected();
	}

	private void NativeVoice_OnTextChanged(object sender, TextChangedEventArgs e)
	{
		SettingsService.Settings.NativeVoice = e.NewTextValue;
	}



	private async void ImportBtn_OnClicked(object sender, EventArgs e)
	{
		try
		{

			var file = await FilePicker.Default.PickAsync(new PickOptions() { PickerTitle = "Import Vocable database " });
			if (file != null)
			{
				App.ShowLoading(async () =>
				{
					using Stream readStream = System.IO.File.OpenRead(file.FullPath);
					using StreamReader reader = new StreamReader(readStream);
					using (Stream writeStream = File.OpenWrite(App.Data.DBPath))
					using (TextWriter writer = new StreamWriter(writeStream))
					{ 
						var content = await reader.ReadToEndAsync();
						{
							await writer.WriteAsync(content);
						}
					}
					// Code to run on the main thread
					Device.BeginInvokeOnMainThread(App.Data.LoadFile);

				}, ()=> App.ShowToast("DB import completed"));
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

		if (await Permissions.CheckStatusAsync<Permissions.StorageWrite>().ConfigureAwait(false) != PermissionStatus.Granted)
		{
			App.ShowToast("No permission to write export file.");
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

		}, () => App.ShowToast("DB export completed"));
	}

	private void PlayChime_OnToggled(object sender, ToggledEventArgs e)
	{
		SettingsService.Settings.PlayRcChime = e.Value;
		if (SettingsService.Settings.PlayRcChime)
		{
			AllwaysPlayChime.IsToggled = false;
		}
	}

	private void AllwaysPlayChime_OnToggled(object sender, ToggledEventArgs e)
	{
		SettingsService.Settings.AllwaysPlayChime = e.Value;
		if (SettingsService.Settings.AllwaysPlayChime)
		{
			PlayChime.IsToggled = false;
		}
	}

	private void PrevPicker_SelectedIndexChanged(object sender, EventArgs e)
	{
		SettingsService.Settings.PrevAction = (ControlAction)PrevPicker.SelectedIndex;
	}

	private void PlayPicker_SelectedIndexChanged(object sender, EventArgs e)
	{
		SettingsService.Settings.PlayPauseAction = (ControlAction)PlayPicker.SelectedIndex;
	}

	private void NextPicker_SelectedIndexChanged(object sender, EventArgs e)
	{
		SettingsService.Settings.NextAction = (ControlAction)NextPicker.SelectedIndex;
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