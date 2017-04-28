using System.Linq;

using BeerViewer.Models.Raw;
using BeerViewer.Models.kcsapi.mst;

namespace BeerViewer.Models.Wrapper
{
	public class MapArea : RawDataWrapper<kcsapi_mst_maparea>, IIdentifiable
	{
		public int Id => this.RawData.api_id;
		public string Name => this.RawData.api_name;

		public MasterTable<MapInfo> MapInfos { get; }

		public MapArea(kcsapi_mst_maparea maparea, MasterTable<MapInfo> mapInfos) : base(maparea)
		{
			this.MapInfos = new MasterTable<MapInfo>(mapInfos.Values.Where(x => x.MapAreaId == maparea.api_id));
			foreach (var cell in this.MapInfos.Values)
				cell.MapArea = this;
		}

		public override string ToString()
			=> $"ID = {this.Id}, Name = {this.Name}";
	}
}
