using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

using BeerViewer.Models.kcsapi;
using BeerViewer.Network;

namespace BeerViewer.Models.Quest
{
	public class TrackManager
	{
		public static TrackManager Instance { get; } = new TrackManager();

		public ObservableCollection<Tracker> trackingAvailable
		{
			get; private set;
		} = new ObservableCollection<Tracker>();

		public List<Tracker> TrackingQuests => trackingAvailable.Where(x => x.IsTracking).ToList();
		public List<Tracker> AllQuests => trackingAvailable.ToList();

		internal event EventHandler<BattleResultEventArgs> BattleResultEvent;
		internal event EventHandler<BattleResultEventArgs> NodeEndEvent;
		internal event EventHandler<MissionResultEventArgs> MissionResultEvent;
		internal event EventHandler<PracticeResultEventArgs> PracticeResultEvent;
		internal event EventHandler RepairStartEvent;
		internal event EventHandler ChargeEvent;
		internal event EventHandler<BaseEventArgs> CreateItemEvent;
		internal event EventHandler CreateShipEvent;
		internal event EventHandler DestroyShipEvent;
		internal event EventHandler<DestroyItemEventArgs> DestroyItemEvent;
		internal event EventHandler<BaseEventArgs> PowerUpEvent;
		internal event EventHandler<BaseEventArgs> RemodelEvent;
		internal event EventHandler HenseiEvent;
		internal event EventHandler EquipEvent;

		internal SlotItemTracker slotitemTracker { get; }

		public readonly System.EventArgs EmptyEventArg = new System.EventArgs();
		public event EventHandler QuestsEventChanged;

		private void Preprocess(Action action)
		{
			try { action(); }
			catch { }
		}

