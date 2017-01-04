using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BeerViewer.Core;
using BeerViewer.Models;
using BeerViewer.Models.Raw;
using BeerViewer.Models.BattleInfo;
using MultipleDisposable = BeerViewer.Models.Raw.Disposable.MultipleDisposable;

namespace BeerViewer.Views.Contents
{
	public partial class BattleView : UserControl
	{
		private BattleData BattleData { get; }

		protected MultipleDisposable CompositeDisposable { get; }
		private BattleEndNotifier notifier { get; }

		#region IsCriticalNotiEnabled
		protected bool IsCriticalNotiEnabled
		{
			get { return this.notifier.CriticalEnabled; }
			set
			{
				if (this.notifier.CriticalEnabled != value)
					this.notifier.CriticalEnabled = value;
			}
		}
		#endregion

		#region IsNotifierEnabled 프로퍼티
		protected bool IsNotifierEnabled
		{
			get { return this.notifier.IsEnabled; }
			set
			{
				if (this.notifier.IsEnabled != value)
					this.notifier.IsEnabled = value;
			}
		}
		#endregion

		#region EnableColorChange
		protected bool EnableColorChange
		{
			get { return Settings.BattleInfo_EnableColorChange.Value; }
			set
			{
				if (Settings.BattleInfo_EnableColorChange.Value != value)
					Settings.BattleInfo_EnableColorChange.Value = value;
			}
		}
		#endregion

		#region DetailKouku
		protected bool DetailKouku
		{
			get { return Settings.BattleInfo_DetailKouku.Value; }
			set
			{
				if (Settings.BattleInfo_DetailKouku.Value != value)
					Settings.BattleInfo_DetailKouku.Value = value;
			}
		}
		#endregion

		#region AutoSelectTab
		protected bool AutoSelectTab
		{
			get { return Settings.BattleInfo_AutoSelectTab.Value; }
			set
			{
				if (Settings.BattleInfo_AutoSelectTab.Value != value)
					Settings.BattleInfo_AutoSelectTab.Value = value;
			}
		}
		#endregion


		public BattleView()
		{
			InitializeComponent();
			this.CompositeDisposable = new MultipleDisposable();

			this.BattleData = new BattleData();
			notifier = new BattleEndNotifier();

			this.CompositeDisposable.Add(new PropertyChangedEventListener(this.BattleData)
			{
				{
					nameof(this.BattleData.CellData),
					(_, __) => this.battleTopView.ApplyCell(this.BattleData.CellData)
				},
				{
					nameof(this.BattleData.Name),
					(_, __) => this.battleTopView.BattleType = this.BattleData.Name
				},
				{
					nameof(this.BattleData.UpdatedTime),
					(_, __) => this.battleTopView.UpdatedTime = this.BattleData.UpdatedTime
				},
				{
					nameof(this.BattleData.BattleSituation),
					(_, __) => this.battleTopView.BattleSituation = this.BattleData.BattleSituation
				},
				{
					nameof(this.BattleData.FriendAirSupremacy),
					(_, __) => this.battleTopView.AirSupremacy = this.BattleData.FriendAirSupremacy
				},
				{
					nameof(this.BattleData.DropShipName),
					(_, __) => this.battleTopView.DropShipName = this.BattleData.DropShipName
				},
				{
					nameof(this.BattleData.RankResult),
					(_, __) => this.battleTopView.ResultRank = this.BattleData.RankResult
				},
				{
					nameof(this.BattleData.AirRankResult),
					(_, __) => this.battleTopView.AirResultRank = this.BattleData.AirRankResult
				},
				{
					nameof(this.BattleData.FlareUsed),
					(_, __) => this.battleTopView.FlareUsed = this.BattleData.FlareUsed
				},
				{
					nameof(this.BattleData.NightReconScouted),
					(_, __) => this.battleTopView.NightReconScouted = this.BattleData.NightReconScouted
				},
				{
					nameof(this.BattleData.AntiAirFired),
					(_, __) => this.battleTopView.AntiAirFired = this.BattleData.AntiAirFired
				},
				{
					nameof(this.BattleData.SupportUsed),
					(_, __) => this.battleTopView.SupportUsed = this.BattleData.SupportUsed
				},

				{
					nameof(this.BattleData.FirstFleet),
					(_, __) => this.FirstFleet.FleetData = this.BattleData.FirstFleet
				},
				{
					nameof(this.BattleData.SecondFleet),
					(_, __) => this.SecondFleet.FleetData = this.BattleData.SecondFleet
				},
				{
					nameof(this.BattleData.SecondEnemies),
					(_, __) => this.SecondEnemies.FleetData = this.BattleData.SecondEnemies
				},
				{
					nameof(this.BattleData.Enemies),
					(_, __) => this.Enemies.FleetData = this.BattleData.Enemies
				},
				{
					nameof(this.BattleData.AirCombatResults),
					(_, __) =>
					{
						this.FirstFleet.AirCombatResults = this.BattleData?.AirCombatResults;
						this.SecondFleet.AirCombatResults = this.BattleData?.AirCombatResults;
						this.SecondEnemies.AirCombatResults = this.BattleData?.AirCombatResults;
						this.Enemies.AirCombatResults = this.BattleData?.AirCombatResults;
					}
				},
			});
		}
		~BattleView()
		{
			this.CompositeDisposable?.Dispose();
		}
	}
}
