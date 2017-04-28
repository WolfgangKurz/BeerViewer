using BeerViewer.Network;
using BeerViewer.Models.Raw;
using BeerViewer.Models.kcsapi;

namespace BeerViewer.Models
{
	public class Materials : Notifier
	{
		#region Fuel Property
		private int _Fuel;
		public int Fuel
		{
			get { return this._Fuel; }
			private set
			{
				if (this._Fuel != value)
				{
					this._Fuel = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region Ammo Property
		private int _Ammo;
		public int Ammo
		{
			get { return this._Ammo; }
			private set
			{
				if (this._Ammo != value)
				{
					this._Ammo = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region Steel Property
		private int _Steel;
		public int Steel
		{
			get { return this._Steel; }
			private set
			{
				if (this._Steel != value)
				{
					this._Steel = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region Bauxite Property
		private int _Bauxite;
		public int Bauxite
		{
			get { return this._Bauxite; }
			private set
			{
				if (this._Bauxite != value)
				{
					this._Bauxite = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region DevMaterials Property
		private int _DevMaterials;
		public int DevMaterials
		{
			get { return this._DevMaterials; }
			private set
			{
				if (this._DevMaterials != value)
				{
					this._DevMaterials = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region RepairBuckets Property
		private int _RepairBuckets;
		public int RepairBuckets
		{
			get { return this._RepairBuckets; }
			private set
			{
				if (this._RepairBuckets != value)
				{
					this._RepairBuckets = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("Bucket");
				}
			}
		}
		#endregion

		#region InstantBuildMaterials Property
		private int _InstantBuildMaterials;
		public int BuildMaterials
		{
			get { return this._InstantBuildMaterials; }
			private set
			{
				if (this._InstantBuildMaterials != value)
				{
					this._InstantBuildMaterials = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region ImprovementMaterials Property
		private int _ImprovementMaterials;
		public int ImproveMaterials
		{
			get { return this._ImprovementMaterials; }
			set
			{
				if (this._ImprovementMaterials != value)
				{
					this._ImprovementMaterials = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion


		internal Materials()
		{
			var proxy = Proxy.Instance;

			proxy.Register<kcsapi_material[]>(Proxy.api_get_member_material, x => this.Update(x.api_data));
			proxy.Register<kcsapi_charge>(Proxy.api_req_hokyu_charge, x => this.Update(x.api_data.api_material));
			proxy.Register<kcsapi_destroyship>(Proxy.api_req_kousyou_destroyship, x => this.Update(x.api_data.api_material));
			proxy.Register<kcsapi_destroyitem2>(Proxy.api_req_kousyou_destroyitem2, x => this.Update(x.api_data.api_material));
		}

		internal void Update(kcsapi_material[] source)
		{
			if (source != null && source.Length >= 8)
			{
				this.Fuel = source[0].api_value;
				this.Ammo = source[1].api_value;
				this.Steel = source[2].api_value;
				this.Bauxite = source[3].api_value;
				this.DevMaterials = source[6].api_value;
				this.RepairBuckets = source[5].api_value;
				this.BuildMaterials = source[4].api_value;
				this.ImproveMaterials = source[7].api_value;
			}
		}
		private void Update(int[] source)
		{
			if (source != null && source.Length == 4)
			{
				this.Fuel = source[0];
				this.Ammo = source[1];
				this.Steel = source[2];
				this.Bauxite = source[3];
			}
		}
	}
}
