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
	/// 항모기동부대 서쪽으로!
	/// </summary>
	internal class Bm6 : CountableTracker
	{
		public override QuestType Type => QuestType.Monthly;
		public override int Id => 264;

		public override int Maximum => 1;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				if (args.MapWorldId != 4 || args.MapAreaId != 2) return; // 5-n
				if ("東方主力艦隊" != args.EnemyName) return;
				if (args.Rank != "S") return;

				var fleet = Homeport.Instance.Organization.Fleets.FirstOrDefault(x => x.Value.IsInSortie).Value;

				if (fleet?.Ships.Count(x => x.Info.ShipType.Id == 2) < 2) return; // DD under 2
				if (fleet?.Ships.Count(x => new int[] { 7, 11, 18 }.Contains(x.Info.ShipType.Id)) < 2) return; // CV/CVL/CVB under 2

				this.Current = this.Current.Add(1).Max(Maximum);
			};
		}
	}
}
