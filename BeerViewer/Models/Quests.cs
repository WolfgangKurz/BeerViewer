using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using BeerViewer.Network;
using BeerViewer.Models.Enums;
using BeerViewer.Models.Raw;
using BeerViewer.Models.Wrapper;
using BeerViewer.Models.kcsapi;


namespace BeerViewer.Models
{
	public class Quests : Notifier
	{
		private readonly List<ConcurrentDictionary<int, Quest>> questPages;

		#region All Property
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

		#region Current Property
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

		internal Quests()
		{
			var proxy = Proxy.Instance;

			this.questPages = new List<ConcurrentDictionary<int, Quest>>();
			this.All = this.Current = new List<Quest>();

			proxy.Register<kcsapi_questlist>(Proxy.api_get_member_questlist, x => this.Update(x.Data));
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
				this.All = this.Current = new List<Quest>();
			}
			else
			{
				var page = questlist.api_disp_page - 1;
				if (page >= this.questPages.Count) page = this.questPages.Count - 1;

				this.questPages[page] = new ConcurrentDictionary<int, Quest>();

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
