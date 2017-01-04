namespace BeerViewer.Views.Contents
{
	partial class BattleView
	{
		/// <summary> 
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// 사용 중인 모든 리소스를 정리합니다.
		/// </summary>
		/// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region 구성 요소 디자이너에서 생성한 코드

		/// <summary> 
		/// 디자이너 지원에 필요한 메서드입니다. 
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
		/// </summary>
		private void InitializeComponent()
		{
			this.layoutFleets = new System.Windows.Forms.FlowLayoutPanel();
			this.FirstFleet = new BeerViewer.Views.Controls.BattleFleetView();
			this.SecondFleet = new BeerViewer.Views.Controls.BattleFleetView();
			this.SecondEnemies = new BeerViewer.Views.Controls.BattleFleetView();
			this.Enemies = new BeerViewer.Views.Controls.BattleFleetView();
			this.battleTopView = new BeerViewer.Views.Controls.BattleTopView();
			this.layoutFleets.SuspendLayout();
			this.SuspendLayout();
			// 
			// layoutFleets
			// 
			this.layoutFleets.AutoSize = true;
			this.layoutFleets.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.layoutFleets.Controls.Add(this.FirstFleet);
			this.layoutFleets.Controls.Add(this.SecondFleet);
			this.layoutFleets.Controls.Add(this.SecondEnemies);
			this.layoutFleets.Controls.Add(this.Enemies);
			this.layoutFleets.Dock = System.Windows.Forms.DockStyle.Top;
			this.layoutFleets.Location = new System.Drawing.Point(0, 115);
			this.layoutFleets.Name = "layoutFleets";
			this.layoutFleets.Size = new System.Drawing.Size(530, 0);
			this.layoutFleets.TabIndex = 39;
			this.layoutFleets.WrapContents = false;
			// 
			// FirstFleet
			// 
			this.FirstFleet.AirCombatResults = null;
			this.FirstFleet.AutoSize = true;
			this.FirstFleet.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.FirstFleet.BackColor = System.Drawing.Color.Transparent;
			this.FirstFleet.FleetData = null;
			this.FirstFleet.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.FirstFleet.ForeColor = System.Drawing.Color.White;
			this.FirstFleet.Location = new System.Drawing.Point(0, 0);
			this.FirstFleet.Margin = new System.Windows.Forms.Padding(0);
			this.FirstFleet.Name = "FirstFleet";
			this.FirstFleet.Padding = new System.Windows.Forms.Padding(2);
			this.FirstFleet.Size = new System.Drawing.Size(0, 0);
			this.FirstFleet.TabIndex = 0;
			// 
			// SecondFleet
			// 
			this.SecondFleet.AirCombatResults = null;
			this.SecondFleet.AutoSize = true;
			this.SecondFleet.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.SecondFleet.BackColor = System.Drawing.Color.Transparent;
			this.SecondFleet.FleetData = null;
			this.SecondFleet.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.SecondFleet.ForeColor = System.Drawing.Color.White;
			this.SecondFleet.Location = new System.Drawing.Point(0, 0);
			this.SecondFleet.Margin = new System.Windows.Forms.Padding(0);
			this.SecondFleet.Name = "SecondFleet";
			this.SecondFleet.Padding = new System.Windows.Forms.Padding(2);
			this.SecondFleet.Size = new System.Drawing.Size(0, 0);
			this.SecondFleet.TabIndex = 1;
			// 
			// SecondEnemies
			// 
			this.SecondEnemies.AirCombatResults = null;
			this.SecondEnemies.AutoSize = true;
			this.SecondEnemies.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.SecondEnemies.BackColor = System.Drawing.Color.Transparent;
			this.SecondEnemies.FleetData = null;
			this.SecondEnemies.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.SecondEnemies.ForeColor = System.Drawing.Color.White;
			this.SecondEnemies.Location = new System.Drawing.Point(0, 0);
			this.SecondEnemies.Margin = new System.Windows.Forms.Padding(0);
			this.SecondEnemies.Name = "SecondEnemies";
			this.SecondEnemies.Padding = new System.Windows.Forms.Padding(2);
			this.SecondEnemies.Size = new System.Drawing.Size(0, 0);
			this.SecondEnemies.TabIndex = 2;
			// 
			// Enemies
			// 
			this.Enemies.AirCombatResults = null;
			this.Enemies.AutoSize = true;
			this.Enemies.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Enemies.BackColor = System.Drawing.Color.Transparent;
			this.Enemies.FleetData = null;
			this.Enemies.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.Enemies.ForeColor = System.Drawing.Color.White;
			this.Enemies.Location = new System.Drawing.Point(0, 0);
			this.Enemies.Margin = new System.Windows.Forms.Padding(0);
			this.Enemies.Name = "Enemies";
			this.Enemies.Padding = new System.Windows.Forms.Padding(2);
			this.Enemies.Size = new System.Drawing.Size(0, 0);
			this.Enemies.TabIndex = 3;
			// 
			// battleTopView
			// 
			this.battleTopView.AirResultRank = BeerViewer.Models.BattleInfo.BattleRank.없음;
			this.battleTopView.AirSupremacy = BeerViewer.Models.BattleInfo.AirSupremacy.항공전없음;
			this.battleTopView.AntiAirFired = BeerViewer.Models.BattleInfo.AirFireFlag.Unset;
			this.battleTopView.AutoSize = true;
			this.battleTopView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.battleTopView.BackColor = System.Drawing.Color.Transparent;
			this.battleTopView.BattleSituation = BeerViewer.Models.BattleInfo.BattleSituation.없음;
			this.battleTopView.BattleType = null;
			this.battleTopView.Dock = System.Windows.Forms.DockStyle.Top;
			this.battleTopView.DropShipName = null;
			this.battleTopView.FlareUsed = BeerViewer.Models.BattleInfo.UsedFlag.Unset;
			this.battleTopView.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.battleTopView.Location = new System.Drawing.Point(0, 0);
			this.battleTopView.Name = "battleTopView";
			this.battleTopView.NightReconScouted = BeerViewer.Models.BattleInfo.UsedFlag.Unset;
			this.battleTopView.ResultRank = BeerViewer.Models.BattleInfo.BattleRank.없음;
			this.battleTopView.Size = new System.Drawing.Size(530, 115);
			this.battleTopView.SupportUsed = BeerViewer.Models.BattleInfo.UsedSupport.Unset;
			this.battleTopView.TabIndex = 38;
			this.battleTopView.UpdatedTime = new System.DateTimeOffset(1, 1, 1, 0, 0, 0, 0, System.TimeSpan.Parse("00:00:00"));
			// 
			// BattleView
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.AutoScroll = true;
			this.AutoScrollMinSize = new System.Drawing.Size(400, 0);
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.layoutFleets);
			this.Controls.Add(this.battleTopView);
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.ForeColor = System.Drawing.Color.White;
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "BattleView";
			this.Size = new System.Drawing.Size(530, 334);
			this.layoutFleets.ResumeLayout(false);
			this.layoutFleets.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Controls.BattleTopView battleTopView;
		private System.Windows.Forms.FlowLayoutPanel layoutFleets;
		private Controls.BattleFleetView FirstFleet;
		private Controls.BattleFleetView SecondFleet;
		private Controls.BattleFleetView SecondEnemies;
		private Controls.BattleFleetView Enemies;
	}
}
