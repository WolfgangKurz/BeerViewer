using System;
using System.Linq;

using BeerViewer.Network;
using BeerViewer.Models.Raw;
using BeerViewer.Models.Enums;
using BeerViewer.Models.Wrapper;
using BeerViewer.Models.kcsapi;

namespace BeerViewer.Models
{
	public class Itemyard : Notifier
	{
		private readonly Homeport homeport;

		public int SlotItemsCount => this.SlotItems.Count;

		#region SlotItems Property
		private MemberTable<SlotItem> _SlotItems;
		public MemberTable<SlotItem> SlotItems
		{
			get { return this._SlotItems; }
			set
			{
				if (this._SlotItems != value)
				{
					this._SlotItems = value;
					this.RaiseSlotItemsChanged();
				}
			}
		}
		#endregion

		#region UseItems Property
		private MemberTable<UseItem> _UseItems;
		public MemberTable<UseItem> UseItems
		{
			get { return this._UseItems; }
			set
			{
				if (this._UseItems != value)
				{
					this._UseItems = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		internal Itemyard(Homeport parent)
		{
			var proxy = Proxy.Instance;
			this.homeport = parent;

			this.SlotItems = new MemberTable<SlotItem>();
			this.UseItems = new MemberTable<UseItem>();

			proxy.Register<kcsapi_slotitem[]>(Proxy.api_get_member_slot_item, x => this.Update(x.Data));
			proxy.Register<kcsapi_createitem>(Proxy.api_req_kousyou_createitem, x=>this.CreateItem(x.Data));
			proxy.Register<kcsapi_destroyitem2>(Proxy.api_req_kousyou_destroyitem2, x => this.DestroyItem(x));

			proxy.Register<kcsapi_useitem[]>(Proxy.api_get_member_useitem, x => this.Update(x.Data));
			proxy.Register<kcsapi_remodel_slot>(Proxy.api_req_kousyou_remodel_slot, x =>
			{
				this.RemoveFromRemodel(x.Data);
				this.RemodelSlotItem(x.Data);
			});
		}


		internal void Update(kcsapi_slotitem[] source)
		{
			this.SlotItems = new MemberTable<SlotItem>(source.Select(x => new SlotItem(x)));
			foreach (var ship in this.homeport.Organization.Ships.Values) ship.UpdateSlots();
		}

		internal void Update(kcsapi_useitem[] source)
		{
			this.UseItems = new MemberTable<UseItem>(source.Select(x => new UseItem(x)));
		}

		internal void AddFromDock(kcsapi_kdock_getship source)
		{
			if (source.api_slotitem == null) return; // まるゆ

			foreach (var x in source.api_slotitem.Select(x => new SlotItem(x)))
				this.SlotItems.Add(x);

			this.RaiseSlotItemsChanged();
		}

		internal void RemoveFromShip(Ship ship)
		{
			ship.Slots.Where(x => x.Equipped).ForEach(x => this.SlotItems.Remove(x.Item));
			this.RaiseSlotItemsChanged();
		}

		internal void RemoveFromRemodel(kcsapi_remodel_slot source)
		{
			if (source.api_use_slot_id != null)
			{
				foreach (var id in source.api_use_slot_id)
					this.SlotItems.Remove(id);

				this.RaiseSlotItemsChanged();
			}
		}

		private void CreateItem(kcsapi_createitem source)
		{
			if (source.api_create_flag == 1 && source.api_slot_item != null)
				this.SlotItems.Add(new SlotItem(source.api_slot_item));

			this.RaiseSlotItemsChanged();
		}

		private void DestroyItem(SvData<kcsapi_destroyitem2> data)
		{
			try
			{
				foreach (var x in data.Request["api_slotitem_ids"].Split(',').Select(int.Parse))
					this.SlotItems.Remove(x);

				this.RaiseSlotItemsChanged();
			}
			catch { }
		}

		private void RemodelSlotItem(kcsapi_remodel_slot source)
		{
			if (source.api_after_slot == null) return;

			this.SlotItems[source.api_after_slot.api_id]
				?.Remodel(source.api_after_slot.api_level, source.api_after_slot.api_slotitem_id);
		}


		private void RaiseSlotItemsChanged()
		{
			this.RaisePropertyChanged(nameof(this.SlotItems));
			this.RaisePropertyChanged(nameof(this.SlotItemsCount));
		}
	}

}
