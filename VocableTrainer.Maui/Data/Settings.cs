namespace VocableTrainer
{
	public class Settings
	{
		public ControlAction PrevAction { get; set; }
		public ControlAction PlayPauseAction { get; set; }
		public ControlAction NextAction { get; set; }
		public bool AllwaysPlayChime { get; set; }
		public bool PlayRcChime { get; set; }
		public int LastTraining { get; set; }
		public int CurrentLanguage { get; set; }
		public string NativeVoice { get; set; }
	}
}
