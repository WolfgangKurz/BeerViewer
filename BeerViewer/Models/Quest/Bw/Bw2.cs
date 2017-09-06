﻿using System;
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
	/// 이호작전
	/// </summary>
	internal class Bw2 : CountableTracker
	{
		public override QuestType Type => QuestType.Weekly;
		public override int Id => 220;

		public override int Maximum => 20;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				Current = Current.Add(
						args.EnemyShips
							.Where(x => x.Type == 7 || x.Type == 11 || x.Type == 18)
							.Where(x => x.MaxHp != int.MaxValue && x.NowHp <= 0)
							.Count()
					).Max(Maximum);
			};
		}
	}
}
