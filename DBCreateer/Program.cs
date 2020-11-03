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
			var json_v = File.ReadAllText(arg.FirstOrDefault());
			arg.Remove(arg.First());
			var json_s = File.ReadAllText(arg.FirstOrDefault());
			arg.Remove(arg.First());
			CreateLanguage(json_v, "Jap vocables");
			CreateLanguage(json_s, "Jap sentences");
		}

		private static void CreateLanguage(string json, string langName)
		{
			var imports = JsonConvert.DeserializeObject<List<JsonImport>>(json);

			Language lang = db.GetLanguages().FirstOrDefault(item => item.Name.Equals(langName));

			if (lang == null)
			{
				lang = new Language()
				{
					Name = langName,
					Voice = "Mizuki"
				};
				db.SaveLanguage(lang);

				lang = db.GetLanguages().FirstOrDefault(item => item.Name.Equals(langName));
			}

			foreach (var import in imports)
			{
				var voc = new Vocable()
				{
					Detail = import.hint,
					Native = import.front,
					Foreign = import.back,
					LangId = lang.Id
				};
				db.SaveVocable(voc);
			}

			var vocables = db.GetVocables(lang.Id);
			foreach (var vocable in vocables)
			{
				var id = imports.FirstOrDefault(item => item.front == vocable.Native
				                                        && item.back == vocable.Foreign
				                                        && item.hint == vocable.Detail);
				if (id != null)
				{
					SaveSound(vocable.Id, Sound.Lang.Foreign, id.ID, langName);
					SaveSound(vocable.Id, Sound.Lang.Native, id.ID, langName);
				}
			}
		}

		private static void SaveSound(int vocableId, Sound.Lang type, string num, string langName)
		{
			string lang = "de";
			if (type == Sound.Lang.Foreign)
			{
				lang = "jap";
			}

			string file = @$"C:\Projects\Privat\WebApps\htmlaudiovocabletrainer\mp3\{langName}\{lang}_{num}.mp3";
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
