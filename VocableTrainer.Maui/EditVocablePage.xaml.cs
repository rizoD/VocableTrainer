using VocableTrainer.Data;

namespace VocableTrainer.Maui;

public partial class EditVocablePage : ContentPage
{
	public ViewModel Data { get; }

	public EditVocablePage()
	{
		Data = App.Data;
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
			App.ShowLoading(action, () => App.ShowToast("Sound updated"));
		}
		else
		{
			action();
			App.ShowToast("Sound updated");
		}
	}

	private void PlayForeign_OnClicked(object sender, EventArgs e)
	{
		App.ShowLoading(() => App.PlaySound(Sound.Lang.Foreign), () => App.ShowToast("Foreign played"));
	}

	private void PlayNative_OnClicked(object sender, EventArgs e)
	{
		App.ShowLoading(() => App.PlaySound(Sound.Lang.Native), () => App.ShowToast("Native played"));

	}

	private void FlagBtn_OnClicked(object sender, EventArgs e)
	{
		App.Data.CurrentVocable.Flag = Flags.None;
	}

	private void RecallResetBtn_OnClicked(object sender, EventArgs e)
	{
		App.Data.CurrentVocable.RecallScore = 0;
	}

	private void InputView_OnTextChanged(object sender, TextChangedEventArgs e)
	{
		//lets the Entry be empty
		if (string.IsNullOrEmpty(e.NewTextValue)) return;

		if (!double.TryParse(e.NewTextValue, out double value))
		{
			((Entry)sender).Text = e.OldTextValue;
		}
	}
}