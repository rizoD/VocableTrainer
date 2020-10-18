using System;
using System.Linq;
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
			
			App.Data.CurrentVocable.Native = Native.Text;
			App.Data.CurrentVocable.Detail = Detail.Text;
			App.Data.CurrentVocable.Foreign = Foreign.Text;
			App.Data.CurrentVocable.Lang = Settings.CurrentLanguage;
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
	}
}