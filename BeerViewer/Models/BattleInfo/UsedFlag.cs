using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models.BattleInfo
{
	public enum UsedFlag
	{
		Unset,
		Unused,
		Used,
		EnemyUsed,
		BothUsed
	}

	public static class UsedFlagExtensions
	{
		public static string ToReadableString(this UsedFlag flag)
		{
			switch (flag)
			{
				case UsedFlag.Unused:
					return "발동 안됨";
				case UsedFlag.Used:
					return "아군 발동";
				case UsedFlag.EnemyUsed:
					return "적 발동";
				case UsedFlag.BothUsed:
					return "쌍방 발동";
				default:
					return "";
			}
		}
	}
}
