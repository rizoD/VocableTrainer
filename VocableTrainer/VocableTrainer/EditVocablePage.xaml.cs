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
			Native.Text = App.Data.CurrentVocable.Native;
			Detail.Text = App.Data.CurrentVocable.Detail;
			Foreign.Text = App.Data.CurrentVocable.Foreign;
			Foreign.Focus();
		}

		private void CancelBtn_OnClicked(object sender, EventArgs e)
		{
			Navigation.PopModalAsync();
		}

		private void OKBtn_OnClicked(object sender, EventArgs e)
		{
			App.Data.CheckSounds(Native.Text, Sound.Lang.Native);
			App.Data.CheckSounds(Foreign.Text, Sound.Lang.Foreign);

			App.Data.CurrentVocable.Flag = Flags.None;
			App.Data.CurrentVocable.Native = Native.Text;
			App.Data.CurrentVocable.Detail = Detail.Text;
			App.Data.CurrentVocable.Foreign = Foreign.Text;
			App.Data.CurrentVocable.LangId = App.Data.CurrentLanguage.Id;
			App.Data.SaveVocable(App.Data.CurrentVocable);
			Navigation.PopModalAsync();
		}

		async void DelBtn_OnClicked(object sender, EventArgs e)
		{
			bool answer = await DisplayAlert("Question?", "Would you like to delete this vocable?", "Yes", "No");
			if (answer)
			{
				App.Data.DeleteVocable(App.Data.CurrentVocable);
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
	}
}