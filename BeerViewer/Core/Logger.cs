using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using BeerViewer.Models;
using BeerViewer.Models.Raw;
using BeerViewer.Models.BattleInfo.Raw;

namespace BeerViewer.Core
{
	public class Logger
	{
		private class NodeData
		{
			public int world { get; set; }
			public int mapnum { get; set; }
			public int node { get; set; }
		}

		private bool waitingForShip = false;
		private int dockid;
		private int[] shipmats;

		private int CurrentDeckId;
		private bool IsBossCell;
		private NodeData nodeData;

		enum LogType { BuildItem, BuildShip, ShipDrop };

		internal Logger()
		{
			var proxy = Proxy.Instance;

			this.shipmats = new int[5];
			try
			{
				proxy.Register(Proxy.api_req_map_start, e =>
				{
					var x = e.TryParse<kcsapi_startnext>();
					if (x == null) return;

					MapStartNext(x.Data, x.Request["api_deck_id"]);
				});
				proxy.Register(Proxy.api_req_map_next, e =>
				{
					var x = e.TryParse<kcsapi_startnext>();
					MapStartNext(x.Data);
				});

				proxy.Register(Proxy.api_req_sortie_battleresult, e =>
				{
					var x = e.TryParse<kcsapi_battleresult>();
					this.BattleResult(x.Data);
				});
				proxy.Register(Proxy.api_req_combined_battle_battleresult, e =>
				{
					var x = e.TryParse<kcsapi_battleresult>();
					this.BattleResult(x.Data);
				});

				proxy.Register(Proxy.api_req_kousyou_createitem, e =>
				{
					var x = e.TryParse<kcsapi_createitem>();
					this.CreateItem(x.Data, x.Request);
				});
				proxy.Register(Proxy.api_req_kousyou_createship, e =>
				{
					var x = e.TryParse();
					this.CreateShip(x.Request);
				});
				proxy.Register(Proxy.api_get_member_kdock, e =>
				{
					var x = e.TryParse<kcsapi_kdock[]>();
					this.KDock(x.Data);
				});
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		private void CreateItem(kcsapi_createitem source, NameValueCollection req)
		{
			this.Log(
				LogType.BuildItem,
				"{0},{1},{2},{3},{4},{5},{6}",
				DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
				DataStorage.Instance.Homeport.Organization.Fleets[1].Ships[0].Info.ShipType.Name,
				req["api_item1"],
				req["api_item2"],
				req["api_item3"],
				req["api_item4"],
				source.api_create_flag == 1
				? DataStorage.Instance.Master.SlotItems[source.api_slot_item.api_slotitem_id].Name
				: "NA"
			);
		}
		private void CreateShip(NameValueCollection req)
		{
			this.waitingForShip = true;
			this.dockid = Int32.Parse(req["api_kdock_id"]);
			this.shipmats[0] = Int32.Parse(req["api_item1"]);
			this.shipmats[1] = Int32.Parse(req["api_item2"]);
			this.shipmats[2] = Int32.Parse(req["api_item3"]);
			this.shipmats[3] = Int32.Parse(req["api_item4"]);
			this.shipmats[4] = Int32.Parse(req["api_item5"]);
		}
		private void KDock(kcsapi_kdock[] docks)
		{
			foreach (var dock in docks)
			{
				if (waitingForShip && dock.api_id == dockid)
				{
					Log(
						LogType.BuildShip,
						"{0},{1},{2},{3},{4},{5},{6},{7}",
						DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
						DataStorage.Instance.Homeport.Organization.Fleets[1].Ships[0].Info.Name,
						shipmats[0],
						shipmats[1],
						shipmats[2],
						shipmats[3],
						shipmats[4],
						DataStorage.Instance.Master.Ships[dock.api_created_ship_id].Name
					);
					waitingForShip = false;
				}
			}
		}

		private void MapStartNext(kcsapi_startnext startnext, string api_deck_id = null)
		{
			if (api_deck_id != null)
				CurrentDeckId = int.Parse(api_deck_id);

			IsBossCell = startnext.api_event_id == 5;

			var organization = DataStorage.Instance.Homeport.Organization;
			nodeData = new NodeData
			{
				world = startnext.api_maparea_id,
				mapnum = startnext.api_mapinfo_no,
				node = startnext.api_no
			};
		}

		private void BattleResult(kcsapi_battleresult br)
		{
			string ShipName = "";
			string MapType = "";
			if (br.api_get_ship != null)
			{
				ShipName = Translator.ShipNameTable.ContainsKey(br.api_get_ship.api_ship_name)
					? Translator.ShipNameTable[br.api_get_ship.api_ship_name]
					: br.api_get_ship.api_ship_name;
			}
			MapType = br.api_quest_name;

			string currentTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

			#region CSV파일 저장
			//날짜,해역이름,해역,보스,적 함대,랭크,드랍
			Log(LogType.ShipDrop, "{0},{1},{2},{3},{4},{5},{6}",
				currentTime,
				MapType,
				$"{nodeData.world}-{nodeData.mapnum}-{nodeData.node}",
				IsBossCell ? "O" : "X",
				br.api_enemy_info.api_deck_name,
				br.api_win_rank,
				ShipName);
			#endregion
		}

		private void Log(LogType Type, string format, params object[] args)
		{
			try
			{
				byte[] utf8Bom = { 0xEF, 0xBB, 0xBF };
				string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

				if (Type == LogType.BuildItem)
				{
					if (!File.Exists(MainFolder + "\\ItemBuildLog.csv"))
					{
						var csvPath = Path.Combine(MainFolder, "ItemBuildLog.csv");
						using (var fileStream = new FileStream(csvPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
						using (var writer = new BinaryWriter(fileStream))
						{
							writer.Write(utf8Bom);
						}
						using (StreamWriter w = File.AppendText(MainFolder + "\\ItemBuildLog.csv"))
						{
							w.WriteLine("날짜,비서함,연료,탄,강재,보크사이트,결과", args);
						}
					}
					using (StreamWriter w = File.AppendText(MainFolder + "\\ItemBuildLog.csv"))
					{
						w.WriteLine(format, args);
					}
					//bin 작성 시작

				}
				else if (Type == LogType.BuildShip)
				{
					if (!File.Exists(MainFolder + "\\ShipBuildLog.csv"))
					{
						var csvPath = Path.Combine(MainFolder, "ShipBuildLog2.csv");
						using (var fileStream = new FileStream(csvPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
						using (var writer = new BinaryWriter(fileStream))
						{
							writer.Write(utf8Bom);
						}
						using (StreamWriter w = File.AppendText(MainFolder + "\\ShipBuildLog.csv"))
						{
							w.WriteLine("날짜,비서함,연료,탄,강재,보크사이트,개발자재,결과", args);
						}
					}

					using (StreamWriter w = File.AppendText(MainFolder + "\\ShipBuildLog.csv"))
					{
						w.WriteLine(format, args);
					}
				}
				else if (Type == LogType.ShipDrop)
				{
					if (!File.Exists(MainFolder + "\\DropLog2.csv"))
					{
						var csvPath = Path.Combine(MainFolder, "DropLog2.csv");
						using (var fileStream = new FileStream(csvPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
						using (var writer = new BinaryWriter(fileStream))
						{
							writer.Write(utf8Bom);
						}
						using (StreamWriter w = File.AppendText(MainFolder + "\\DropLog.csv"))
						{
							w.WriteLine("날짜,해역이름,해역,보스,적 함대,랭크,드랍", args);
						}
					}

					using (StreamWriter w = File.AppendText(MainFolder + "\\DropLog2.csv"))
					{
						w.WriteLine(format, args);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}
	}
}
