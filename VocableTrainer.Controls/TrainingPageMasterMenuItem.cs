using System;

namespace VocableTrainer
{
	public class TrainingPageMasterMenuItem
	{
		public TrainingPageMasterMenuItem()
		{
			TargetType = typeof(TrainingPageMasterMenuItem);
		}
		public int Id { get; set; }
		public string Title { get; set; }

		public Type TargetType { get; set; }
	}
}