using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VocableTrainer
{
	public class ViewModel : INotifyPropertyChanged
	{
		private Vocable _CurrentVocable;
		private Vocable _CurrentTraining;
		private Language _CurrentLanguage;
		private ObservableCollection<Vocable> _Vocables;
		private ObservableCollection<Language> _Languages;
		private ObservableCollection<Vocable> _Trainings;
		private ObservableCollection<Tuple<string, string>> _dataStores;



		public const string DBFile = "VocableTraining.db3";

		public LangDatabase Database;
		public event PropertyChangedEventHandler PropertyChanged = delegate { };


		public ObservableCollection<Tuple<string, string>> DataStores
		{
			get => _dataStores;

			set
			{
				_dataStores = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(DataStores)));
			}
		}
		public ObservableCollection<Vocable> Vocables
		{
			get => _Vocables;

			set
			{
				_Vocables = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(Vocables)));
			}
		}
		public ObservableCollection<Language> Languages
		{
			get => _Languages;

			set
			{
				_Languages = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(Languages)));
			}
		}
		public ObservableCollection<Vocable> Trainings
		{
			get => _Trainings;

			set
			{
				_Trainings = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(Trainings)));
			}
		}

		public Language CurrentLanguage
		{
			get => _CurrentLanguage;

			set
			{
				_CurrentLanguage = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(CurrentLanguage)));
			}
		}

		public Vocable CurrentTraining
		{
			get => _CurrentTraining;

			set
			{
				_CurrentTraining = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(CurrentTraining)));
			}
		}

		public Vocable CurrentVocable
		{
			get => _CurrentVocable;

			set
			{
				_CurrentVocable = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(CurrentVocable)));
			}
		}

		public ViewModel()
		{
			DataStores = new ObservableCollection<Tuple<string, string>>()
			{
				new Tuple<string, string>("Download", Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath),
				new Tuple<string, string>("Documents", Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath),
				new Tuple<string, string>("App Internal", Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)),
			};
			LoadFile(Settings.FilePath);
		}

		public void LoadFile(string folder)
		{
			try
			{
				Database = new LangDatabase(Path.Combine(folder, DBFile));
			}
			catch (Exception ex)
			{
				LoadFile(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
			}
			LoadLang();
			LoadVocables();
		}

		private void LoadVocables()
		{
			Vocables = new ObservableCollection<Vocable>(Database.GetVocablesAsync().Result);
			Trainings = new ObservableCollection<Vocable>(Database.GetVocablesAsync().Result);

			Trainings = Vocables;
			CurrentVocable = Trainings.LastOrDefault();
			CurrentTraining = Trainings.LastOrDefault();
		}
		private void LoadLang()
		{
			Languages = new ObservableCollection<Language>(Database.GetLanguagesAsync().Result);

			Languages.Insert(0, new Language()
			{
				Id = 0,
				Name = "{New language}"
			});
			CurrentLanguage = Languages.FirstOrDefault(item => item.Id == Settings.CurrentLanguage);
		}

		public void DeleteLang(Language item)
		{
			Database.DeleteLanguageAsync(item);
			Languages.Remove(item);
		}

		public void DeleteVocable(Vocable item)
		{
			Database.DeleteVocableAsync(item);
			Vocables.Remove(item);
		}

		public void SaveLang(Language item)
		{
			bool isNew = (item.Id == 0);
			Database.SaveLanguageAsync(item);
			LoadLang();
			if (isNew)
			{
				Settings.CurrentLanguage = Languages.LastOrDefault().Id;
			}

		}

		public void SaveVocable(Vocable item)
		{
			Database.SaveVocableAsync(item);
			LoadVocables();
		}


		public void SaveSound(Sound item)
		{
			Database.SaveSoundAsync(item);
		}

		
	}
}
