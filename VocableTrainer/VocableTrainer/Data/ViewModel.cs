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
using Android.Icu.Text;
using Android.Widget;
using Newtonsoft.Json;
using VocableTrainer.Data;

namespace VocableTrainer
{
	public class ViewModel : INotifyPropertyChanged
	{
		private IEnumerable<Vocable> vocables;

		private Training _Training;
		private Vocable _CurrentVocable;
		private Language _CurrentLanguage;
		

		private ObservableCollection<Vocable> _Vocables;
		private ObservableCollection<Language> _Languages;
		private ObservableCollection<Vocable> _Trainings;
		private string _SearchText = string.Empty;

		private PlayState _State = PlayState.Finished;

		public readonly static string DBFile = "VocableTraining.db3";
		public readonly string DBPath = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
			DBFile);

		public LangDatabase Database;
		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		public bool NotPlaying
		{
			get => _State != PlayState.Playing;
		}

		public bool Playing
		{
			get => _State == PlayState.Playing;
		}

		public PlayState State
		{
			get => _State;

			set
			{
				_State = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(State)));
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(NotPlaying)));
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(Playing)));
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

		public Training CurrentTraining
		{
			get => _Training;

			set
			{
				_Training = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(CurrentTraining)));
			}
		}

		public Language CurrentLanguage
		{
			get => _CurrentLanguage;

			set
			{
				_CurrentLanguage = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(CurrentLanguage)));
				LoadVocables();
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

		public string SearchText
		{
			get => _SearchText;

			set
			{
				_SearchText = value;
				if (SearchText == "flagged")
				{
					Vocables = new ObservableCollection<Vocable>(vocables.Where(item => item.Flag > 0));
				}
				else
				{
					Vocables = new ObservableCollection<Vocable>(vocables.Where(item =>
						item.Native.ToLower().Contains(value.ToLower()) ||
						item.Detail.ToLower().Contains(value.ToLower()) ||
						item.Foreign.ToLower().Contains(value.ToLower())));
				}

				PropertyChanged(this, new PropertyChangedEventArgs(nameof(SearchText)));
			}
		}

		public ViewModel()
		{
			LoadFile();
		}

		public void LoadFile()
		{
			Database = new LangDatabase(DBPath);
			LoadLang();
			LoadVocables();
		}

		private void LoadVocables(Vocable vocable = null)
		{
			vocables = Database.GetVocables(Settings.CurrentLanguage);
			LoadTraining();
			Vocables = new ObservableCollection<Vocable>(vocables);
			Trainings = new ObservableCollection<Vocable>(CurrentTraining.ApplySorting(vocables));

			if (vocable != null)
			{
				CurrentVocable = vocables.LastOrDefault(item => item.Id == vocable.Id);
			}
			else
			{
				vocable = vocables.FirstOrDefault(item => item.Id == Settings.LastTraining);
				if (vocable != null)
				{
					CurrentVocable = vocable;
				}
				else
				{
					CurrentVocable = Vocables.LastOrDefault();
				}
			}
		}

		private void LoadTraining()
		{
			CurrentTraining = Database.GetTraining(Settings.CurrentLanguage);
			if (CurrentTraining == null)
			{
				CurrentTraining = new Training()
				{
					LangId = CurrentLanguage.Id
				};
			}
		}

		private void LoadLang()
		{
			Languages = new ObservableCollection<Language>(Database.GetLanguages());

			Languages.Insert(0, new Language()
			{
				Id = 0,
				Name = "{New language}"
			});
			CurrentLanguage = Languages.FirstOrDefault(item => item.Id == Settings.CurrentLanguage);
		}

		public void DeleteLang(Language item)
		{
			Database.DeleteLanguage(item);
			Languages.Remove(item);
		}

		public void DeleteVocable(Vocable item)
		{
			Database.DeleteVocable(item);
			Vocables.Remove(item);
		}

		public void SaveLang(Language item)
		{
			bool isNew = (item.Id == 0);
			Database.SaveLanguage(item);
			LoadLang();
			if (isNew)
			{
				Settings.CurrentLanguage = Languages.LastOrDefault().Id;
			}
		}


		public async void UpdateCurrentSound(Sound.Lang type)
		{
			Sound sound = Database.GetSound(CurrentVocable.Id, type);
			if (sound != null)
			{
				Database.DeleteSound(CurrentVocable, type);
			}

			sound = SoundManager.CreateSound(CurrentVocable, type);
			if (sound != null)
			{
				SaveSound(sound);
			}
		}

		public void CheckSounds(string text, Sound.Lang type)
		{
			switch (type)
			{
				case Sound.Lang.Native:
					if (CurrentVocable.Native != text)
					{
						Database.DeleteSound(CurrentVocable, type);
					}

					break;
				case Sound.Lang.Foreign:
					if (CurrentVocable.Foreign != text)
					{
						Database.DeleteSound(CurrentVocable, type);
					}
					break;
				default:
					break;
			}

		}

		public void SaveVocable(Vocable item)
		{
			Database.SaveVocable(item);
			LoadVocables(item);
		}


		public void SaveSound(Sound item)
		{
			Database.SaveSound(item);
		}


		public void ShuffleTraining()
		{
			var train = vocables.OrderByDescending(item => item.Id);
			var first = train.Take(CurrentTraining.MostRecent).OrderBy(item => Guid.NewGuid());
			var last = train.Skip(CurrentTraining.MostRecent).OrderBy(item => Guid.NewGuid());
			Trainings = new ObservableCollection<Vocable>(first.Concat(last));
		}

		public void SaveTrainingState()
		{
			CurrentTraining.SaveSorting(Trainings);
			Database.SaveTraining(CurrentTraining);
		}

	}
}
