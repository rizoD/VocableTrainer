using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

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


		public List<Sound> GetSounds()
		{
			var task = _database.Table<Sound>().ToListAsync();
			task.Wait();
			return task.Result;
		}

		public List<Language> GetLanguages()
		{
			var task= _database.Table<Language>().ToListAsync();
			task.Wait();
			return task.Result;
		}

		public List<Vocable> GetVocables()
		{
			var task = _database.Table<Vocable>().ToListAsync();
			task.Wait();
			return task.Result;
		}

		public Vocable GetVocable(int id)
		{
			var task = _database.Table<Vocable>()
				.Where(i => i.Id == id)
				.FirstOrDefaultAsync();
			task.Wait();
			return task.Result;
		}

		public Sound GetSound(int Vocableid, Sound.Lang type)
		{
			var task = _database.Table<Sound>()
				.Where(i => i.VocableId == Vocableid && i.Type == type)
				.FirstOrDefaultAsync();
			task.Wait();
			return task.Result;
		}

		public int SaveSound(Sound item)
		{
			Task<int> task;
			if (item.Id != 0)
			{
				task = _database.UpdateAsync(item);
			}
			else
			{
				task = _database.InsertAsync(item);
			}
			task.Wait();
			return task.Result;
		}

		public int SaveLanguage(Language item)
		{
			Task<int> task;
			if (item.Id != 0)
			{
				task = _database.UpdateAsync(item);
			}
			else
			{
				task = _database.InsertAsync(item);
			}
			task.Wait();
			return task.Result;
		}

		public int SaveVocable(Vocable item)
		{
			Task<int> task;
			if (item.Id != 0)
			{
				task = _database.UpdateAsync(item);
			}
			else
			{
				task = _database.InsertAsync(item);
			}
			task.Wait();
			return task.Result;
		}
		public int DeleteLanguage(Sound item)
		{
			var task = _database.DeleteAsync(item);
			task.Wait();
			return task.Result;
		}

		public int DeleteLanguage(Language item)
		{
			var task = _database.DeleteAsync(item);
			task.Wait();
			return task.Result;
		}

		public int DeleteVocable(Vocable item)
		{
			var task = _database.DeleteAsync(item);
			task.Wait();
			return task.Result;
		}
	}
}
