using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Codeplex.Data;

using BeerViewer.Core;
using BeerViewer.Models.Raw;

namespace BeerViewer.Models
{
	public class Quests : Notifier
	{
		private readonly List<ConcurrentDictionary<int, Quest>> questPages;

		#region All 프로퍼티
		private IReadOnlyCollection<Quest> _All;
		public IReadOnlyCollection<Quest> All
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

		#region Current 프로퍼티
		private IReadOnlyCollection<Quest> _Current;
		public IReadOnlyCollection<Quest> Current
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

		#region IsUntaken 프로퍼티
		private bool _IsUntaken;
		public bool IsUntaken
		{
			get { return this._IsUntaken; }
			set
			{
				if (this._IsUntaken != value)
				{
					this._IsUntaken = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region IsEmpty 프로퍼티
		private bool _IsEmpty;
		public bool IsEmpty
		{
			get { return this._IsEmpty; }
			set
			{
				if (this._IsEmpty != value)
				{
					this._IsEmpty = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		internal Quests()
		{
			var proxy = Proxy.Instance;

			this.questPages = new List<ConcurrentDictionary<int, Quest>>();
			this.IsUntaken = true;
			this.All = this.Current = new List<Quest>();

			proxy.Register(Proxy.api_get_member_questlist, e => this.Update(e));
		}

		private void Update(Nekoxy.Session session)
		{
			this.IsUntaken = false;

			var data = DynamicJson.Parse(SvData.JsonParse(session.Response.BodyAsString));
			kcsapi_questlist questlist = new kcsapi_questlist
			{
				api_count = (int)data.api_data.api_count,
				api_disp_page = (int)data.api_data.api_disp_page,
				api_page_count = (int)data.api_data.api_page_count,
				api_exec_count = (int)data.api_data.api_exec_count
			};
			if (data.api_data.api_list != null)
			{
				var list = new List<kcsapi_quest>();
				var serializer = new DataContractJsonSerializer(typeof(kcsapi_quest));

				foreach (var x in data.api_data.api_list)
				{
					try
					{
						list.Add(
							serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(x.ToString()))) as kcsapi_quest
						);
					}
					catch { }
				}

				questlist.api_list = list.ToArray();
			}


			if (this.questPages.Count > questlist.api_page_count)
				while (this.questPages.Count > questlist.api_page_count)
					this.questPages.RemoveAt(this.questPages.Count - 1);

			else if (this.questPages.Count < questlist.api_page_count)
				while (this.questPages.Count < questlist.api_page_count)
					this.questPages.Add(null);


			if (questlist.api_list == null)
			{
				this.IsEmpty = true;
				this.All = this.Current = new List<Quest>();
			}
			else
			{
				var page = questlist.api_disp_page - 1;
				if (page >= this.questPages.Count) page = this.questPages.Count - 1;

				this.questPages[page] = new ConcurrentDictionary<int, Quest>();
				this.IsEmpty = false;

				var quests = questlist.api_list
					.Select(x => new Quest(x));

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