		public TrackManager()
		{
			var homeport = Homeport.Instance;
			var proxy = Proxy.Instance;
			var MapInfo = new TrackerMapInfo();
			var battleTracker = new BattleTracker();

			slotitemTracker = new SlotItemTracker(homeport, proxy);

			// Ship changing on fleet
			homeport.Organization.PropertyEvent(nameof(homeport.Organization.Fleets), () =>
			{
				var fleets = homeport.Organization.Fleets.Select(x => x.Value);
				foreach (var x in fleets)
					x.PropertyEvent(nameof(x.ShipsUpdated), () => Preprocess(() => HenseiEvent?.Invoke(this, this.EmptyEventArg)));
			});

			// Equipment changing
			proxy.Register<kcsapi_slot_exchange_index>(Proxy.api_req_kaisou_slot_exchange_index,
				x => Preprocess(() => EquipEvent?.Invoke(this, this.EmptyEventArg)));
			proxy.Register<kcsapi_slot_deprive>(Proxy.api_req_kaisou_slot_deprive,
				x => Preprocess(() => EquipEvent?.Invoke(this, this.EmptyEventArg)));

			// Practice end
			proxy.Register<kcsapi_practice_result>("/kcsapi/api_req_practice/battle_result",
				x => Preprocess(() => PracticeResultEvent?.Invoke(this, new PracticeResultEventArgs(x.Data))));

			// Powerup ship
			proxy.Register<kcsapi_powerup>(Proxy.api_req_kaisou_powerup,
				x => Preprocess(() => PowerUpEvent?.Invoke(this, new BaseEventArgs(x.Data.api_powerup_flag != 0))));

			// Improve item
			proxy.Register<kcsapi_remodel_slot>(Proxy.api_req_kousyou_remodel_slot,
				x => Preprocess(() => RemodelEvent?.Invoke(this, new BaseEventArgs(x.Data.api_remodel_flag != 0))));

			// Destroy item
			slotitemTracker.DestoryItemEvent += (s, e) => Preprocess(() => DestroyItemEvent?.Invoke(this, e));

			// Destroy ship
			proxy.Register<kcsapi_destroyship>(Proxy.api_req_kousyou_destroyship,
				x => Preprocess(() => DestroyShipEvent?.Invoke(this, this.EmptyEventArg)));

			// Construction (create ship)
			proxy.Register<kcsapi_createship>(Proxy.api_req_kousyou_createship,
				x => Preprocess(() => CreateShipEvent?.Invoke(this, this.EmptyEventArg)));

			// Development (create item)
			slotitemTracker.CreateItemEvent += (s, e) => Preprocess(() => CreateItemEvent?.Invoke(this, e));

			// Supply
			proxy.Register<kcsapi_charge>(Proxy.api_req_hokyu_charge,
				x => Preprocess(() => ChargeEvent?.Invoke(this, this.EmptyEventArg)));

			// Repair
			proxy.Register("/kcsapi/api_req_nyukyo/start",
				x => Preprocess(() => RepairStartEvent?.Invoke(this, this.EmptyEventArg)));

			// Expedition
			proxy.Register<kcsapi_mission_result>(Proxy.api_req_mission_result,
				x => Preprocess(() => MissionResultEvent?.Invoke(this, new MissionResultEventArgs(x.Data))));

			// Sortie(Start, Next), Homing(Node ended)
			proxy.Register<kcsapi_map_start>(Proxy.api_req_map_start,
				x => Preprocess(() => MapInfo.Reset(x.Data.api_maparea_id, x.Data.api_mapinfo_no, x.Data.api_no, x.Data.api_event_id == 5)));
			proxy.Register<kcsapi_map_start>(Proxy.api_req_map_next,
				x => Preprocess(() =>
				{
					NodeEndEvent?.Invoke(this, new BattleResultEventArgs(MapInfo));
					MapInfo.Next(x.Data.api_maparea_id, x.Data.api_mapinfo_no, x.Data.api_no, x.Data.api_event_id == 5);
				}));

			#region Battles
			// Single - Day Combat
			proxy.Register<kcsapi_sortie_battle>(Proxy.api_req_sortie_battle,
				x => Preprocess(() => battleTracker.BattleProcess(x.Data)));

			// Single - Night Battle
			proxy.Register<kcsapi_battle_midnight_battle>("/kcsapi/api_req_battle_midnight/battle",
				x => Preprocess(() => battleTracker.BattleProcess(x.Data)));

			// Single - Begin Night Battle
			proxy.Register<kcsapi_battle_midnight_sp_midnight>("/kcsapi/api_req_battle_midnight/sp_midnight",
				x => Preprocess(() => battleTracker.BattleProcess(x.Data)));

			// Single - Aerial Battle
			proxy.Register<kcsapi_sortie_airbattle>("/kcsapi/api_req_sortie/airbattle",
				x => Preprocess(() => battleTracker.BattleProcess(x.Data)));

			// Single - Air Raid Battle
			proxy.Register<kcsapi_sortie_airbattle>("/kcsapi/api_req_sortie/ld_airbattle",
				x => Preprocess(() => battleTracker.BattleProcess(x.Data)));

			// Combined - Day Combat (Carrier/Surface Task Force)
			proxy.Register<kcsapi_combined_battle>(Proxy.api_req_combined_battle_battle,
				x => Preprocess(() => battleTracker.BattleProcess(x.Data)));

			// Combined - Day Combat (Transport Escort)
			proxy.Register<kcsapi_combined_battle>("/kcsapi/api_req_combined_battle/battle_water",
				x => Preprocess(() => battleTracker.BattleProcess(x.Data)));

			// Single vs Combined - Day Combat
			proxy.Register<kcsapi_combined_each_battle>("/kcsapi/api_req_combined_battle/ec_battle",
				x => Preprocess(() => battleTracker.BattleProcess(x.Data, false)));

			// Combined vs Combined - Day Combat (Carrier/Surface Task Force)
			proxy.Register<kcsapi_combined_each_battle>("/kcsapi/api_req_combined_battle/each_battle",
				x => Preprocess(() => battleTracker.BattleProcess(x.Data, true)));

			// Combined vs Combined - Day Combat (Transport Escort)
			proxy.Register<kcsapi_combined_each_battle>("/kcsapi/api_req_combined_battle/each_battle_water",
				x => Preprocess(() => battleTracker.BattleProcess(x.Data, true)));

			// Combined - Aerial Battle
			proxy.Register<kcsapi_combined_battle_airbattle>(Proxy.api_req_combined_battle_airbattle,
				x => Preprocess(() => battleTracker.BattleProcess(x.Data)));

			// Combined - Air Raid Battle
			proxy.Register<kcsapi_combined_battle_airbattle>("/kcsapi/api_req_combined_battle/ld_airbattle",
				x => Preprocess(() => battleTracker.BattleProcess(x.Data)));

			// Combined - Night Battle
			proxy.Register<kcsapi_combined_battle_midnight_battle>("/kcsapi/api_req_combined_battle/midnight_battle",
				x => Preprocess(() => battleTracker.BattleProcess(x.Data)));

			// Combined - Begin Night Battle
			proxy.Register<kcsapi_combined_battle_midnight_battle>("/kcsapi/api_req_combined_battle/sp_midnight",
				x => Preprocess(() => battleTracker.BattleProcess(x.Data)));

			// Combined vs Combined - Night Battle
			proxy.Register<kcsapi_combined_each_midnight_battle>("/kcsapi/api_req_combined_battle/ec_midnight_battle",
				x => Preprocess(() => battleTracker.BattleProcess(x.Data)));
			#endregion

			// Battle end (Includes Combined-Fleet)
			proxy.Register<kcsapi_battleresult>(Proxy.api_req_sortie_battleresult,
				x => Preprocess(() => BattleResultEvent?.Invoke(this, new BattleResultEventArgs(MapInfo.AfterCombat(), battleTracker.enemyShips, x.Data))));
			proxy.Register<kcsapi_combined_battle_battleresult>(Proxy.api_req_combined_battle_battleresult,
				x => Preprocess(() => BattleResultEvent?.Invoke(this, new BattleResultEventArgs(MapInfo.AfterCombat(), battleTracker.enemyShips, x.Data))));

			// Register all trackers
			trackingAvailable = new ObservableCollection<Tracker>(trackingAvailable.OrderBy(x => x.Id));
			trackingAvailable.CollectionChanged += (sender, e) =>
			{
				if (e.Action != NotifyCollectionChangedAction.Add) return;

				foreach (Tracker tracker in e.NewItems)
				{
					tracker.RegisterEvent(this);
					tracker.ResetQuest();
					tracker.ProcessChanged += ((x, y) =>
					{
						try
						{
							QuestsEventChanged?.Invoke(this, EmptyEventArg);
						}
						catch { }
					});
				}
			};
		}

		public void RefreshTrackers()
		{
			Preprocess(() => HenseiEvent?.Invoke(this, EmptyEventArg));
			Preprocess(() => EquipEvent?.Invoke(this, EmptyEventArg));
			QuestsEventChanged?.Invoke(this, EmptyEventArg);
		}
	}
}
