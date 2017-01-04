using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models.BattleInfo
{
	public class MapCellData
	{
		public CellType CellType { get; set; } = CellType.없음;
		public string CellName { get; set; } = "";
		public bool IsFirst { get; set; } = false;
	}
}
