namespace BeerViewer.Views.Controls
{
	partial class ShipListTable
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
			this.panelDataView = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// panelDataView
			// 
			this.panelDataView.AutoScroll = true;
			this.panelDataView.Location = new System.Drawing.Point(85, 53);
			this.panelDataView.Margin = new System.Windows.Forms.Padding(0);
			this.panelDataView.Name = "panelDataView";
			this.panelDataView.Size = new System.Drawing.Size(333, 195);
			this.panelDataView.TabIndex = 0;
			// 
			// ShipListTable
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.panelDataView);
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "ShipListTable";
			this.Size = new System.Drawing.Size(451, 263);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelDataView;
	}
}
