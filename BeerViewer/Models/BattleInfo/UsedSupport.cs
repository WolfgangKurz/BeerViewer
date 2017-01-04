using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models.BattleInfo
{
	public enum UsedSupport
	{
		Unset,
		Unused,
		Hourai,
		Kouku
	}

	public static class UsedSupportExtensions
	{
		public static string ToReadableString(this UsedSupport flag)
		{
			switch (flag)
			{
				case UsedSupport.Unused:
					return "지원 없음";
				case UsedSupport.Hourai:
					return "포격/뇌격";
				case UsedSupport.Kouku:
					return "항공 지원";
				default:
					return "";
			}
		}
	}
}
