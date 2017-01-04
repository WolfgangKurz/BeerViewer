using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models
{
	/// <summary>
	/// 건조독 상태
	/// </summary>
	public enum BuildingDockState
	{
		/// <summary>
		/// 개방 안됨
		/// </summary>
		Locked = -1,

		/// <summary>
		/// 개방됨, 미사용
		/// </summary>
		Unlocked = 0,

		/// <summary>
		/// 개방됨, 건조중
		/// </summary>
		Building = 2,

		/// <summary>
		/// 개방됨, 건조 완료
		/// </summary>
		Completed = 3,
	}
}
