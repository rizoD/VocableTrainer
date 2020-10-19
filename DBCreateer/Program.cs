using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using VocableTrainer;

namespace DBCreateer
{
	class Program
	{
		private static LangDatabase db;

		static void Main(string[] args)
		{
			var arg = args.ToList();
			db = new LangDatabase(arg.FirstOrDefault());
			arg.Remove(arg.First());
			var json = File.ReadAllText(arg.FirstOrDefault());
			arg.Remove(arg.First());
			var imports = JsonConvert.DeserializeObject<List<JsonImport>>(json);
			
			Language lang = db.GetLanguages().FirstOrDefault();

			if (lang == null)
			{
				lang = new Language()
				{
					Name = "Japanisch",
					Voice = "Mizuki"
				};
				db.SaveLanguage(lang);

				lang = db.GetLanguages().FirstOrDefault();
			}

			foreach (var import in imports)
			{
				var voc = new Vocable()
				{
					Detail = import.hint,
					Native = import.front,
					Foreign = import.back,
					Lang = lang.Id
				};
				db.SaveVocable(voc);
			}

			var vocables = db.GetVocables();
			foreach (var vocable in vocables)
			{
				var id = imports.FirstOrDefault(item => item.front == vocable.Native
														&& item.back == vocable.Foreign
														&& item.hint == vocable.Detail);
				if (id != null)
				{
					SaveSound(vocable.Id, Sound.Lang.Foreign, id.ID);
					SaveSound(vocable.Id, Sound.Lang.Native, id.ID);
				}
			}
		}

		private static void SaveSound(int vocableId, Sound.Lang type, string num)
		{
			string lang = "de";
			if (type == Sound.Lang.Foreign)
			{
				lang = "jap";
			}

			string file = @$"C:\Projects\Privat\WebApps\htmlaudiovocabletrainer\mp3\{lang}_{num}.mp3";
			if (File.Exists(file))
			{
				var sound = new Sound()
				{
					Data = File.ReadAllBytes(file),
					Type = type,
					VocableId = vocableId
				};
				db.SaveSound(sound);
			}
		}
	}
}
