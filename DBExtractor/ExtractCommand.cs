using AnkiNet;
using DBExtractor.DTO;
using Microsoft.Data.Sqlite;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Data.Common;
using System.IO.Compression;

namespace DBExtractor
{
	public class ExtractCommand : Command<ExtractCommand.Settings>
	{
		public class Settings : CommandSettings
		{
			internal IAnsiConsole Console { get; }

			[CommandArgument(0, "<dbFile>")]
			[Description("The database file to extract data from.")]
			public string DBFile { get; set; }

			[CommandArgument(1, "<outputFile>")]
			[Description("The ankni apkg file where the extracted data is outputted.")]
			public string? OutputFile { get; set; }


			public Settings(IAnsiConsole console)
			{
				Console = console;
			}
			public override ValidationResult Validate()
			{
				if (!File.Exists(DBFile))
				{
					return ValidationResult.Error($"DB File: '{DBFile}' could not be found");
				}

				if (!Directory.Exists(Path.GetDirectoryName(OutputFile)))
				{
					return ValidationResult.Error($"Path to OutputFile: '{Path.GetDirectoryName(OutputFile)}' could not be found");
				}
				WriteToConsole();
				return base.Validate();
			}
			private void WriteToConsole()
			{
				var table = new Table();
				table.Border = TableBorder.SimpleHeavy;
				table.AddColumn("Option").AddColumn("Value");
				table.AddRow("DB file", DBFile);
				table.AddRow("APKG file", OutputFile);

				Console.Write(table);
			}
		}

		public override int Execute(CommandContext context, Settings settings)
		{
			settings.Console.WriteLine("Loading DB file");

			using var connection = new SqliteConnection($"Data Source={settings.DBFile}");
			connection.Open();

			AnkiDeckGenerator ankiDeckGenerator = new AnkiDeckGenerator("Japanisch");

			var vocableCount = GetVocableCount(connection);

			settings.Console.Progress().Start(ctx =>
			{
				var task = ctx.AddTask("Writing cards", true, vocableCount);
				
				foreach (var vocable in GetVocables(connection))
				{
					ankiDeckGenerator.AddCard(vocable.Foreign, vocable.Native, vocable.Detail, vocable.Sounds.ForeignId, vocable.Sounds.NativeId);
					task.Increment(1);
				}
			});

			settings.Console.WriteLine("Writing APKG file.");

			var test = AnkiFileWriter.WriteToFileAsync(settings.OutputFile, ankiDeckGenerator.Collection);
			test.Wait();

			using (var zipFile = ZipFile.Open(settings.OutputFile, ZipArchiveMode.Update))
			{
				settings.Console.WriteLine("Adding media files to APKG.");

				settings.Console.Progress().Start(ctx =>
				{
					var task = ctx.AddTask("Writing media files", true, vocableCount);

					foreach (var vocable in GetVocables(connection))
					{
						ankiDeckGenerator.AddCard(vocable.Foreign, vocable.Native, vocable.Detail, vocable.Sounds.ForeignId, vocable.Sounds.NativeId);
						task.Increment(1);
					}
				});
			}
			settings.Console.WriteLine("Finished.");

			return 0;
		}


		private long GetVocableCount(DbConnection connection)
		{
			using var command = connection.CreateCommand();
			command.CommandText = "select count(*) from Vocable where LangId = 1";
			return (long)command.ExecuteScalar();
		}
		private IEnumerable<Vocable> GetVocables(DbConnection connection)
		{
			using var command = connection.CreateCommand();
			command.CommandText = "select Id, [Foreign], Detail, Native from Vocable where LangId = 1";

			using var reader = command.ExecuteReader();

			while (reader.Read())
			{
				var vocableId = reader.GetString(0);
				yield return new Vocable()
				{
					Id = vocableId,
					Foreign = reader.GetString(1),
					Detail = reader.GetString(2),
					Native = reader.GetString(3),
					Sounds = GetSounds(connection, vocableId)
				};
			}
		}

		private Sounds GetSounds(DbConnection connection, string vocableId)
		{
			return new Sounds()
			{
				ID = vocableId,
				Foreign = GetSound(connection, vocableId, "0"),
				Native = GetSound(connection, vocableId, "1"),
			};
		}

		private byte[] GetSound(DbConnection connection, string vocableId, string type)
		{
			using var command = connection.CreateCommand();
			command.CommandText = $"select Data from Sound where VocableId = {vocableId} and Type = {type}";
			return (byte[])command.ExecuteScalar();
		}


	}
}
