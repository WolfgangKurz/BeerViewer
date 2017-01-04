using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models
{
	/// <summary>
	/// 임무 종류
	/// </summary>
	public enum QuestType
	{
		/// <summary>
		/// 일일
		/// </summary>
		Daily = 1,

		/// <summary>
		/// 주간
		/// </summary>
		Weekly = 2,

		/// <summary>
		/// 월간
		/// </summary>
		Monthly = 3,

		/// <summary>
		/// 일회성
		/// </summary>
		OneTime = 4,

		/// <summary>
		/// 그 외 (계절?)
		/// </summary>
		Other = 5,
	}
}
