using Microsoft.Extensions.Configuration;

namespace VocableTrainer
{
	public class SettingsService
	{
		public static void Load(IConfiguration configuration)
		{
			Settings = configuration.GetRequiredSection("Settings").Get<Settings>();
		}

		public static Settings Settings { get; private set; }
	}
}
