using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VocableTrainer.Maui;

public partial class FlyoutMenuPage : ContentPage
{
	public ListView ListView;

	public FlyoutMenuPage()
	{
		InitializeComponent();

		BindingContext = new TrainingPageMasterViewModel();
		ListView = MenuItemsListView;
	}

	class TrainingPageMasterViewModel : INotifyPropertyChanged
	{
		public ObservableCollection<FlyoutPageMenuItem> MenuItems { get; set; }


		public TrainingPageMasterViewModel()
		{
			MenuItems = new ObservableCollection<FlyoutPageMenuItem>(new[]
				{
					new FlyoutPageMenuItem { Id = 0, Title = "Training" , TargetType = typeof(MainPage)},
					new FlyoutPageMenuItem { Id = 1, Title = "Vocables" , TargetType = typeof(VocablePageDetail)},
					new FlyoutPageMenuItem { Id = 2, Title = "Settings" , TargetType = typeof(SettingsPageDetail)},
					});
		}

		#region INotifyPropertyChanged Implementation
		public event PropertyChangedEventHandler PropertyChanged;
		void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			if (PropertyChanged == null)
				return;

			PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
}