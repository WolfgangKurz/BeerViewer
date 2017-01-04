using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models.BattleInfo.Raw
{
	interface ICommonFirstBattleMembers : ICommonBattleMembers
	{
		int[] api_formation { get; set; }
	}
}
