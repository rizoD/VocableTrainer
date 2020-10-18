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

		public Task<int> SaveLanguageAsync(Language lang)
		{
			if (lang.Id != 0)
			{
				return _database.UpdateAsync(lang);
			}
			else
			{
				return _database.InsertAsync(lang);
			}
		}

		public Task<int> SaveVocableAsync(Vocable vocable)
		{
			if (vocable.Id != 0)
			{
				return _database.UpdateAsync(vocable);
			}
			else
			{
				return _database.InsertAsync(vocable);
			}
		}
		public Task<int> DeleteLanguageAsync(Language lang)
		{
			return _database.DeleteAsync(lang);
		}

		public Task<int> DeleteVocableAsync(Vocable vocable)
		{
			return _database.DeleteAsync(vocable);
		}
	}
}
