
namespace DBExtractor.DTO
{
	internal class Vocable
	{
		public string Id {  get; set; }
		public string Foreign {  get; set; }
		public string Detail {  get; set; }
		public string Native {  get; set; }

		public Sounds Sounds { get; set; } = null;
	}
}
