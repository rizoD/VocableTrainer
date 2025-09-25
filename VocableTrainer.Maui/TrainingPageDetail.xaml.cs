namespace VocableTrainer.Maui;

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
}