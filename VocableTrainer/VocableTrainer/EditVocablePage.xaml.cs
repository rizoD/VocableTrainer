using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Android.Widget;
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
			if (App.Data.CurrentVocable != null &&
			    !string.IsNullOrWhiteSpace(App.Data.CurrentVocable.Foreign) &&
			    !string.IsNullOrWhiteSpace(App.Data.CurrentVocable.Native))
			{
				App.Data.CurrentVocable.LangId = App.Data.CurrentLanguage.Id;
				App.Data.SaveVocable(App.Data.CurrentVocable);
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
			App.ShowLoading(() =>
			{
				App.Data.UpdateCurrentSound(Sound.Lang.Native);
				App.Data.UpdateCurrentSound(Sound.Lang.Foreign);
			}, Toast.MakeText(Android.App.Application.Context, "Sound updated", ToastLength.Long).Show);
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