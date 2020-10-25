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

		private void Prev_OnClicked(object sender, EventArgs e)
		{
			App.Prev();
		}


		private void Pause_OnClicked(object sender, EventArgs e)
		{
			App.Pause();
		}

		private void Next_OnClicked(object sender, EventArgs e)
		{
			App.Next();
		}

		private void Foreign_OnClicked(object sender, EventArgs e)
		{
			App.PlaySound(Sound.Lang.Foreign);
		}

		private void Native_OnClicked(object sender, EventArgs e)
		{
			App.PlaySound(Sound.Lang.Native);
		}

		private void Edit_OnClicked(object sender, EventArgs e)
		{
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

		private void Flag_OnClicked(object sender, EventArgs e)
		{
			App.TrainingFlag();
		}
	}
}