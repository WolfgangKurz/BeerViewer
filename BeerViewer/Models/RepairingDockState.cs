using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models
{
	/// <summary>
	/// 입거독의 상태
	/// </summary>
	public enum RepairingDockState
	{
		/// <summary>
		/// 개방 안됨
		/// </summary>
		Locked = -1,

		/// <summary>
		/// 개방됨, 미사용중
		/// </summary>
		Unlocked = 0,

		/// <summary>
		/// 개방됨, 사용중
		/// </summary>
		Repairing = 1,
	}
}
