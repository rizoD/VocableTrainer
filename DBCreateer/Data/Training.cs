using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;

namespace VocableTrainer
{
	public class Training
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public int LangId { get; set; }
		public byte[] Sorting { get; set; }
		
		public Training()
		{
		}

		public void SaveSorting(IEnumerable<Vocable> list)
		{
			var lang = list.FirstOrDefault()?.LangId;
			if (lang != null)
			{
				LangId = lang.Value;
			}
			List<byte> bytes = new List<byte>();
			foreach (Vocable vocable in list)
			{
				bytes.Add((byte)(vocable.Id >> 8));
				bytes.Add((byte)vocable.Id);
			}
		}

		public IEnumerable<Vocable> ApplySorting(IEnumerable<Vocable> list)
		{
			if (list != null && Sorting != null)
			{
				int idx = 0;
				List<Tuple<int, int>> sortinglist = new List<Tuple<int, int>>();
				for (int i = 0; i < Sorting.Length/2; i++)
				{
					var id = Sorting[i * 2] << 8 | Sorting[i * 2+1];
					sortinglist.Add(new Tuple<int, int>(id, idx++));
				}
				return list.Where(item => item.LangId == LangId).OrderBy(item => sortinglist.FirstOrDefault(si => si.Item1 == item.Id)?.Item2);
			}
			return list;
		}
	}
}
