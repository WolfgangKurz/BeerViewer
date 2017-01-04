using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models
{
	/// <summary>
	/// 피로도 종류
	/// </summary>
	public enum ConditionType
	{
		/// <summary>
		/// 반짝이 전의고양 상태 (50 이상, 노란색)
		/// </summary>
		Brilliant,

		/// <summary>
		/// 평소 상태 (40 ~ 49, 흰색)
		/// </summary>
		Normal,

		/// <summary>
		/// 약간 피로 (30 ~ 39, 연한 주황색)
		/// </summary>
		Tired,

		/// <summary>
		/// 중간 피로 (20 ~ 29, 진한 주황색)
		/// </summary>
		OrangeTired,

		/// <summary>
		/// 매우 피로 (0 ~ 20, 빨간색)
		/// </summary>
		RedTired,
	}

	public static class ConditionTypeHelper
	{
		/// <summary>
		/// Int형 피로도 수치를 ConditionType 으로
		/// </summary>
		public static ConditionType ToConditionType(int condition)
		{
			if (condition >= 50) return ConditionType.Brilliant;
			if (condition >= 40) return ConditionType.Normal;
			if (condition >= 30) return ConditionType.Tired;
			if (condition >= 20) return ConditionType.OrangeTired;
			return ConditionType.RedTired;
		}
	}
}
