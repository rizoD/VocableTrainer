using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DBCreateer
{
	internal class App
	{
		public static T GetResult<T>(Task<T> task)
		{
			task.Wait(TimeSpan.FromSeconds(5));
			return task.Result;
		}

	}
}
