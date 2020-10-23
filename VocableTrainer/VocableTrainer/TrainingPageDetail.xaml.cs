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


		private void Play_OnClicked(object sender, EventArgs e)
		{
			App.PlayPause();
		}

		private void Next_OnClicked(object sender, EventArgs e)
		{
			App.Next();
		}

		private void Foreign_OnClicked(object sender, EventArgs e)
		{
			App.Play(Sound.Lang.Foreign);
		}

		private void Native_OnClicked(object sender, EventArgs e)
		{
			App.Play(Sound.Lang.Native);
		}

		private void Edit_OnClicked(object sender, EventArgs e)
		{
			Navigation.PushModalAsync(new EditVocablePage());
		}
	}
}