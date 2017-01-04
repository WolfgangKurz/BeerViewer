namespace BeerViewer.Views.Contents
{
	partial class FleetsView
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
			this.fleetView = new BeerViewer.Views.Controls.FleetView();
			this.layoutFleets.SuspendLayout();
			this.SuspendLayout();
			// 
			// layoutFleets
			// 
			this.layoutFleets.AutoSize = true;
			this.layoutFleets.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.layoutFleets.Controls.Add(this.fleetView);
			this.layoutFleets.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.layoutFleets.Location = new System.Drawing.Point(0, 0);
			this.layoutFleets.Name = "layoutFleets";
			this.layoutFleets.Size = new System.Drawing.Size(30, 64);
			this.layoutFleets.TabIndex = 2;
			this.layoutFleets.WrapContents = false;
			// 
			// fleetView
			// 
			this.fleetView.AutoSize = true;
			this.fleetView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.fleetView.BackColor = System.Drawing.Color.Transparent;
			this.fleetView.Fleet = null;
			this.fleetView.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.fleetView.Location = new System.Drawing.Point(0, 0);
			this.fleetView.Margin = new System.Windows.Forms.Padding(0);
			this.fleetView.Name = "fleetView";
			this.fleetView.Padding = new System.Windows.Forms.Padding(0, 0, 0, 8);
			this.fleetView.Size = new System.Drawing.Size(30, 64);
			this.fleetView.TabIndex = 2;
			// 
			// FleetsView
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.layoutFleets);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "FleetsView";
			this.Size = new System.Drawing.Size(377, 438);
			this.layoutFleets.ResumeLayout(false);
			this.layoutFleets.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel layoutFleets;
		private Controls.FleetView fleetView;
	}
}
