using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models
{
	[Flags]
	public enum FleetSituation
	{
		/// <summary>
		/// 편성된 칸무스 없음
		/// </summary>
		Empty = 0,

		/// <summary>
		/// 모항에서 대기중
		/// </summary>
		Homeport = 1,

		/// <summary>
		/// 연합함대
		/// </summary>
		Combined = 1 << 1,

		/// <summary>
		/// 출격중
		/// </summary>
		Sortie = 1 << 2,

		/// <summary>
		/// 원정중
		/// </summary>
		Expedition = 1 << 3,

		/// <summary>
		/// 대파 존재
		/// </summary>
		HeavilyDamaged = 1 << 4,

		/// <summary>
		/// 보급 안됨
		/// </summary>
		InShortSupply = 1 << 5,

		/// <summary>
		/// 수리중
		/// </summary>
		Repairing = 1 << 6,

		/// <summary>
		/// 공작함 기함
		/// </summary>
		FlagshipIsRepairShip = 1 << 7,
	}
}
