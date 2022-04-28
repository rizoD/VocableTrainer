using System;
using Android.Widget;
using Java.IO;
using Java.Lang;
using VocableTrainer.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VocableTrainer
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditVocablePage : ContentPage
	{

		public EditVocablePage()
		{
			InitializeComponent();
			Foreign.Focus();
			this.Disappearing += OnDisappearing;
		}

		private void OnDisappearing(object sender, EventArgs e)
		{
			Save(false);
		}

		private void Save(bool userRequested)
		{
			if (App.Data.CurrentVocable != null &&
				!string.IsNullOrWhiteSpace(App.Data.CurrentVocable.Foreign) &&
				!string.IsNullOrWhiteSpace(App.Data.CurrentVocable.Native))
			{
				// check ID before save.. since after it will be updated if new
				bool isNew = App.Data.CurrentVocable.Id == 0;

				App.Data.CurrentVocable.LangId = App.Data.CurrentLanguage.Id;
				App.Data.SaveVocable(App.Data.CurrentVocable, true);

				// if we have a new vocable we update the sounds anyway
				if (userRequested || isNew)
				{
					UpdateAllSound(userRequested);
				}
			}
		}

		async void DelBtn_OnClicked(object sender, EventArgs e)
		{
			bool answer = await DisplayAlert("Question?", "Would you like to delete this vocable?", "Yes", "No");
			if (answer)
			{
				App.Data.DeleteVocable(App.Data.CurrentVocable);
				App.Data.CurrentVocable = null;
				Navigation.PopModalAsync();
			}
		}

		private void SoundBtn_OnClicked(object sender, EventArgs e)
		{
			Save(true);
		}

		private void UpdateAllSound(bool uiUpdate)
		{
			Action action = () =>
			{
				App.Data.UpdateCurrentSound(Sound.Lang.Native);
				App.Data.UpdateCurrentSound(Sound.Lang.Foreign);
			};

			if (uiUpdate)
			{
				App.ShowLoading(action, Toast.MakeText(Android.App.Application.Context, "Sound updated", ToastLength.Long).Show);
			}
			else
			{
				action();
				Toast.MakeText(Android.App.Application.Context, "Sound updated", ToastLength.Long).Show();
			}
		}

		private void PlayForeign_OnClicked(object sender, EventArgs e)
		{
			App.ShowLoading(() => { App.PlaySound(Sound.Lang.Foreign); },
				Toast.MakeText(Android.App.Application.Context, "Foreign played", ToastLength.Long).Show);

		}

		private void PlayNative_OnClicked(object sender, EventArgs e)
		{
			App.ShowLoading(() => { App.PlaySound(Sound.Lang.Native); },
				Toast.MakeText(Android.App.Application.Context, "Native played", ToastLength.Long).Show);

		}

		private void FlagBtn_OnClicked(object sender, EventArgs e)
		{
			App.Data.CurrentVocable.Flag = Flags.None;
		}
	}
}