using Microsoft.Extensions.Configuration;

namespace VocableTrainer.Maui;

public partial class TrainingPage : ContentPage
{
	IConfiguration _configuration;
	public TrainingPage(IConfiguration config)
	{
		_configuration = config;
		InitializeComponent();
		MasterPage.ListView.ItemSelected += ListView_ItemSelected;
	}

	private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
	{
		var item = e.SelectedItem as TrainingPageMasterMenuItem;
		if (item == null)
			return;

		var page = (Page)Activator.CreateInstance(item.TargetType);
		page.Title = item.Title;

		Detail = new NavigationPage(page);
		IsPresented = false;

		MasterPage.ListView.SelectedItem = null;
	}
}