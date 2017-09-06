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
	/// 강행수송함대, 발묘!
	/// </summary>
	internal class Bq3 : CountableTracker
	{
		public override QuestType Type => QuestType.Quarterly;
		public override int Id => 861;

		public override int Maximum => 2;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.NodeEndEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				if (args.MapWorldId != 1 || args.MapAreaId != 6) return; // 1-6
				if (args.MapNodeId != 14 && args.MapNodeId != 17) return; // 1-6-N node

				var fleet = Homeport.Instance.Organization.Fleets.FirstOrDefault(x => x.Value.IsInSortie).Value;
				if (fleet?.Ships.Count(x => x.Info.ShipType.Id == 10 || x.Info.ShipType.Id == 22) < 2) return; // BBV or AO under 2

				this.Current = this.Current.Add(1).Max(Maximum);
			};
		}
	}
}
