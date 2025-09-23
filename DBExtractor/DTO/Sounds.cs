
namespace DBExtractor.DTO
{
	internal class Sounds
	{
		public string ID { get; set; }

		public string ForeignId => $"{ID}_JP";
		public string NativeId => $"{ID}_DE";

		public byte[] Foreign { get; set; }
		public byte[] Native { get; set; }
	}
}
