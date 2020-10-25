using System;
using System.ComponentModel;
using SQLite;

namespace VocableTrainer
{
	public class Vocable: INotifyPropertyChanged
	{
		private int _Id = 0;
		private string _Foreign = String.Empty;
		private string _Detail = String.Empty;
		private string _Native = String.Empty;	
		private int _LangId = 0;
		private int _flag = 0;

		public event PropertyChangedEventHandler PropertyChanged = delegate { };
		public Vocable()
		{
			
		}

		[PrimaryKey, AutoIncrement]
		public int Id
		{
			get => _Id;

			set
			{
				_Id = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(Id)));
			}
		}

		public int LangId
		{
			get => _LangId;

			set
			{
				_LangId = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(LangId)));
			}

		}

		public int Flag
		{
			get => _flag;

			set
			{
				_flag = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(Flag)));
			}
		}

		public string Foreign
		{
			get => _Foreign;

			set
			{
				_Foreign = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(Foreign)));
			}
		}
		public string Detail
		{
			get => _Detail;

			set
			{
				_Detail = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(Detail)));
			}
		}
		public string Native
		{
			get => _Native;

			set
			{
				_Native = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(Native)));
			}
		}
	}
}
