using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Java.IO;
using SQLite;
using File = System.IO.File;
using IOException = System.IO.IOException;

namespace VocableTrainer
{
	public class LangDatabase
	{
		readonly SQLiteAsyncConnection _database;

		public LangDatabase(string dbPath)
		{

			_database = new SQLiteAsyncConnection(dbPath);
			_database.CreateTableAsync<Vocable>().Wait();
			_database.CreateTableAsync<Language>().Wait();
			_database.CreateTableAsync<Sound>().Wait();
			_database.CreateTableAsync<Training>().Wait();
		}

		

		public List<Sound> GetSounds()
		{
			return App.GetResult(_database.Table<Sound>().ToListAsync());
		}

		public List<Language> GetLanguages()
		{
			return App.GetResult(_database.Table<Language>().ToListAsync());
		}

		public IEnumerable<Vocable> GetVocables(int LangId)
		{
			return App.GetResult(_database.Table<Vocable>().ToListAsync()).Where(item =>item.LangId == LangId);
		}

		public Training GetTraining(int LangId)
		{
			return App.GetResult(_database.Table<Training>()
				.Where(i => i.LangId == LangId)
				.FirstOrDefaultAsync());
		}

		public Vocable GetVocable(int id)
		{
			return App.GetResult(_database.Table<Vocable>()
				.Where(i => i.Id == id)
				.FirstOrDefaultAsync());
		}

		public Sound GetSound(int Vocableid, Sound.Lang type)
		{
			return App.GetResult(_database.Table<Sound>()
				.Where(i => i.VocableId == Vocableid && i.Type == type)
				.FirstOrDefaultAsync());
		}

		public int SaveSound(Sound item)
		{
			if (item.Id != 0)
			{
				return App.GetResult(_database.UpdateAsync(item));
			}
			return App.GetResult(_database.InsertAsync(item));

		}

		public int SaveLanguage(Language item)
		{
			if (item.Id != 0)
			{
				return App.GetResult(_database.UpdateAsync(item));
			}
			return App.GetResult(_database.InsertAsync(item));
		}

		public int SaveTraining(Training item)
		{
			if (item.Id != 0)
			{
				return App.GetResult(_database.UpdateAsync(item));
			}
			return App.GetResult(_database.InsertAsync(item));
		}

		public int SaveVocable(Vocable item)
		{
			if (item.Id != 0)
			{
				return App.GetResult(_database.UpdateAsync(item));
			}

			return App.GetResult(_database.InsertAsync(item));

		}
		public int DeleteLanguage(Sound item)
		{
			return App.GetResult(_database.DeleteAsync(item));
		}

		public int DeleteLanguage(Language item)
		{
			return App.GetResult(_database.DeleteAsync(item));
		}

		public int DeleteVocable(Vocable item)
		{
			return App.GetResult(_database.DeleteAsync(item));
		}

		public void DeleteSound(Vocable item, Sound.Lang type)
		{
			var sound = App.GetResult(_database.Table<Sound>()
						.Where(i => i.VocableId == item.Id && i.Type == type)
						.FirstOrDefaultAsync());
			if (sound != null)
			{
				_database.DeleteAsync(sound);
			}
		}
	}
}
