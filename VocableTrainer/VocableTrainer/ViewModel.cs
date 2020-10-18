using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace VocableTrainer
{
	public class ViewModel : INotifyPropertyChanged
	{
		private Vocable _CurrentVocable;
		private Vocable _CurrentTraining;
		private ObservableCollection<Vocable> _Vocables;
		private ObservableCollection<Language> _Languages;
		private ObservableCollection<Vocable> _Trainings;
		public LangDatabase Database;
		public event PropertyChangedEventHandler PropertyChanged = delegate { };

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
			Database = new LangDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VocableTraining.db3"));
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
			Database.SaveLanguageAsync(item).Result.ToString();
			LoadLang();
			if (isNew)
			{
				Settings.CurrentLanguage = Languages.LastOrDefault().Id;
			}

		}

		public void SaveVocable(Vocable item)
		{
			Database.SaveVocableAsync(item).Result.ToString();
			LoadVocables();
		}
	}
}
