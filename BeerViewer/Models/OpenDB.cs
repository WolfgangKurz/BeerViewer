using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

using System.Threading;

using BeerViewer.Core;
using BeerViewer.Models;
using BeerViewer.Views.Catalogs;
using BeerViewer.Models.Raw;

namespace BeerViewer.Models
{
	public class OpenDB
	{
#if DEBUG
		private bool DEBUG => true;
#else
		private bool DEBUG => false;
#endif

		private bool Initialized { get; set; } = false;

		// OpenDB host
		private string OpenDBReport => "http://swaytwig.com/opendb/report/";
		private int MAX_TRY => 3;

		private bool Enabled { get; set; }

		public OpenDB()
		{
			Initialized = false;

			var client = DataStorage.Instance;
			client.PropertyEvent(nameof(client.Initialized), () => Initialize());
		}

		private void Initialize()
		{
			if (Initialized) return;
			Initialized = true;

			bool IsFirst = Settings.OpenDB_IsFirst.Value;

			Settings.OpenDB_Enabled.PropertyEvent(nameof(Settings.OpenDB_Enabled.Value), () => this.Enabled = Settings.OpenDB_Enabled.Value, true);

			if (IsFirst || DEBUG) // Is the first load after install?
			{
				// Show alert popup
				catalogOpenDBEnable catalog = new catalogOpenDBEnable();
				catalog.ShowDialog();
			}

			// Save IsFirst setting
			Settings.OpenDB_IsFirst.Value = false;

			var homeport = DataStorage.Instance.Homeport;
			var proxy = Proxy.Instance;

			#region Development (Create slotitem at arsenal)

			proxy.Register(Proxy.api_req_kousyou_createitem, e => {
				var x = e.TryParse<kcsapi_createitem>();
				if (x == null) return;

				///////////////////////////////////////////////////////////////////
				if (!Enabled) return; // Disabled sending statistics data to server

				var item = 0; // Failed to build
				if (x.Data.api_create_flag == 1)
					item = x.Data.api_slot_item.api_slotitem_id;

				var material = new int[] {
					int.Parse(x.Request["api_item1"]),
					int.Parse(x.Request["api_item2"]),
					int.Parse(x.Request["api_item3"]),
					int.Parse(x.Request["api_item4"])
				};
				var flagship = homeport.Organization.Fleets[1].Ships[0].Info.Id;

				new Thread(() =>
				{
					string post = string.Join("&", new string[] {
						"apiver=" + 2,
						"flagship=" + flagship,
						"fuel=" + material[0],
						"ammo=" + material[1],
						"steel=" + material[2],
						"bauxite=" + material[3],
						"result=" + item
					});

					int tries = MAX_TRY;
					while (tries > 0)
					{
						var y = HTTPRequest.Post(OpenDBReport + "equip_dev.php", post);
						if (y != null)
						{
							y?.Close();
							break;
						}
						tries--;
					}
				}).Start();
			});
			#endregion

			#region Construction (Build new ship at arsenal)

			bool ship_dev_wait = false;
			int ship_dev_dockid = 0;

			proxy.Register(Proxy.api_req_kousyou_createship, e => {
				var x = e.TryParse();
				if (x == null) return;
				
				ship_dev_wait = true;
				ship_dev_dockid = int.Parse(x.Request["api_kdock_id"]);
			});
			proxy.Register(Proxy.api_get_member_kdock, e => {
				var x = e.TryParse<kcsapi_kdock[]>();
				if (x == null) return;

				if (!ship_dev_wait) return; // Not created
				ship_dev_wait = false;

				///////////////////////////////////////////////////////////////////
				if (!Enabled) return; // Disabled sending statistics data to server

				var dock = x.Data.SingleOrDefault(y => y.api_id == ship_dev_dockid);
				var flagship = homeport.Organization.Fleets[1].Ships[0].Info.Id;
				var ship = dock.api_created_ship_id;

				new Thread(() =>
				{
					string post = string.Join("&", new string[] {
						"apiver=" + 2,
						"flagship=" + flagship,
						"fuel=" + dock.api_item1,
						"ammo=" + dock.api_item2,
						"steel=" + dock.api_item3,
						"bauxite=" + dock.api_item4,
						"material=" + dock.api_item5,
						"result=" + ship
					});

					int tries = MAX_TRY;
					while (tries > 0)
					{
						var y = HTTPRequest.Post(OpenDBReport + "ship_dev.php", post);
						if (y != null)
						{
							y?.Close();
							break;
						}
						tries--;
					}
				}).Start();
			});
			#endregion

			#region Drop (Get new ship from sea)

			int drop_world = 0;
			int drop_map = 0;
			int drop_node = 0;
			int drop_maprank = 0;

			var drop_prepare = new Action<kcsapi_startnext>(x =>
			{
				drop_world = x.api_maparea_id;
				drop_map = x.api_mapinfo_no;
				drop_node = x.api_no;
				drop_maprank = x.api_eventmap?.api_selected_rank ?? 0;
				// 0:None, 丙:1, 乙:2, 甲:3
			});
			var drop_report = new Action<kcsapi_battleresult>(x =>
			{
				///////////////////////////////////////////////////////////////////
				if (!Enabled) return; // Disabled sending statistics data to server

				if (homeport.Organization.Ships.Count >= homeport.Admiral.MaxShipCount)
					return; // Maximum ship-count

				var drop_shipid = 0;
				var drop_rank = x.api_win_rank;
				if (x.api_get_ship != null) drop_shipid = x.api_get_ship.api_ship_id;

				new Thread(() =>
				{
					string post = string.Join("&", new string[] {
							"apiver=" + 3,
							"world=" + drop_world,
							"map=" + drop_map,
							"node=" + drop_node,
							"rank=" + drop_rank,
							"maprank=" + drop_maprank,
							"result=" + drop_shipid
						});

					int tries = MAX_TRY;
					while (tries > 0)
					{
						var y = HTTPRequest.Post(OpenDBReport + "ship_drop.php", post);
						if (y != null)
						{
							y?.Close();
							break;
						}
						tries--;
					}
				}).Start();
			});

			// To gether Map-id
			proxy.Register(Proxy.api_req_map_start, e =>
			{
				var x = e.TryParse<kcsapi_startnext>();
				if (x == null) return;

				drop_prepare(x.Data);
			});
			proxy.Register(Proxy.api_req_map_next, e =>
			{
				var x = e.TryParse<kcsapi_startnext>();
				if (x == null) return;

				drop_prepare(x.Data);
			});

			// To gether dropped ship
			proxy.Register(Proxy.api_req_sortie_battleresult, e =>
			{
				var x = e.TryParse<kcsapi_battleresult>();
				if (x == null) return;

				drop_report(x.Data);
			});
			proxy.Register(Proxy.api_req_combined_battle_battleresult, e =>
			{
				var x = e.TryParse<kcsapi_battleresult>();
				if (x == null) return;

				drop_report(x.Data);
			});

			#endregion

			#region Slotitem Improvement (Remodel slotitem)

			proxy.Register(Proxy.api_req_kousyou_remodel_slot, e => {
				var x = e.TryParse<kcsapi_remodel_slot>();
				if (x == null) return;

				///////////////////////////////////////////////////////////////////
				if (!Enabled) return; // Disabled sending statistics data to server

				if (int.Parse(x.Request["api_certain_flag"]) == 1) return; // 100% improvement option used

				var item = x.Data.api_remodel_id[0]; // Slotitem master id
				var flagship = homeport.Organization.Fleets[1].Ships[0].Info.Id; // Flagship (Akashi or Akashi Kai)
				var assistant = x.Data.api_voice_ship_id; // Assistant ship master id
				var level = 0; // After level
				var result = x.Data.api_remodel_flag; // Is succeeded?

				// !!! api_after_slot is null when failed to improve !!!

				if (result == 1)
				{
					level = x.Data.api_after_slot.api_level - 1;
					if (level < 0) level = 10;
				}
				else
				{
					level = homeport.Itemyard.SlotItems[
						int.Parse(x.Request["api_slot_id"])
					].Level;
				}

				new Thread(() =>
				{
					string post = string.Join("&", new string[] {
						"apiver=" + 2,
						"flagship=" + flagship,
						"assistant=" + assistant,
						"item=" + item,
						"level=" + level,
						"result=" + result
					});

					int tries = MAX_TRY;
					while (tries > 0)
					{
						var y = HTTPRequest.Post(OpenDBReport + "equip_remodel.php", post);
						if (y != null)
						{
							y?.Close();
							break;
						}
						tries--;
					}
				}).Start();
			});

			#endregion
		}
	}
}
