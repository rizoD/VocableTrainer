using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VocableTrainer
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TrainingPageMaster : ContentPage
	{
		public ListView ListView;

		public TrainingPageMaster()
		{
			InitializeComponent();

			BindingContext = new TrainingPageMasterViewModel();
			ListView = MenuItemsListView;
		}

		class TrainingPageMasterViewModel : INotifyPropertyChanged
		{
			public ObservableCollection<TrainingPageMasterMenuItem> MenuItems { get; set; }


			public TrainingPageMasterViewModel()
			{
				MenuItems = new ObservableCollection<TrainingPageMasterMenuItem>(new[]
					{
					new TrainingPageMasterMenuItem { Id = 0, Title = "Training" , TargetType = typeof(TrainingPageDetail)},
					new TrainingPageMasterMenuItem { Id = 1, Title = "Vocables" , TargetType = typeof(VocablePageDetail)},
					new TrainingPageMasterMenuItem { Id = 2, Title = "Settings" , TargetType = typeof(SettingsPageDetail)},
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
}