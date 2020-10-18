using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
namespace VocableTrainer
{
	public partial class App : Application, INotifyPropertyChanged
	{
		public static ViewModel Data = new ViewModel();

		public App()
		{
			InitializeComponent();
			MainPage = new TrainingPage(); // new MainPage();
		}

		protected override void OnStart()
		{
		}

		protected override void OnSleep()
		{
		}

		protected override void OnResume()
		{
		}
	}
}
