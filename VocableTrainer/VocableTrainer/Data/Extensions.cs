using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace VocableTrainer.Data
{
	public static class Extensions
	{
		public static bool Filter(this string text, string search)
		{
			if (text != null && search != null)
			{
				return text.ToLower().Contains(search.ToLower());
			}

			return false;
		}
	}
}
