using System;
using System.Collections.Generic;
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
		}



		public List<Sound> GetSoundsAsync()
		{
			return App.GetResult(_database.Table<Sound>().ToListAsync());
		}

		public List<Language> GetLanguagesAsync()
		{
			return App.GetResult(_database.Table<Language>().ToListAsync());
		}

		public List<Vocable> GetVocablesAsync()
		{
			return App.GetResult(_database.Table<Vocable>().ToListAsync());
		}

		public Vocable GetVocableAsync(int id)
		{
			return App.GetResult(_database.Table<Vocable>()
				.Where(i => i.Id == id)
				.FirstOrDefaultAsync());
		}

		public Sound GetSoundAsync(int Vocableid, Sound.Lang type)
		{
			return App.GetResult(_database.Table<Sound>()
				.Where(i => i.VocableId == Vocableid && i.Type == type)
				.FirstOrDefaultAsync());
		}

		public int SaveSoundAsync(Sound item)
		{
			if (item.Id != 0)
			{
				return App.GetResult(_database.UpdateAsync(item));
			}
			return App.GetResult(_database.InsertAsync(item));

		}

		public int SaveLanguageAsync(Language item)
		{
			if (item.Id != 0)
			{
				return App.GetResult(_database.UpdateAsync(item));
			}
			return App.GetResult(_database.InsertAsync(item));
		}

		public int SaveVocableAsync(Vocable item)
		{
			if (item.Id != 0)
			{
				return App.GetResult(_database.UpdateAsync(item));
			}

			return App.GetResult(_database.InsertAsync(item));

		}
		public int DeleteLanguageAsync(Sound item)
		{
			return App.GetResult(_database.DeleteAsync(item));
		}

		public int DeleteLanguageAsync(Language item)
		{
			return App.GetResult(_database.DeleteAsync(item));
		}

		public int DeleteVocableAsync(Vocable item)
		{
			return App.GetResult(_database.DeleteAsync(item));
		}

		public void DeleteSoundAsync(Vocable item, Sound.Lang type)
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
