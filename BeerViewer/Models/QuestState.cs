using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models
{
	public enum QuestState
	{
		/// <summary>
		/// 미수행중
		/// </summary>
		None = 1,

		/// <summary>
		/// 수행중
		/// </summary>
		TakeOn = 2,

		/// <summary>
		/// 완료
		/// </summary>
		Accomplished = 3,
	}
}
