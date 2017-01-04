using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models.BattleInfo
{
	[Flags]
	public enum CellType
	{
		None = 0,

		시작 = 1 << 0,
		없음 = 1 << 1,
		보급 = 1 << 2,
		소용돌이 = 1 << 3,
		전투 = 1 << 4,
		보스 = 1 << 5,
		상륙지점 = 1 << 6,
		항공전 = 1 << 7,
		모항 = 1 << 8,
		항공정찰 = 1 << 9,
		공습전 = 1 << 10,

		야전 = 1 << 31,

		연습전 = -1,
	}

	public static class CellTypeExtensions
	{
		public static CellType ToCellType(this int colorNo)
		{
			return (CellType)(1 << colorNo);
		}

		public static CellType ToCellType(this string battleType)
		{
			return battleType.Contains("sp_midnight") ? CellType.야전
				: battleType.Contains("ld_airbattle") ? CellType.공습전 // ColorNo 로 알겠지만, 항공전으로 오인하지 않도록
				: battleType.Contains("airbattle") ? CellType.항공전
				: CellType.None;
		}
	}
}
