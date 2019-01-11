using BeerViewer.Models.Raw;
using BeerViewer.Models.kcsapi;

namespace BeerViewer.Models.Wrapper
{
	public class UseItem : RawDataWrapper<kcsapi_useitem>, IIdentifiable
	{
		public int Id => this.RawData.api_id;
		public string Name => this.RawData.api_name;
		public int Count => this.RawData.api_count;

		internal UseItem(kcsapi_useitem Data) : base(Data) { }

		public override string ToString()
			=> $"ID = {this.Id}, Name = \"{this.Name}\", Count = {this.Count}";
	}
}
