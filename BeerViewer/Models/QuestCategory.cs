using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models
{
	/// <summary>
	/// 任務のカテゴリを示す識別子を定義します。
	/// </summary>
	public enum QuestCategory
	{
		/// <summary>
		/// 편성 임무
		/// </summary>
		Composition = 1,

		/// <summary>
		/// 출격 임무
		/// </summary>
		Sortie = 2,

		/// <summary>
		/// 연습 임무
		/// </summary>
		Practice = 3,

		/// <summary>
		/// 원정 임무
		/// </summary>
		Expeditions = 4,

		/// <summary>
		/// 보급/입거 임무
		/// </summary>
		Supply = 5,

		/// <summary>
		/// 공창 임무
		/// </summary>
		Building = 6,

		/// <summary>
		/// 근대화개수 임무
		/// </summary>
		Remodelling = 7,

		/// <summary>
		/// 출격 임무 (2)
		/// </summary>
		// 칸코레 API 구조상 임무는 api_no의 100의 자리와 api_category가 일치하게 구현되어있다
		// api_no 에 대해서 출격 임무였던 200번대가 부족해져서인지, 같은 출격 임무인데도 800번대가 나타나고,
		// 출격을 의미하는 api_category는 2, 8 두 종류가 되었다
		Sortie2 = 8,

		/// <summary>
		/// 그 외 임무
		/// </summary>
		Other = 9,
	}
}
