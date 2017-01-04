using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models.BattleInfo
{
	public enum AirFireFlag
	{
		Unset,
		Unused,
		Used,

		Used1,
		Used2,
		UsedAll
	}

	public static class AirFireFlagExtensions
	{
		public static string ToReadableString(this AirFireFlag flag)
		{
			switch (flag)
			{
				case AirFireFlag.Unused:
					return "발동 안됨";
				case AirFireFlag.Used:
					return "발동";

				case AirFireFlag.Used1:
					return "1차 발동";
				case AirFireFlag.Used2:
					return "2차 발동";
				case AirFireFlag.UsedAll:
					return "전부 발동";
				default:
					return "";
			}
		}
	}
}
