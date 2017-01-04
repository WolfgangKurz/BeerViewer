using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Core;
using BeerViewer.Models.Raw;

namespace BeerViewer.Models.BattleInfo
{
	public class MapCell
	{
		public int ColorNo { get; private set; }
		public int Id { get; private set; }
		public int MapInfoId { get; private set; }

		public MapInfo MapInfo => DataStorage.Instance.Master.MapInfos[this.MapInfoId] ?? MapInfo.Dummy;
		public int MapAreaId { get; private set; }
		public int MapInfoIdInEachMapArea { get; private set; }

		public int IdInEachMapInfo { get; private set; }

		public MapCell(kcsapi_mst_mapcell cell)
		{
			this.ColorNo = cell.api_color_no;
			this.Id = cell.api_id;
			this.MapInfoId = cell.api_map_no;
			this.MapAreaId = cell.api_maparea_id;
			this.MapInfoIdInEachMapArea = cell.api_mapinfo_no;
			this.IdInEachMapInfo = cell.api_no;
		}
	}
}
