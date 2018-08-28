using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeerViewer.Models;
using BeerViewer.Models.Raw;
using BeerViewer.Models.Enums;

namespace BeerViewer.Models.Quest
{
	/// <summary>
	/// 제5전대 출격하라!
	/// </summary>
	internal class Bm1 : CountableTracker
	{
		public override QuestType Type => QuestType.Monthly;
		public override int Id => 249;

		public override int Maximum => 1;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				var shipList = new int[]
				{
					62,  // Myoukou
					265, // Myoukou Kai
					319, // Myoukou Kai Ni
					63,  // Nachi
					266, // Nachi Kai
					192, // Nachi Kai Ni
					65,  // Haguro
					268, // Haguro Kai
					194, // Haguro Kai Ni
				};

				if (args.MapWorldId != 2 || args.MapAreaId != 5) return; // 2-5
				if ("敵主力艦隊" != args.EnemyName) return; // boss
				if (args.Rank != "S") return;

				var fleet = Homeport.Instance.Organization.Fleets.FirstOrDefault(x => x.Value.IsInSortie).Value;
				if (fleet?.Ships.Count(x => shipList.Contains(x.Info?.Id ?? 0)) != 3) return; // Myoukou or Nachi or Haguro not exists

				this.Current = this.Current.Add(1).Max(Maximum);
			};
		}
	}
}
