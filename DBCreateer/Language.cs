using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using SQLite;

namespace VocableTrainer
{
	public class Language : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		public int _Id = 0;

		[PrimaryKey, AutoIncrement]
		public int Id
		{
			get => this._Id;
			set
			{
				_Id = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(Id)));
			}
		}
		
		public string _Language = String.Empty;

		public string Name
		{
			get => _Language;
			set
			{
				_Language = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(Name)));
			}
		}

		public string _Voice = String.Empty;

		public string Voice
		{
			get => _Voice;
			set
			{
				_Voice = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(Voice)));
			}
		}
	}
}
