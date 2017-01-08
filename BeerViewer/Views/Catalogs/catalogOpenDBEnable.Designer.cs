namespace BeerViewer.Views.Catalogs
{
	partial class catalogOpenDBEnable
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(catalogOpenDBEnable));
			this.labelInfoText1 = new System.Windows.Forms.Label();
			this.labelInfoText2 = new System.Windows.Forms.Label();
			this.labelInfoText3 = new System.Windows.Forms.Label();
			this.btnNotUse = new BeerViewer.Views.Controls.FlatButton();
			this.btnUse = new BeerViewer.Views.Controls.FlatButton();
			this.SuspendLayout();
			// 
			// labelInfoText1
			// 
			this.labelInfoText1.AutoSize = true;
			this.labelInfoText1.Location = new System.Drawing.Point(12, 9);
			this.labelInfoText1.Name = "labelInfoText1";
			this.labelInfoText1.Size = new System.Drawing.Size(244, 21);
			this.labelInfoText1.TabIndex = 0;
			this.labelInfoText1.Text = "OpenDB 통계 전송 기능이 추가되었습니다.";
			this.labelInfoText1.UseCompatibleTextRendering = true;
			// 
			// labelInfoText2
			// 
			this.labelInfoText2.AutoSize = true;
			this.labelInfoText2.Location = new System.Drawing.Point(12, 30);
			this.labelInfoText2.Name = "labelInfoText2";
			this.labelInfoText2.Size = new System.Drawing.Size(272, 21);
			this.labelInfoText2.TabIndex = 1;
			this.labelInfoText2.Text = "서버에 데이터를 전송할지 여부를 설정해주세요.";
			this.labelInfoText2.UseCompatibleTextRendering = true;
			// 
			// labelInfoText3
			// 
			this.labelInfoText3.AutoSize = true;
			this.labelInfoText3.Location = new System.Drawing.Point(12, 51);
			this.labelInfoText3.Name = "labelInfoText3";
			this.labelInfoText3.Size = new System.Drawing.Size(351, 21);
			this.labelInfoText3.TabIndex = 2;
			this.labelInfoText3.Text = "설정한 이후에도 설정 탭에서 전송 여부를 변경할 수 있습니다.";
			this.labelInfoText3.UseCompatibleTextRendering = true;
			// 
			// btnNotUse
			// 
			this.btnNotUse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(39)))), ((int)(((byte)(47)))));
			this.btnNotUse.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(83)))), ((int)(((byte)(83)))));
			this.btnNotUse.DownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(117)))), ((int)(((byte)(142)))));
			this.btnNotUse.DownBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(71)))), ((int)(((byte)(83)))));
			this.btnNotUse.Font = new System.Drawing.Font("맑은 고딕", 9F);
			this.btnNotUse.ForeColor = System.Drawing.Color.White;
			this.btnNotUse.Location = new System.Drawing.Point(292, 82);
			this.btnNotUse.Margin = new System.Windows.Forms.Padding(2);
			this.btnNotUse.Name = "btnNotUse";
			this.btnNotUse.OverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(89)))), ((int)(((byte)(89)))));
			this.btnNotUse.OverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(71)))), ((int)(((byte)(83)))));
			this.btnNotUse.Size = new System.Drawing.Size(112, 28);
			this.btnNotUse.TabIndex = 4;
			this.btnNotUse.Text = "전송하지 않음";
			// 
			// btnUse
			// 
			this.btnUse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(39)))), ((int)(((byte)(47)))));
			this.btnUse.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(83)))), ((int)(((byte)(83)))));
			this.btnUse.DownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(117)))), ((int)(((byte)(142)))));
			this.btnUse.DownBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(71)))), ((int)(((byte)(83)))));
			this.btnUse.Font = new System.Drawing.Font("맑은 고딕", 9F);
			this.btnUse.ForeColor = System.Drawing.Color.White;
			this.btnUse.Location = new System.Drawing.Point(196, 82);
			this.btnUse.Margin = new System.Windows.Forms.Padding(2);
			this.btnUse.Name = "btnUse";
			this.btnUse.OverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(89)))), ((int)(((byte)(89)))));
			this.btnUse.OverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(71)))), ((int)(((byte)(83)))));
			this.btnUse.Size = new System.Drawing.Size(88, 28);
			this.btnUse.TabIndex = 5;
			this.btnUse.Text = "전송";
			// 
			// catalogOpenDBEnable
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(72)))));
			this.ClientSize = new System.Drawing.Size(415, 121);
			this.Controls.Add(this.btnUse);
			this.Controls.Add(this.btnNotUse);
			this.Controls.Add(this.labelInfoText3);
			this.Controls.Add(this.labelInfoText2);
			this.Controls.Add(this.labelInfoText1);
			this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.ForeColor = System.Drawing.Color.White;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MaximizeBox = false;
			this.Name = "catalogOpenDBEnable";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "BeerViewer - OpenDB";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelInfoText1;
		private System.Windows.Forms.Label labelInfoText2;
		private System.Windows.Forms.Label labelInfoText3;
		private Controls.FlatButton btnNotUse;
		private Controls.FlatButton btnUse;
	}
}