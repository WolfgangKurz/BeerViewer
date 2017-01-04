using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models.BattleInfo
{
	/// <summary>
	/// 1함대분의 데미지 목록
	/// </summary>
	public class FleetDamages
	{
		public int Ship1 { get; set; }
		public int Ship2 { get; set; }
		public int Ship3 { get; set; }
		public int Ship4 { get; set; }
		public int Ship5 { get; set; }
		public int Ship6 { get; set; }

		public int[] ToArray()
		{
			return new[]
			{
				this.Ship1,
				this.Ship2,
				this.Ship3,
				this.Ship4,
				this.Ship5,
				this.Ship6,
			};
		}

		public FleetDamages Add(FleetDamages value)
		{
			return Parse(new[]
			{
				this.Ship1 + value.Ship1,
				this.Ship2 + value.Ship2,
				this.Ship3 + value.Ship3,
				this.Ship4 + value.Ship4,
				this.Ship5 + value.Ship5,
				this.Ship6 + value.Ship6,
			});
		}

		public static FleetDamages Parse(IEnumerable<int> damages)
		{
			if (damages == null) throw new ArgumentNullException();
			var arr = damages.ToArray();
			if (arr.Length != 6) throw new ArgumentException("함대 데미지 배열의 길이는 6이어야 합니다.");
			return new FleetDamages
			{
				Ship1 = arr[0],
				Ship2 = arr[1],
				Ship3 = arr[2],
				Ship4 = arr[3],
				Ship5 = arr[4],
				Ship6 = arr[5],
			};
		}
	}

	public static class FleetDamagesExtensions
	{
		public static FleetDamages ToFleetDamages(this IEnumerable<int> damages)
		{
			return FleetDamages.Parse(damages);
		}
	}
}
