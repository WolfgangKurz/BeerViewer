using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;

using BeerViewer.Network;
using BeerViewer.Models.Enums;
using BeerViewer.Models.Raw;
using BeerViewer.Models.Wrapper;
using BeerViewer.Models.kcsapi;
using BeerViewer.Modules;

using Codeplex.Data;
using QuestModel = BeerViewer.Models.Wrapper.Quest;

namespace BeerViewer.Models
{
	public class Quests : Notifier
	{
		private readonly List<QuestModel> currentQuests;

		#region All Property
		private IReadOnlyCollection<QuestModel> _All;
		public IReadOnlyCollection<QuestModel> All
		{
			get { return this._All; }
			set
			{
				if (!Equals(this._All, value))
				{
					this._All = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		internal Quests()
		{
			var proxy = Proxy.Instance;

			this.currentQuests = new List<QuestModel>();
			this.All = this.currentQuests.ToArray();

			proxy.Register(Proxy.api_get_member_questlist, x =>
			{
				var origin = x.Response.BodyAsString.StartsWith("svdata=")
					? x.Response.BodyAsString.Substring(7)
					: null;
				if (origin == null) return; // Not api response

				var json = DynamicJson.Parse(origin);
				json = json.api_data;

				var questlist = new kcsapi_questlist
				{
					api_count = Convert.ToInt32(json.api_count),
					api_disp_page = Convert.ToInt32(json.api_disp_page),
					api_exec_count = Convert.ToInt32(json.api_exec_count),
					api_page_count = Convert.ToInt32(json.api_page_count)
				};

				if (json.api_list != null)
				{
					var serializer = new DataContractJsonSerializer(typeof(kcsapi_quest));
					var list = new List<kcsapi_quest>();

					foreach (var y in (object[])json.api_list)
					{
						if (y.GetType() == typeof(double) && (double)y == -1) // Last page bug
							continue;

						try
						{
							var b = Encoding.UTF8.GetBytes(y.ToString());
							using (var ms = new MemoryStream(b))
								list.Add(serializer.ReadObject(ms) as kcsapi_quest);
						}
						catch (Exception ex)
						{
							Logger.Log(ex.ToString());
						}
					}

					questlist.api_list = list.ToArray();
				}
				this.Update(questlist);
			});
			proxy.Register(Proxy.api_req_quest_clearitemget, e =>
			{
				var x = e.TryParse();

				int q_id;
				if (!int.TryParse(x.Request["api_quest_id"], out q_id)) return;

				ClearQuest(q_id);
			});
			proxy.Register(Proxy.api_req_quest_stop, e =>
			{
				var x = e.TryParse();

				int q_id;
				if (!int.TryParse(x.Request["api_quest_id"], out q_id)) return;

				StopQuest(q_id);
			});
		}

		private void Update(kcsapi_questlist questList)
		{
			if (questList.api_list == null) return;

			foreach (var quest in questList.api_list)
			{
				this.currentQuests.RemoveAll(x => x.Id == quest.api_no);

				switch ((QuestState)quest.api_state)
				{
					/*
					case QuestState.None:
						break;
					*/
					case QuestState.Accomplished:
					case QuestState.TakeOn:
						if (!this.currentQuests.Any(x => x.Id == quest.api_no))
							this.currentQuests.Add(new QuestModel(quest));
						break;
				}
			}
			this.Publish();
		}

		private void ClearQuest(int q_id)
		{
			this.currentQuests.RemoveAll(x => x.Id == q_id);
			this.Publish();
		}
		private void StopQuest(int q_id)
		{
			this.currentQuests.RemoveAll(x => x.Id == q_id);
			this.Publish();
		}

		private void Publish()
		{
			this.All = this.currentQuests
				.OrderBy(x => x.Id)
				.ToArray();
		}
	}
}
