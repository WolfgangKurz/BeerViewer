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
		private readonly List<ConcurrentDictionary<int, QuestModel>> questPages;

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

		#region Current Property
		private IReadOnlyCollection<QuestModel> _Current;
		public IReadOnlyCollection<QuestModel> Current
		{
			get { return this._Current; }
			set
			{
				if (!Equals(this._Current, value))
				{
					this._Current = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		internal Quests()
		{
			var proxy = Proxy.Instance;

			this.questPages = new List<ConcurrentDictionary<int, QuestModel>>();
			this.All = this.Current = new List<QuestModel>();

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
		}

		private void Update(kcsapi_questlist questlist)
		{
			if (this.questPages.Count > questlist.api_page_count)
				while (this.questPages.Count > questlist.api_page_count)
					this.questPages.RemoveAt(this.questPages.Count - 1);

			else if (this.questPages.Count < questlist.api_page_count)
				while (this.questPages.Count < questlist.api_page_count)
					this.questPages.Add(null);


			if (questlist.api_list == null)
			{
				this.All = this.Current = new List<QuestModel>();
			}
			else
			{
				var page = questlist.api_disp_page - 1;
				if (page >= this.questPages.Count) page = this.questPages.Count - 1;

				this.questPages[page] = new ConcurrentDictionary<int, QuestModel>();

				var quests = questlist.api_list
					.Select(x => new QuestModel(x));

				foreach (var quest in quests)
					this.questPages[page].AddOrUpdate(quest.Id, quest, (_, __) => quest);

				this.All = this.questPages.Where(x => x != null)
					.SelectMany(x => x.Select(kvp => kvp.Value))
					.Distinct(x => x.Id)
					.OrderBy(x => x.Id)
					.ToList();

				var current = this.All.Where(x => x.State == QuestState.TakeOn || x.State == QuestState.Accomplished)
					.OrderBy(x => x.Id)
					.ToList();

				while (current.Count < questlist.api_exec_count) current.Add(null);
				this.Current = current;
			}
		}
	}
}
