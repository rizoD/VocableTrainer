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

		public int _pause = 4;
		public int _mostRecent = 30;

		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public int LangId { get; set; }

		public bool CanPlay
		{
			get => Sorting != null;
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

		public byte[] Sorting { get; set; }
		
		public Training()
		{
		}

		public void Finished()
		{
			Sorting = null;
			PropertyChanged(this, new PropertyChangedEventArgs(nameof(CanPlay)));
		}

		public void SaveSorting(IEnumerable<Vocable> list)
		{
			var lang = list.FirstOrDefault()?.LangId;
			if (lang != null)
			{
				LangId = lang.Value;
			}

			Sorting = GetBytes(list);
		}

		public IEnumerable<Vocable> ApplySorting(IEnumerable<Vocable> list)
		{
			if (list != null && Sorting != null)
			{
				int idx = 0;
				var sortinglist = GetList(Sorting).Select(item => new { ID = item, Idx = idx++ });
				return list
					.Where(item => item.LangId == LangId && sortinglist.Any(sitem => item.Id == sitem.ID)) // filter only element available during trainging start
					.OrderBy(item => sortinglist.FirstOrDefault(sitem => sitem.ID == item.Id)?.Idx); // order do reflect last training session
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
