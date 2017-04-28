using BeerViewer.Models.Raw;
using BeerViewer.Models.kcsapi.mst;

namespace BeerViewer.Models.Wrapper
{
	public class MapInfo : RawDataWrapper<kcsapi_mst_mapinfo>, IIdentifiable
	{
		public int Id => this.RawData.api_id;
		public string Name => this.RawData.api_name;

		public int MapAreaId => this.RawData.api_maparea_id;
		public MapArea MapArea { get; internal set; }
		public int IdInEachMapArea => this.RawData.api_no;

		public int Level => this.RawData.api_level;
		public string OperationName => this.RawData.api_opetext;
		public string OperationSummary => this.RawData.api_infotext;
		public int RequiredDefeatCount => this.RawData.api_required_defeat_count ?? 1;

		public MapInfo(kcsapi_mst_mapinfo mapinfo) : base(mapinfo) { }

		public override string ToString()
			=> $"ID = {this.Id}, Name = {this.Name}";
	}
}
