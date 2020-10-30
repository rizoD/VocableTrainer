using Android.Media;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dalvik.Annotation;
using Newtonsoft.Json;
using Xamarin.Forms;
using Encoding = System.Text.Encoding;

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
			var taks = Task.Factory.StartNew(() =>
			{
				bool finished = false;
				MediaPlayer currentPlayer = new MediaPlayer();
				currentPlayer.Prepared += (sender, e) =>
				{
					currentPlayer.Start();
				};
				currentPlayer.Completion += (sender, args) =>
				{
					args.ToString();
					finished = true;
				};
				currentPlayer.Stop();
				currentPlayer.SetDataSource(new StreamMediaDataSource(new MemoryStream(sound.Data)));
				currentPlayer.PrepareAsync();
				while (!finished)
				{
					Thread.Sleep(100);
				}
			});

			taks.Wait(TimeSpan.FromSeconds(5));
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
