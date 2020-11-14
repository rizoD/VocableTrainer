using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VocableTrainer
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class VocablePageDetail : ContentPage
	{
		public VocablePageDetail()
		{
			InitializeComponent();
			this.Appearing += OnAppearing;
		}

		private void OnAppearing(object sender, EventArgs e)
		{
			App.Data.CurrentVocable = null;
		}

		private void AddBtn_OnClicked(object sender, EventArgs e)
		{
			App.Data.CurrentVocable = new Vocable();
			Navigation.PushModalAsync(new EditVocablePage());
		}

		private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			Navigation.PushModalAsync(new EditVocablePage());

		}
	}
}