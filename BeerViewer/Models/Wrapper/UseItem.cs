using BeerViewer.Models.Raw;
using BeerViewer.Models.kcsapi;

namespace BeerViewer.Models.Wrapper
{
	public class UseItem : SvData<kcsapi_useitem>, IIdentifiable
	{
		public int Id => this.api_data.api_id;
		public string Name => this.api_data.api_name;
		public int Count => this.api_data.api_count;

		internal UseItem(kcsapi_useitem rawData) : base(rawData) { }

		public override string ToString()
			=> $"ID = {this.Id}, Name = \"{this.Name}\", Count = {this.Count}";
	}
}
