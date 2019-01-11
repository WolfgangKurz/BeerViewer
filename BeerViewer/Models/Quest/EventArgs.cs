using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Models.Raw;
using BeerViewer.Models.kcsapi;

namespace BeerViewer.Models.Quest
{
	internal class BaseEventArgs
	{
		public bool IsSuccess { get; set; }

		public BaseEventArgs(bool IsSuccess)
		{
			this.IsSuccess = IsSuccess;
		}
	}

	internal class BattleResultEventArgs
	{
		public string EnemyName { get; set; }
		public TrackerEnemyShip[] EnemyShips { get; set; }

		public int MapWorldId { get; set; }
		public int MapAreaId { get; set; }
		public int MapNodeId { get; set; }

		public bool IsFirstCombat { get; set; }
		public bool IsBoss { get; set; }
		public string Rank { get; set; }

		public BattleResultEventArgs(TrackerMapInfo MapInfo)
		{
			IsFirstCombat = MapInfo.IsFirstCombat;
			MapWorldId = MapInfo.WorldId;
			MapAreaId = MapInfo.MapId;
			MapNodeId = MapInfo.NodeId;
			IsBoss = MapInfo.IsBoss;
		}
		public BattleResultEventArgs(TrackerMapInfo MapInfo, TrackerEnemyShip[] enemyShips, kcsapi_battleresult data) : this(MapInfo)
		{
			EnemyName = data?.api_enemy_info?.api_deck_name;
			EnemyShips = enemyShips;
			Rank = data?.api_win_rank;
		}
		public BattleResultEventArgs(TrackerMapInfo MapInfo, TrackerEnemyShip[] enemyShips, kcsapi_combined_battle_battleresult data) : this(MapInfo)
		{
			EnemyName = data?.api_enemy_info?.api_deck_name;
			EnemyShips = enemyShips;
			Rank = data?.api_win_rank;
		}
	}
	internal class DestroyItemEventArgs
	{
		public int[] itemList { get; set; }

		public DestroyItemEventArgs(NameValueCollection request, kcsapi_destroyitem2 data)
		{
			itemList = request["api_slotitem_ids"]
				.Split(',')
				.Select(int.Parse)
				.ToArray();
		}
	}
	internal class MissionResultEventArgs : BaseEventArgs
	{
		public string Name { get; set; }

		public MissionResultEventArgs(kcsapi_mission_result data) : base(data.api_clear_result > 0)
		{
			Name = data.api_quest_name;
		}
	}
	internal class PracticeResultEventArgs : BaseEventArgs
	{
		public PracticeResultEventArgs(kcsapi_practice_result data) : base("SAB".Contains(data.api_win_rank))
		{
		}
	}
}
