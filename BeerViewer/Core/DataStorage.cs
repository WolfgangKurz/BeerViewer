using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeerViewer.Models.Raw;

using BeerViewer.Models;

namespace BeerViewer.Core
{
	public class DataStorage : Notifier
	{
		public static DataStorage Instance { get; } = new DataStorage();

		/// <summary>
		/// 서버에서 받은 상수 데이터
		/// </summary>
		public Master Master { get; private set; }

		/// <summary>
		/// 모항 데이터
		/// </summary>
		public Homeport Homeport { get; private set; }


		#region Initialized 프로퍼티
		private bool _Initialized { get; set; }
		public bool Initialized
		{
			get { return this._Initialized; }
			set
			{
				if(this._Initialized!=value)
				{
					this._Initialized = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region IsInSortie 프로퍼티
		private bool _IsInSortie { get; set; }
		public bool IsInSortie
		{
			get { return this._IsInSortie; }
			set
			{
				if(this._IsInSortie != value)
				{
					this._IsInSortie = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion


		public void Initialize()
		{
			Proxy.Instance.Register(Proxy.api_start2, e =>
			{
				var x = e.TryParse<kcsapi_start2>();
				if (x == null) return;

				this.Master = new Master(x.Data);
			});
			Proxy.Instance.Register(Proxy.api_get_member_require_info, e =>
			{
				var x = e.TryParse<kcsapi_require_info>();
				if (x == null) return;

				this.Homeport = new Homeport();
				this.Homeport.UpdateAdmiral(x.Data.api_basic);
				this.Homeport.Itemyard.Update(x.Data.api_slot_item);
				this.Homeport.Dockyard.Update(x.Data.api_kdock);

				Initialized = true;
			});
		}
	}
}
