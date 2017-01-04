using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models
{
	public enum QuestProgress
	{
		/// <summary>
		/// 진행률 없음
		/// </summary>
		None = 0,

		/// <summary>
		/// 50% 이상 진행
		/// </summary>
		Progress50 = 1,

		/// <summary>
		/// 80% 이상 진행
		/// </summary>
		Progress80 = 2,
	}
}
