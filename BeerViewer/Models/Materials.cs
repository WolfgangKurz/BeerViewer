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

		#region DevelopmentMaterial Property
		private int _DevelopmentMaterial;
		public int DevelopmentMaterial
		{
			get { return this._DevelopmentMaterial; }
			private set
			{
				if (this._DevelopmentMaterial != value)
				{
					this._DevelopmentMaterial = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region RepairBucket Property
		private int _RepairBucket;
		public int RepairBucket
		{
			get { return this._RepairBucket; }
			private set
			{
				if (this._RepairBucket != value)
				{
					this._RepairBucket = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region InstantConstruction Property
		private int _InstantConstruction;
		public int InstantConstruction
		{
			get { return this._InstantConstruction; }
			private set
			{
				if (this._InstantConstruction != value)
				{
					this._InstantConstruction = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region ImprovementMaterial Property
		private int _ImprovementMaterial;
		public int ImprovementMaterial
		{
			get { return this._ImprovementMaterial; }
			set
			{
				if (this._ImprovementMaterial != value)
				{
					this._ImprovementMaterial = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion


		internal Materials()
		{
			var proxy = Proxy.Instance;

			proxy.Register<kcsapi_material[]>(Proxy.api_get_member_material, x => this.Update(x.Data));
			proxy.Register<kcsapi_charge>(Proxy.api_req_hokyu_charge, x => this.Update(x.Data.api_material));
			proxy.Register<kcsapi_destroyship>(Proxy.api_req_kousyou_destroyship, x => this.Update(x.Data.api_material));
			proxy.Register<kcsapi_destroyitem2>(Proxy.api_req_kousyou_destroyitem2, x => this.Update(x.Data.api_material));
		}

		internal void Update(kcsapi_material[] source)
		{
			if (source != null && source.Length >= 8)
			{
				this.Fuel = source[0].api_value;
				this.Ammo = source[1].api_value;
				this.Steel = source[2].api_value;
				this.Bauxite = source[3].api_value;
				this.DevelopmentMaterial = source[6].api_value;
				this.RepairBucket = source[5].api_value;
				this.InstantConstruction = source[4].api_value;
				this.ImprovementMaterial = source[7].api_value;
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
