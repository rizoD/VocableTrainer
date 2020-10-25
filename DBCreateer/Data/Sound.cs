using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using SQLite;

namespace VocableTrainer
{
	public class Sound : INotifyPropertyChanged
	{
		public enum Lang
		{
			Foreign,
			Native
		}
	
		private int _Id = 0;
		private int _vocableId = 0;
		private byte[] _Data;
		private Lang _Type;

		public event PropertyChangedEventHandler PropertyChanged = delegate { };
		public Sound()
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

		public int VocableId
		{
			get => _vocableId;

			set
			{
				_vocableId = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(VocableId)));
			}
		}

		public Lang Type
		{
			get => _Type;

			set
			{
				_Type = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(Type)));
			}
		}

		public byte[] Data
		{
			get => _Data;

			set
			{
				_Data = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(Data)));
			}
		}


	}
}
