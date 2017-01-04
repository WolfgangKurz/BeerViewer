using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models
{
	/// <summary>
	/// 속력 종류를 정의
	/// </summary>
	public enum ShipSpeed
	{
		/// <summary>
		/// 불명 (기지 등)
		/// </summary>
		Immovable = 0,

		/// <summary>
		/// 저속
		/// </summary>
		Slow = 5,

		/// <summary>
		/// 고속
		/// </summary>
		Fast = 10,
	}
}
