using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Android;
using SQLite;
using Xamarin.Forms.Internals;

namespace VocableTrainer
{
	public class Training : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = delegate { };
		public bool _autoPlay = false;
		public bool _PlayAnswer = false;
		public bool _oneHandMode = false;

		public int _pause = 4;
		public int _mostRecent = 30;

		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public int LangId { get; set; }

		public bool NotOneHandMode
		{
			get => !_oneHandMode;
			set
			{
				_oneHandMode = !value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(NotOneHandMode)));
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(OneHandMode)));

			}
		}
		public bool OneHandMode
		{
			get => _oneHandMode;
			set
			{
				_oneHandMode = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(NotOneHandMode)));
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(OneHandMode)));
			}
		}
		public int MostRecent
		{
			get => _mostRecent;
			set
			{
				_mostRecent = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(MostRecent)));
			}
		}
		public int Pause
		{
			get => _pause;
			set
			{
				_pause = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(Pause)));
			}
		}
		public bool AutoPlay
		{
			get => _autoPlay;
			set
			{
				_autoPlay = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(AutoPlay)));
			}
		}

		public bool PlayAnswer
		{
			get => _PlayAnswer;
			set
			{
				_PlayAnswer = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(PlayAnswer)));
			}
		}

		/// <summary>
		/// Stores the sorting for next training (if not reset)
		/// </summary>
		public byte[] Sorting { get; set; }


		/// <summary>
		/// Saves the previously sorted list to the training so it can be reused after a restart
		/// </summary>
		/// <param name="list"></param>
		public void SaveSorting(IEnumerable<Vocable> list)
		{
			var lang = list.FirstOrDefault()?.LangId;
			if (lang != null)
			{
				LangId = lang.Value;
			}

			Sorting = GetBytes(list);
		}

		/// <summary>
		/// Applies the sorting to the vocalbe list tat was previously saved
		/// so that we have the same sorting as in the last run
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		public IEnumerable<Vocable> ApplySorting(IEnumerable<Vocable> list)
		{
			if (list != null && Sorting != null)
			{
				int idx = 0;
				// order to reflect last training session
				var sortedList = GetList(Sorting).Select(vocID => list.FirstOrDefault(item => item.Id == vocID));
				return sortedList.Where(item => item != null); // filter only element available during training start
			}
			return list;
		}

		private byte[] GetBytes(IEnumerable<Vocable> list)
		{
			List<byte> bytes = new List<byte>();
			foreach (Vocable vocable in list)
			{
				bytes.Add((byte)(vocable.Id >> 8));
				bytes.Add((byte)vocable.Id);
			}

			return bytes.ToArray();
		}

		private IEnumerable<int> GetList(byte[] bytes)
		{
			for (int i = 0; i < bytes.Length / 2; i++)
			{
				yield return bytes[i * 2] << 8 | bytes[i * 2 + 1];
			}
		}
	}
}
