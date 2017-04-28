using BeerViewer.Models.Raw;
using BeerViewer.Models.kcsapi.mst;

namespace BeerViewer.Models.Wrapper
{
	public class UseItemInfo : SvData<kcsapi_mst_useitem>, IIdentifiable
	{
		public int Id => this.api_data.api_id;
		public string Name => this.api_data.api_name;

		internal UseItemInfo(kcsapi_mst_useitem rawData) : base(rawData) { }

		public override string ToString()
			=> $"ID = {this.Id}, Name = \"{this.Name}\"";
	}
}
