using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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