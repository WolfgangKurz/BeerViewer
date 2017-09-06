using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Models;
using BeerViewer.Models.Enums;
using BeerViewer.Models.kcsapi;
using BeerViewer.Models.Raw;
using BeerViewer.Models.Wrapper;
using BeerViewer.Network;
using System.Collections;

namespace BeerViewer.Models.Quest
{
	public class SlotItemTracker
	{
		internal event EventHandler<BaseEventArgs> CreateItemEvent;
		internal event EventHandler<DestroyItemEventArgs> DestoryItemEvent;

		private readonly Homeport homeport;

		public int SlotItemsCount => this.SlotItems.Count;
		internal MemberTable<SlotItem> SlotItems { get; private set; }

		internal SlotItemTracker(Homeport parent, Proxy proxy)
		{
			this.homeport = parent;

			this.SlotItems = new MemberTable<SlotItem>();

			proxy.Register<kcsapi_slotitem[]>(Proxy.api_get_member_slot_item, x => this.Update(x.Data));
			proxy.Register<kcsapi_createitem>(Proxy.api_req_kousyou_createitem, x => this.CreateItem(x.Data));
			proxy.Register<kcsapi_destroyitem2>(Proxy.api_req_kousyou_destroyitem2, this.DestroyItem);

			proxy.Register<kcsapi_remodel_slot>(Proxy.api_req_kousyou_remodel_slot, x =>
			{
				this.RemoveFromRemodel(x.Data);
				this.RemodelSlotItem(x.Data);
			});

			foreach (var item in this.homeport.Itemyard.SlotItems)
				this.SlotItems.Add(new SlotItem(item.Value.RawData));
		}

		internal void Update(kcsapi_slotitem[] source)
		{
			this.SlotItems = new MemberTable<SlotItem>(source.Select(x => new SlotItem(x)));
			foreach (var ship in this.homeport.Organization.Ships.Values) ship.UpdateSlots();
		}
		internal void RemoveFromRemodel(kcsapi_remodel_slot source)
		{
			if (source.api_use_slot_id != null)
			{
				foreach (var id in source.api_use_slot_id)
					this.SlotItems.Remove(id);
			}
		}

		private void CreateItem(kcsapi_createitem source)
		{
			CreateItemEvent?.Invoke(this, new BaseEventArgs(source.api_create_flag == 1));

			if (source.api_create_flag == 1 && source.api_slot_item != null)
				this.SlotItems.Add(new SlotItem(source.api_slot_item));
		}
		private void DestroyItem(SvData<kcsapi_destroyitem2> data)
		{
			if (data == null || !data.IsSuccess) return;

			try
			{
				DestoryItemEvent?.Invoke(this, new DestroyItemEventArgs(data.Request, data.Data));

				foreach (var x in data.Request["api_slotitem_ids"].Split(',').Select(int.Parse))
					this.SlotItems.Remove(x);
			}
			catch (Exception ex)
			{
				Logger.Instance.Log("Failed to destroy equipment: {0}", ex);
			}
		}
		private void RemodelSlotItem(kcsapi_remodel_slot source)
		{
			if (source.api_after_slot == null) return;

			this.SlotItems[source.api_after_slot.api_id]
				?.Remodel(source.api_after_slot.api_level, source.api_after_slot.api_slotitem_id);
		}

		internal class MemberTable<TValue> : IReadOnlyDictionary<int, TValue> where TValue : class, IIdentifiable
		{
			private readonly IDictionary<int, TValue> dictionary;
			public TValue this[int key] => this.dictionary.ContainsKey(key) ? this.dictionary[key] : null;

			public MemberTable() : this(new List<TValue>()) { }

			public MemberTable(IEnumerable<TValue> source)
			{
				this.dictionary = source.ToDictionary(x => x.Id);
			}

			internal void Add(TValue value)
				=> this.dictionary.Add(value.Id, value);

			internal void Remove(TValue value)
				=> this.dictionary.Remove(value.Id);

			internal void Remove(int id)
				=> this.dictionary.Remove(id);

			#region IReadOnlyDictionary<TK, TV> members
			public IEnumerator<KeyValuePair<int, TValue>> GetEnumerator()
				=> this.dictionary.GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator()
				=> this.GetEnumerator();

			public int Count => this.dictionary.Count;

			public bool ContainsKey(int key)
				=> this.dictionary.ContainsKey(key);

			public bool TryGetValue(int key, out TValue value)
				=> this.dictionary.TryGetValue(key, out value);

			public IEnumerable<int> Keys => this.dictionary.Keys;
			public IEnumerable<TValue> Values => this.dictionary.Values;
			#endregion
		}
		internal class SlotItem : RawDataWrapper<kcsapi_slotitem>, IIdentifiable
		{
			public int Id => this.RawData.api_id;

			public SlotItemInfo Info { get; private set; }

			internal SlotItem(kcsapi_slotitem rawData) : base(rawData)
			{
				this.Info = Master.Instance.SlotItems[this.RawData.api_slotitem_id] ?? SlotItemInfo.Empty;
			}

			public void Remodel(int level, int masterId)
			{
				this.RawData.api_level = level;
				this.Info = Master.Instance.SlotItems[masterId] ?? SlotItemInfo.Empty;

				this.RaisePropertyChanged(nameof(this.Info));
			}

			public override string ToString()
				=> $"ID = {this.Id}, Name = \"{this.Info.Name}\"";

			public static SlotItem Dummy { get; } = new SlotItem(new kcsapi_slotitem { api_slotitem_id = -1, });
		}
	}
}
