using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models.BattleInfo
{
	public enum BattleRank
	{
		에러 = -1,
		없음 = 0,

		완전승리S = 1,
		S승리 = 2,
		A승리 = 3,
		B승리 = 4,
		C패배 = 5,
		D패배 = 6,
		E패배 = 7,
		공습전 = 8,
	}
}
