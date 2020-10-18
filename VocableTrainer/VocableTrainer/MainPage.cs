using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace VocableTrainer
{
	public class MainPage : ContentPage
	{
		public MainPage()
		{
			Content = new StackLayout
			{
				Children = {
					new Label { Text = "Welcome to Xamarin.Forms!" }
				}
			};
		}
	}
}