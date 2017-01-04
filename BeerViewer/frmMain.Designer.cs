namespace BeerViewer
{
	partial class frmMain
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

		#region Windows Form 디자이너에서 생성한 코드

		/// <summary>
		/// 디자이너 지원에 필요한 메서드입니다. 
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
			this.contentContainer = new System.Windows.Forms.TableLayoutPanel();
			this.panelBrowser = new System.Windows.Forms.Panel();
			this.layoutTab = new System.Windows.Forms.TableLayoutPanel();
			this.layoutTabHost = new System.Windows.Forms.Panel();
			this.layoutTabRight = new System.Windows.Forms.FlowLayoutPanel();
			this.tabSettings = new System.Windows.Forms.Label();
			this.tabBattle = new System.Windows.Forms.Label();
			this.tabFleets = new System.Windows.Forms.Label();
			this.tabGeneral = new System.Windows.Forms.Label();
			this.contentBattle = new BeerViewer.Views.Contents.BattleView();
			this.contentFleets = new BeerViewer.Views.Contents.FleetsView();
			this.contentGeneral = new BeerViewer.Views.Contents.GeneralView();
			this.contentSettings = new BeerViewer.Views.Contents.SettingsView();
			this.btnRefresh = new BeerViewer.Views.Controls.FlatButton();
			this.btnMute = new BeerViewer.Views.Controls.FlatButton();
			this.btnScreenshot = new BeerViewer.Views.Controls.FlatButton();
			this.browserMain = new System.Windows.Forms.WebBrowser();
			this.contentContainer.SuspendLayout();
			this.panelBrowser.SuspendLayout();
			this.layoutTab.SuspendLayout();
			this.layoutTabHost.SuspendLayout();
			this.layoutTabRight.SuspendLayout();
			this.SuspendLayout();
			// 
			// contentContainer
			// 
			this.contentContainer.ColumnCount = 2;
			this.contentContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.contentContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.contentContainer.Controls.Add(this.contentBattle, 0, 1);
			this.contentContainer.Controls.Add(this.contentFleets, 1, 0);
			this.contentContainer.Controls.Add(this.contentGeneral, 0, 0);
			this.contentContainer.Controls.Add(this.contentSettings, 1, 1);
			this.contentContainer.Location = new System.Drawing.Point(80, 40);
			this.contentContainer.Name = "contentContainer";
			this.contentContainer.RowCount = 2;
			this.contentContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.contentContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.contentContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.contentContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.contentContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.contentContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.contentContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.contentContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.contentContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.contentContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.contentContainer.Size = new System.Drawing.Size(640, 383);
			this.contentContainer.TabIndex = 3;
			this.contentContainer.Visible = false;
			// 
			// panelBrowser
			// 
			this.panelBrowser.AutoSize = true;
			this.panelBrowser.Controls.Add(this.browserMain);
			this.panelBrowser.Dock = System.Windows.Forms.DockStyle.Left;
			this.panelBrowser.Location = new System.Drawing.Point(0, 0);
			this.panelBrowser.Name = "panelBrowser";
			this.panelBrowser.Size = new System.Drawing.Size(800, 681);
			this.panelBrowser.TabIndex = 14;
			// 
			// layoutTab
			// 
			this.layoutTab.AutoScroll = true;
			this.layoutTab.BackColor = System.Drawing.Color.Transparent;
			this.layoutTab.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.layoutTab.ColumnCount = 1;
			this.layoutTab.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.layoutTab.Controls.Add(this.layoutTabHost, 0, 0);
			this.layoutTab.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutTab.Location = new System.Drawing.Point(800, 0);
			this.layoutTab.Margin = new System.Windows.Forms.Padding(0);
			this.layoutTab.Name = "layoutTab";
			this.layoutTab.RowCount = 2;
			this.layoutTab.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
			this.layoutTab.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.layoutTab.Size = new System.Drawing.Size(464, 681);
			this.layoutTab.TabIndex = 15;
			// 
			// layoutTabHost
			// 
			this.layoutTabHost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
			this.layoutTabHost.Controls.Add(this.layoutTabRight);
			this.layoutTabHost.Controls.Add(this.tabSettings);
			this.layoutTabHost.Controls.Add(this.tabBattle);
			this.layoutTabHost.Controls.Add(this.tabFleets);
			this.layoutTabHost.Controls.Add(this.tabGeneral);
			this.layoutTabHost.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutTabHost.Location = new System.Drawing.Point(0, 0);
			this.layoutTabHost.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
			this.layoutTabHost.Name = "layoutTabHost";
			this.layoutTabHost.Size = new System.Drawing.Size(464, 36);
			this.layoutTabHost.TabIndex = 3;
			// 
			// layoutTabRight
			// 
			this.layoutTabRight.AutoSize = true;
			this.layoutTabRight.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.layoutTabRight.BackColor = System.Drawing.Color.Transparent;
			this.layoutTabRight.Controls.Add(this.btnRefresh);
			this.layoutTabRight.Controls.Add(this.btnMute);
			this.layoutTabRight.Controls.Add(this.btnScreenshot);
			this.layoutTabRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.layoutTabRight.Location = new System.Drawing.Point(344, 0);
			this.layoutTabRight.Margin = new System.Windows.Forms.Padding(0);
			this.layoutTabRight.Name = "layoutTabRight";
			this.layoutTabRight.Padding = new System.Windows.Forms.Padding(2);
			this.layoutTabRight.Size = new System.Drawing.Size(120, 36);
			this.layoutTabRight.TabIndex = 11;
			this.layoutTabRight.WrapContents = false;
			// 
			// tabSettings
			// 
			this.tabSettings.BackColor = System.Drawing.Color.Transparent;
			this.tabSettings.Dock = System.Windows.Forms.DockStyle.Left;
			this.tabSettings.Font = new System.Drawing.Font("맑은 고딕", 10F);
			this.tabSettings.ForeColor = System.Drawing.Color.White;
			this.tabSettings.Location = new System.Drawing.Point(240, 0);
			this.tabSettings.Margin = new System.Windows.Forms.Padding(0);
			this.tabSettings.Name = "tabSettings";
			this.tabSettings.Size = new System.Drawing.Size(64, 36);
			this.tabSettings.TabIndex = 10;
			this.tabSettings.Text = "설정";
			this.tabSettings.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// tabBattle
			// 
			this.tabBattle.BackColor = System.Drawing.Color.Transparent;
			this.tabBattle.Dock = System.Windows.Forms.DockStyle.Left;
			this.tabBattle.Font = new System.Drawing.Font("맑은 고딕", 10F);
			this.tabBattle.ForeColor = System.Drawing.Color.White;
			this.tabBattle.Location = new System.Drawing.Point(152, 0);
			this.tabBattle.Margin = new System.Windows.Forms.Padding(0);
			this.tabBattle.Name = "tabBattle";
			this.tabBattle.Size = new System.Drawing.Size(88, 36);
			this.tabBattle.TabIndex = 9;
			this.tabBattle.Text = "전투 정보";
			this.tabBattle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// tabFleets
			// 
			this.tabFleets.BackColor = System.Drawing.Color.Transparent;
			this.tabFleets.Dock = System.Windows.Forms.DockStyle.Left;
			this.tabFleets.Font = new System.Drawing.Font("맑은 고딕", 10F);
			this.tabFleets.ForeColor = System.Drawing.Color.White;
			this.tabFleets.Location = new System.Drawing.Point(64, 0);
			this.tabFleets.Margin = new System.Windows.Forms.Padding(0);
			this.tabFleets.Name = "tabFleets";
			this.tabFleets.Size = new System.Drawing.Size(88, 36);
			this.tabFleets.TabIndex = 8;
			this.tabFleets.Text = "함대 정보";
			this.tabFleets.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// tabGeneral
			// 
			this.tabGeneral.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(117)))), ((int)(((byte)(142)))));
			this.tabGeneral.Dock = System.Windows.Forms.DockStyle.Left;
			this.tabGeneral.Font = new System.Drawing.Font("맑은 고딕", 10F);
			this.tabGeneral.ForeColor = System.Drawing.Color.White;
			this.tabGeneral.Location = new System.Drawing.Point(0, 0);
			this.tabGeneral.Margin = new System.Windows.Forms.Padding(0);
			this.tabGeneral.Name = "tabGeneral";
			this.tabGeneral.Size = new System.Drawing.Size(64, 36);
			this.tabGeneral.TabIndex = 3;
			this.tabGeneral.Text = "종합";
			this.tabGeneral.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// contentBattle
			// 
			this.contentBattle.AutoScroll = true;
			this.contentBattle.AutoScrollMinSize = new System.Drawing.Size(400, 0);
			this.contentBattle.AutoSize = true;
			this.contentBattle.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.contentBattle.BackColor = System.Drawing.Color.Transparent;
			this.contentBattle.Dock = System.Windows.Forms.DockStyle.Fill;
			this.contentBattle.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.contentBattle.ForeColor = System.Drawing.Color.White;
			this.contentBattle.Location = new System.Drawing.Point(0, 191);
			this.contentBattle.Margin = new System.Windows.Forms.Padding(0);
			this.contentBattle.Name = "contentBattle";
			this.contentBattle.Size = new System.Drawing.Size(320, 192);
			this.contentBattle.TabIndex = 19;
			// 
			// contentFleets
			// 
			this.contentFleets.AutoScroll = true;
			this.contentFleets.AutoSize = true;
			this.contentFleets.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.contentFleets.BackColor = System.Drawing.Color.Transparent;
			this.contentFleets.Dock = System.Windows.Forms.DockStyle.Fill;
			this.contentFleets.Location = new System.Drawing.Point(320, 0);
			this.contentFleets.Margin = new System.Windows.Forms.Padding(0);
			this.contentFleets.Name = "contentFleets";
			this.contentFleets.Size = new System.Drawing.Size(320, 191);
			this.contentFleets.TabIndex = 17;
			// 
			// contentGeneral
			// 
			this.contentGeneral.AutoScroll = true;
			this.contentGeneral.AutoScrollMinSize = new System.Drawing.Size(340, 0);
			this.contentGeneral.AutoSize = true;
			this.contentGeneral.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.contentGeneral.BackColor = System.Drawing.Color.Transparent;
			this.contentGeneral.Dock = System.Windows.Forms.DockStyle.Fill;
			this.contentGeneral.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.contentGeneral.Location = new System.Drawing.Point(0, 0);
			this.contentGeneral.Margin = new System.Windows.Forms.Padding(0);
			this.contentGeneral.Name = "contentGeneral";
			this.contentGeneral.Size = new System.Drawing.Size(320, 191);
			this.contentGeneral.TabIndex = 8;
			// 
			// contentSettings
			// 
			this.contentSettings.AutoScroll = true;
			this.contentSettings.BackColor = System.Drawing.Color.Transparent;
			this.contentSettings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.contentSettings.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.contentSettings.ForeColor = System.Drawing.Color.White;
			this.contentSettings.Location = new System.Drawing.Point(320, 191);
			this.contentSettings.Margin = new System.Windows.Forms.Padding(0);
			this.contentSettings.Name = "contentSettings";
			this.contentSettings.Size = new System.Drawing.Size(320, 192);
			this.contentSettings.TabIndex = 20;
			// 
			// btnRefresh
			// 
			this.btnRefresh.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(39)))), ((int)(((byte)(47)))));
			this.btnRefresh.BackgroundImage = global::BeerViewer.Properties.Resources.Refresh;
			this.btnRefresh.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(83)))), ((int)(((byte)(83)))));
			this.btnRefresh.DownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(117)))), ((int)(((byte)(142)))));
			this.btnRefresh.DownBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(71)))), ((int)(((byte)(83)))));
			this.btnRefresh.Font = new System.Drawing.Font("맑은 고딕", 10F);
			this.btnRefresh.ForeColor = System.Drawing.Color.White;
			this.btnRefresh.Location = new System.Drawing.Point(6, 2);
			this.btnRefresh.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
			this.btnRefresh.Name = "btnRefresh";
			this.btnRefresh.OverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(89)))), ((int)(((byte)(89)))));
			this.btnRefresh.OverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(71)))), ((int)(((byte)(83)))));
			this.btnRefresh.Size = new System.Drawing.Size(32, 32);
			this.btnRefresh.TabIndex = 16;
			this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
			// 
			// btnMute
			// 
			this.btnMute.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.btnMute.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(39)))), ((int)(((byte)(47)))));
			this.btnMute.BackgroundImage = global::BeerViewer.Properties.Resources.Volume;
			this.btnMute.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(83)))), ((int)(((byte)(83)))));
			this.btnMute.DownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(117)))), ((int)(((byte)(142)))));
			this.btnMute.DownBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(71)))), ((int)(((byte)(83)))));
			this.btnMute.Font = new System.Drawing.Font("맑은 고딕", 10F);
			this.btnMute.ForeColor = System.Drawing.Color.White;
			this.btnMute.Location = new System.Drawing.Point(42, 2);
			this.btnMute.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
			this.btnMute.Name = "btnMute";
			this.btnMute.OverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(89)))), ((int)(((byte)(89)))));
			this.btnMute.OverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(71)))), ((int)(((byte)(83)))));
			this.btnMute.Size = new System.Drawing.Size(36, 32);
			this.btnMute.TabIndex = 15;
			this.btnMute.Click += new System.EventHandler(this.btnMute_Click);
			// 
			// btnScreenshot
			// 
			this.btnScreenshot.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.btnScreenshot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(39)))), ((int)(((byte)(47)))));
			this.btnScreenshot.BackgroundImage = global::BeerViewer.Properties.Resources.Screenshot;
			this.btnScreenshot.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(83)))), ((int)(((byte)(83)))));
			this.btnScreenshot.DownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(117)))), ((int)(((byte)(142)))));
			this.btnScreenshot.DownBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(71)))), ((int)(((byte)(83)))));
			this.btnScreenshot.Font = new System.Drawing.Font("맑은 고딕", 10F);
			this.btnScreenshot.ForeColor = System.Drawing.Color.White;
			this.btnScreenshot.Location = new System.Drawing.Point(82, 2);
			this.btnScreenshot.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
			this.btnScreenshot.Name = "btnScreenshot";
			this.btnScreenshot.OverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(89)))), ((int)(((byte)(89)))));
			this.btnScreenshot.OverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(71)))), ((int)(((byte)(83)))));
			this.btnScreenshot.Size = new System.Drawing.Size(36, 32);
			this.btnScreenshot.TabIndex = 14;
			this.btnScreenshot.Click += new System.EventHandler(this.btnScreenshot_Click);
			// 
			// browserMain
			// 
			this.browserMain.AllowWebBrowserDrop = false;
			this.browserMain.IsWebBrowserContextMenuEnabled = false;
			this.browserMain.Location = new System.Drawing.Point(0, 0);
			this.browserMain.Margin = new System.Windows.Forms.Padding(0);
			this.browserMain.MinimumSize = new System.Drawing.Size(20, 20);
			this.browserMain.Name = "browserMain";
			this.browserMain.ScriptErrorsSuppressed = true;
			this.browserMain.Size = new System.Drawing.Size(800, 480);
			this.browserMain.TabIndex = 15;
			this.browserMain.WebBrowserShortcutsEnabled = false;
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(72)))));
			this.ClientSize = new System.Drawing.Size(1264, 681);
			this.Controls.Add(this.contentContainer);
			this.Controls.Add(this.layoutTab);
			this.Controls.Add(this.panelBrowser);
			this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.ForeColor = System.Drawing.Color.White;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MinimumSize = new System.Drawing.Size(816, 519);
			this.Name = "frmMain";
			this.Text = "BeerViewer";
			this.contentContainer.ResumeLayout(false);
			this.contentContainer.PerformLayout();
			this.panelBrowser.ResumeLayout(false);
			this.layoutTab.ResumeLayout(false);
			this.layoutTabHost.ResumeLayout(false);
			this.layoutTabHost.PerformLayout();
			this.layoutTabRight.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.TableLayoutPanel contentContainer;
		private Views.Contents.GeneralView contentGeneral;
		private Views.Contents.FleetsView contentFleets;
		private Views.Contents.BattleView contentBattle;
		private Views.Contents.SettingsView contentSettings;
		private System.Windows.Forms.Panel panelBrowser;
		private System.Windows.Forms.WebBrowser browserMain;
		private System.Windows.Forms.TableLayoutPanel layoutTab;
		private System.Windows.Forms.Panel layoutTabHost;
		private System.Windows.Forms.Label tabSettings;
		private System.Windows.Forms.Label tabBattle;
		private System.Windows.Forms.Label tabFleets;
		private System.Windows.Forms.Label tabGeneral;
		private System.Windows.Forms.FlowLayoutPanel layoutTabRight;
		private Views.Controls.FlatButton btnRefresh;
		private Views.Controls.FlatButton btnMute;
		private Views.Controls.FlatButton btnScreenshot;
	}
}

