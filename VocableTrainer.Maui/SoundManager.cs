using System.Net;
using Newtonsoft.Json;
using Encoding = System.Text.Encoding;
using VocableTrainer.Maui.sounds;
using VocableTrainer.Maui;

namespace VocableTrainer
{
	public class SoundManager
	{

		public static Sound CreateSound(Vocable vocable, Sound.Lang type)
		{
			string voice = SettingsService.Settings.NativeVoice;
			string text = vocable.Native;
			if (type == Sound.Lang.Foreign)
			{
				voice = App.Data.CurrentLanguage.Voice;
				text = vocable.Foreign;
			}

			var bits = DownloadSoundAsync(text, voice);
			bits.Wait();
			if (bits.Result != null)
			{
				return new Sound()
				{
					Data = bits.Result,
					Type = type,
					VocableId = vocable.Id
				};
			}
			return null;
		}

		public static void Play(Sound sound)
		{
			SoundService.PlayStream(new MemoryStream(sound.Data));
		}


		private const string URL = "https://ttsmp3.com/makemp3_new.php";
		private static HttpClient client = new HttpClient
		{
			Timeout = TimeSpan.FromSeconds(10),
		};

		static async Task<byte[]> DownloadSoundAsync(string text, string voice)
		{
			try
			{
				text = text.Replace("&", "and");
				var userid = string.Empty; // "&user=198529";
				string parameters = $"msg={text}&lang={voice}&source=ttsmp3{userid}";
				var content = new StringContent(parameters, Encoding.Default, "application/x-www-form-urlencoded");
				TTSState state = null;
				using (var httpResponse = client.PostAsync(URL, content))
				{
					httpResponse.Wait(TimeSpan.FromSeconds(10));
					if (httpResponse.Result != null &&
						httpResponse.Result.StatusCode == HttpStatusCode.OK)
					{
						var json = await httpResponse.Result.Content.ReadAsStringAsync();
						state = JsonConvert.DeserializeObject<TTSState>(json);
					}
				}
				if (state != null && state.Error == 0)
				{
					string url = $"https://ttsmp3.com/dlmp3.php?mp3={state.MP3}&location={state.tasktype}";
					using (var httpResponse = client.GetAsync(url))
					{
						httpResponse.Wait(TimeSpan.FromSeconds(10));
						if (httpResponse.Result != null &&
							httpResponse.Result.StatusCode == HttpStatusCode.OK)
						{
							return await httpResponse.Result.Content.ReadAsByteArrayAsync();
						}
					}
				}
			}
			catch (Exception)
			{
				//Handle Exception
			}
			return null;
		}

		/*
		public class StreamMediaDataSource : MediaDataSource
		{
			System.IO.Stream data;

			public StreamMediaDataSource(System.IO.Stream Data)
			{
				data = Data;
			}

			public override long Size
			{
				get
				{
					return data.Length;
				}
			}

			public override int ReadAt(long position, byte[] buffer, int offset, int size)
			{
				data.Seek(position, System.IO.SeekOrigin.Begin);
				return data.Read(buffer, offset, size);
			}

			public override void Close()
			{
				if (data != null)
				{
					data.Dispose();
					data = null;
				}
			}

			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);

				if (data != null)
				{
					data.Dispose();
					data = null;
				}
			}
		}
		*/

		public class TTSState
		{
			public int Error { get; set; } //0
			public string Speaker { get; set; } //"Joanna"
			public int Cached { get; set; } //1,
			public string Text { get; set; } //"Error"
			public string tasktype { get; set; } //"direct"
			public string URL { get; set; } //"https:\/\/ttsmp3.com\/created_mp3\/289c40e8303f8fccd9a2fe4fad1a55f6.mp3"
			public string MP3 { get; set; } //"289c40e8303f8fccd9a2fe4fad1a55f6.mp3"
		}
	}
}
