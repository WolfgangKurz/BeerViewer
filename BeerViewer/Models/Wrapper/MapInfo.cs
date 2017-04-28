using BeerViewer.Models.Raw;
using BeerViewer.Models.kcsapi.mst;

namespace BeerViewer.Models.Wrapper
{
	public class MapInfo : SvData<kcsapi_mst_mapinfo>, IIdentifiable
	{
		public int Id => this.api_data.api_id;
		public string Name => this.api_data.api_name;

		public int MapAreaId => this.api_data.api_maparea_id;
		public MapArea MapArea { get; internal set; }
		public int IdInEachMapArea => this.api_data.api_no;

		public int Level => this.api_data.api_level;
		public string OperationName => this.api_data.api_opetext;
		public string OperationSummary => this.api_data.api_infotext;
		public int RequiredDefeatCount => this.api_data.api_required_defeat_count ?? 1;

		public MapInfo(kcsapi_mst_mapinfo mapinfo) : base(mapinfo) { }

		public override string ToString()
			=> $"ID = {this.Id}, Name = {this.Name}";
	}
}
