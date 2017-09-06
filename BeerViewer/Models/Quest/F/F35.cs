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
	/// 숙련승무원 양성
	/// </summary>
	internal class F35 : CountableTracker
	{
		public override QuestType Type => QuestType.Quarterly;
		public override int Id => 637;

		public override int Maximum => 1;

		public override void RegisterEvent(TrackManager manager)
		{
			EventHandler handler = (sender, args) =>
			{
				if (!IsTracking) return;

				var flagshipTable = new int[]
				{
					89,  // Houshou
					285, // Houshou Kai
				};

				var fleet = Homeport.Instance.Organization.Fleets[1];
				if (!flagshipTable.Any(x => x == (fleet?.Ships[0]?.Info.Id ?? 0))) // Flagship is not Houshou
				{
					this.Current = 0;
					return;
				}

				var slotitems = fleet?.Ships[0]?.Slots;
				if (!slotitems.Any(x => x.Item.Info.Id == 19 && x.Item.Level == 10 && x.Item.Proficiency == 7))
				{
					// Max proficiency, Max improvement Type 96 Fighter
					this.Current = 0;
					return;
				}

				this.Current = 1;
			};
			manager.HenseiEvent += handler;
			manager.EquipEvent += handler;
		}
	}
}
