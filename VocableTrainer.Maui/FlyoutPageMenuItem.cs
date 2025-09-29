using System;

namespace VocableTrainer
{
	public class FlyoutPageMenuItem
	{
		public FlyoutPageMenuItem()
		{
			TargetType = typeof(FlyoutPageMenuItem);
		}
		public int Id { get; set; }
		public string Title { get; set; }

		public Type TargetType { get; set; }
	}
}