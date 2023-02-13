using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VocableTrainer
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TrainingPageDetail : ContentPage
	{
		public TrainingPageDetail()
		{
			InitializeComponent();
			if (App.Data.CurrentVocable == null)
			{
				App.Data.CurrentVocable = App.Data.Vocables.LastOrDefault();
			}
		}

		private void Pause_OnClicked(object sender, EventArgs e)
		{
			App.Controls.PlayPause(false);
		}

		private void Next_OnClicked(object sender, EventArgs e)
		{
			App.Controls.Next(false);
		}

		private void Foreign_OnClicked(object sender, EventArgs e)
		{
			Task.Run(() =>
			{
				App.PlaySound(Sound.Lang.Foreign);
			});
		}

		private void Native_OnClicked(object sender, EventArgs e)
		{
			Task.Run(() =>
			{
				App.PlaySound(Sound.Lang.Native);
			});
		}

		private void Edit_OnClicked(object sender, EventArgs e)
		{
			//App.TogglePlay();
			App.Pause();
			Navigation.PushModalAsync(new EditVocablePage());
		}

		private async void Restar_OnClicked(object sender, EventArgs e)
		{
			bool answer = await DisplayAlert("Question?", "Do you realy want to restart?", "Yes", "No");
			if (answer)
			{
				App.Restart();
			}
		}

		private void Resume_OnClicked(object sender, EventArgs e)
		{
			App.Play();
		}

		private void Prev_OnClicked(object sender, EventArgs e)
		{
			App.Controls.Prev(false);
		}

		private void SwipedLeft(object sender, SwipedEventArgs e)
		{
			App.Pause();
		}

		private void SwipedUp(object sender, SwipedEventArgs e)
		{
			App.Controls.Next(false);
		}

		private void SwipedDown(object sender, SwipedEventArgs e)
		{
			App.Controls.Prev(false);
		}
	}
}