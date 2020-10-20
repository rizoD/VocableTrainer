using System;
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


		public Task<List<Sound>> GetSoundsAsync()
		{
			return _database.Table<Sound>().ToListAsync();
		}

		public Task<List<Language>> GetLanguagesAsync()
		{
			return _database.Table<Language>().ToListAsync();
		}

		public Task<List<Vocable>> GetVocablesAsync()
		{
			return _database.Table<Vocable>().ToListAsync();
		}

		public Task<Vocable> GetVocableAsync(int id)
		{
			return _database.Table<Vocable>()
				.Where(i => i.Id == id)
				.FirstOrDefaultAsync();
		}

		public Task<Sound> GetSoundAsync(int Vocableid, Sound.Lang type)
		{
			return _database.Table<Sound>()
				.Where(i => i.VocableId == Vocableid && i.Type == type)
				.FirstOrDefaultAsync();
		}

		public Task<int> SaveSoundAsync(Sound item)
		{
			if (item.Id != 0)
			{
				return _database.UpdateAsync(item);
			}
			else
			{
				return _database.InsertAsync(item);
			}
		}

		public Task<int> SaveLanguageAsync(Language item)
		{
			if (item.Id != 0)
			{
				return _database.UpdateAsync(item);
			}
			else
			{
				return _database.InsertAsync(item);
			}
		}

		public Task<int> SaveVocableAsync(Vocable item)
		{
			if (item.Id != 0)
			{
				return _database.UpdateAsync(item);
			}
			else
			{
				return _database.InsertAsync(item);
			}
		}
		public Task<int> DeleteLanguageAsync(Sound item)
		{
			return _database.DeleteAsync(item);
		}

		public Task<int> DeleteLanguageAsync(Language item)
		{
			return _database.DeleteAsync(item);
		}

		public Task<int> DeleteVocableAsync(Vocable item)
		{
			return _database.DeleteAsync(item);
		}

		public async void DeleteSoundAsync(Vocable item, Sound.Lang type)
		{
			var sound = _database.Table<Sound>()
						.Where(i => i.VocableId == item.Id && i.Type == type)
						.FirstOrDefaultAsync();
			sound.Wait();
			if (sound.Result != null)
			{
				_database.DeleteAsync(sound.Result);
			}
		}
	}
}
