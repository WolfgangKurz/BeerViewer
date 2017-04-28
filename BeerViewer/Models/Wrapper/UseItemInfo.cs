using BeerViewer.Models.Raw;
using BeerViewer.Models.kcsapi.mst;

namespace BeerViewer.Models.Wrapper
{
	public class UseItemInfo : RawDataWrapper<kcsapi_mst_useitem>, IIdentifiable
	{
		public int Id => this.RawData.api_id;
		public string Name => this.RawData.api_name;

		internal UseItemInfo(kcsapi_mst_useitem Data) : base(Data) { }

		public override string ToString()
			=> $"ID = {this.Id}, Name = \"{this.Name}\"";
	}
}
