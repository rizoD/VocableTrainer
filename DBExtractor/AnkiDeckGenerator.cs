using AnkiNet;

namespace DBExtractor
{
	internal class AnkiDeckGenerator
	{
		public AnkiCollection Collection { get; private set; }
		public long DeckId { get; private set; }
		public long NoteTypeId { get; private set;}

		public AnkiDeckGenerator(string deckName)
		{
			var cardTypes = new[]
			{
				new AnkiCardType(
					"Japanisch/Deutsch",
					0,
					"{{Japanisch}}<br/>{{hint:Details}}{{Sound_Jp}}",
					"{{Japanisch}}<hr id=\"answer\">{{Deutsch}}{{Sound_De}}"
				),
				new AnkiCardType(
					"Deutsch/Japanisch",
					1,
					"{{Deutsch}}{{Sound_De}}",
					"{{Deutsch}}<hr id=\"answer\">{{Japanisch}}<br/>{{hint:Details}}{{Sound_Jp}}"
				)
			};

			var noteType = new AnkiNoteType("Japansich/Deutsch", cardTypes, new[] { "Japanisch", "Deutsch", "Details", "Sound_Jp", "Sound_De" });

			Collection = new AnkiCollection();
			NoteTypeId = Collection.CreateNoteType(noteType);
			DeckId = Collection.CreateDeck(deckName);
		}
		public void AddCard(string japanese, string hint, string german, string jpSound, string deSound)
		{
			Collection.CreateNote(DeckId, NoteTypeId, japanese, german, hint, $"[sound:{jpSound}.mp3]", $"[sound:{deSound}.mp3]");
		}	
	}
}
