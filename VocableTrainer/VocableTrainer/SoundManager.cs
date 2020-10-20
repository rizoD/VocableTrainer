using Android.Media;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dalvik.Annotation;
using Newtonsoft.Json;

namespace VocableTrainer
{
	public class SoundManager
	{

		public static Sound CreateSound(Vocable vocable, Sound.Lang type)
		{
			string voice = Settings.NativeVoice;
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
			try
			{
				MediaPlayer currentPlayer = new MediaPlayer();
				currentPlayer.Prepared += (sender, e) =>
				{
					currentPlayer.Start();
				};
				currentPlayer.Stop();
				currentPlayer.SetDataSource(new StreamMediaDataSource(new MemoryStream(sound.Data)));
				currentPlayer.Prepare();
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
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
				var content = new StringContent($"msg={text}&lang={voice}&source=ttsmp3{userid}", System.Text.Encoding.Default, "application/json");
				content = new StringContent("");
				TTSState state = null;
				using (var httpResponse = await client.PostAsync(URL, content))
				{
					if (httpResponse.StatusCode == HttpStatusCode.OK)
					{
						var json = await httpResponse.Content.ReadAsStringAsync();
						state = JsonConvert.DeserializeObject<TTSState>(json);
					}
				}
				if (state != null && state.Error == 0)
				{
					using (var httpResponse = await client.GetAsync(state.URL))
					{
						if (httpResponse.StatusCode == HttpStatusCode.OK)
						{
							return await httpResponse.Content.ReadAsByteArrayAsync();
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
