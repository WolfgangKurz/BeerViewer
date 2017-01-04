using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Core;
using BeerViewer.Models.Raw;

namespace BeerViewer.Models
{
	/// <summary>
	/// 모항 데이터
	/// </summary>
	public class Homeport : Notifier
	{
		/// <summary>
		/// 함대 데이터
		/// </summary>
		public Organization Organization { get; }

		/// <summary>
		/// 자원 데이터
		/// </summary>
		public Materials Materials { get; }

		/// <summary>
		/// 장비 데이터
		/// </summary>
		public Itemyard Itemyard { get; }

		/// <summary>
		/// 건조독 데이터
		/// </summary>
		public Dockyard Dockyard { get; }

		/// <summary>
		/// 입거독 데이터
		/// </summary>
		public Repairyard Repairyard { get; }

		/// <summary>
		/// 임무 데이터
		/// </summary>
		public Quests Quests { get; }


		#region Admiral 프로퍼티
		private Admiral _Admiral;
		public Admiral Admiral
		{
			get { return this._Admiral; }
			private set
			{
				if (this._Admiral != value)
				{
					this._Admiral = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		internal Homeport()
		{
			var proxy = Proxy.Instance;

			this.Materials = new Materials();
			this.Itemyard = new Itemyard(this);
			this.Organization = new Organization(this);
			this.Repairyard = new Repairyard(this);
			this.Dockyard = new Dockyard();
			this.Quests = new Quests();

			proxy.Register(Proxy.api_port, e => {
				var x = e.TryParse<kcsapi_port>();
				if (x == null) return;

				this.UpdateAdmiral(x.Data.api_basic);
				this.Organization.Update(x.Data.api_ship);
				this.Repairyard.Update(x.Data.api_ndock);
				this.Organization.Update(x.Data.api_deck_port);
				this.Organization.Combined = x.Data.api_combined_flag != 0;
				this.Materials.Update(x.Data.api_material);
			});
			proxy.Register(Proxy.api_get_member_basic, e =>
			{
				var x = e.TryParse<kcsapi_basic>();
				if (x == null) return;

				this.UpdateAdmiral(x.Data);
			});
			proxy.Register(Proxy.api_req_member_updatecomment, e =>
			{
				var x = e.TryParse();
				if (x == null) return;

				this.UpdateComment(x);
			});
		}


		internal void UpdateAdmiral(kcsapi_basic data)
		{
			this.Admiral = new Admiral(data);
		}

		private void UpdateComment(SvData data)
		{
			if (data == null || !data.IsSuccess) return;

			try
			{
				this.Admiral.Comment = data.Request["api_cmt"];
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("艦隊名の変更に失敗しました: {0}", ex);
			}
		}

		internal void StartConditionCount()
		{
			//Observable.Timer(TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(3))
		}
	}
}
