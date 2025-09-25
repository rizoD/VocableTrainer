using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VocableTrainer.Data;

namespace VocableTrainer
{
	public class ViewModel : INotifyPropertyChanged
	{
		private IEnumerable<Vocable> vocables;

		private Training _Training;
		private Vocable _CurrentVocable;
		private Language _CurrentLanguage;
		private Language _EditLanguage;


		private ObservableCollection<Vocable> _Vocables = new ObservableCollection<Vocable>();
		private ObservableCollection<Language> _Languages = new ObservableCollection<Language>();
		private ObservableCollection<Vocable> _Trainings = new ObservableCollection<Vocable>();
		private string _SearchText = string.Empty;

		private bool isBusy = false;

		private PlayState _State = PlayState.Finished;

		public readonly static string DBFile = "VocableTraining.db3";
		public readonly string DBPath = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
			DBFile);

		public LangDatabase Database;
		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		public List<Sound.Lang> TrainingSound { get; set; }
		public Sound.Lang LastTrainingSound { get; set; }

		public bool NotPlaying
		{
			get => _State != PlayState.Playing;
		}

		public bool Playing
		{
			get => _State == PlayState.Playing;
		}

		public bool Paused
		{
			get => _State == PlayState.Pause;
		}


		public PlayState State
		{
			get => _State;

			set
			{
				_State = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(State)));
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(Paused)));
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

		public bool IsBusy
		{
			get => isBusy;

			set
			{
				isBusy = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsBusy)));
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
				if (_CurrentLanguage != value)
				{
					_CurrentLanguage = value;
					PropertyChanged(this, new PropertyChangedEventArgs(nameof(CurrentLanguage)));
					if (value != null)
					{
						App.UpdateLang();
					}
				}
			}
		}


		public Language EditLanguage
		{
			get => _EditLanguage;

			set
			{
				_EditLanguage = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(EditLanguage)));
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
				if (_SearchText != value)
				{
					_SearchText = value;

					Task.Run(() => { ApplyFilter(vocables); });
					PropertyChanged(this, new PropertyChangedEventArgs(nameof(SearchText)));
				}
			}
		}

		public ViewModel()
		{
			TrainingSound = new List<Sound.Lang>();
		}

		public void LoadFile()
		{
			Database = new LangDatabase(DBPath);
			LoadLang();
		}

		public void ApplyFilter(IEnumerable<Vocable> vocables)
		{
			try
			{
				if (SearchText == "flagged")
				{
					Vocables = new ObservableCollection<Vocable>(vocables.Where(item => item.Flag > 0));
				}
				else
				{
					Vocables = new ObservableCollection<Vocable>(vocables.Where(item =>
						item.Native.Filter(SearchText) ||
						item.Detail.Filter(SearchText) ||
						item.Foreign.Filter(SearchText)));
				}
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
		}

		internal void LoadVocables(Vocable vocable = null)
		{
			Task.Run(() =>
			{
				vocables = Database.GetVocables(Settings.CurrentLanguage).OrderByDescending(item => item.Id);
				LoadTraining();
				ApplyFilter(vocables);
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
			});
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


		public Sound UpdateCurrentSound(Sound.Lang type)
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

			return sound;
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


		public void SaveVocable(Vocable item, bool reload)
		{
			Database.SaveVocable(item);
			if (reload)
			{
				LoadVocables(item);
			}
		}


		public void SaveSound(Sound item)
		{
			Database.SaveSound(item);
		}

		public void BlockShuffle(bool useRecall)
		{

			IOrderedEnumerable<Vocable> train = null;

			if (useRecall)
			{ 
				train = vocables.OrderBy(item => item.RecallScore).ThenByDescending(item => item.Id);
			}
			else
			{
				train = vocables.OrderByDescending(item => item.Id);
			}

			int i = 0;
			IEnumerable<Vocable> traininglist = new List<Vocable>();

			// takes the list splits it up in blocks of (size MostRecent) and shuffles it with the Orderby new Guid
			while (i < train.Count())
			{
				traininglist =
					traininglist.Concat(train.Skip(i).Take(CurrentTraining.MostRecent).OrderBy(item => Guid.NewGuid()));
				i += CurrentTraining.MostRecent;
			}

			Trainings = new ObservableCollection<Vocable>(traininglist);
			CurrentVocable = Trainings.FirstOrDefault();
			Settings.LastTraining = CurrentVocable.Id;

		}

		public void SaveTrainingState()
		{
			CurrentTraining.SaveSorting(Trainings);
			Database.SaveTraining(CurrentTraining);
		}

	}
}
