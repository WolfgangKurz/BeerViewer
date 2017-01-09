namespace BeerViewer.Views.Contents
{
	partial class GeneralView
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
			BeerViewer.Views.Controls.GeneralTableView.HeaderValue headerValue16 = new BeerViewer.Views.Controls.GeneralTableView.HeaderValue();
			BeerViewer.Views.Controls.GeneralTableView.HeaderValue headerValue17 = new BeerViewer.Views.Controls.GeneralTableView.HeaderValue();
			BeerViewer.Views.Controls.GeneralTableView.HeaderValue headerValue18 = new BeerViewer.Views.Controls.GeneralTableView.HeaderValue();
			BeerViewer.Views.Controls.GeneralTableView.HeaderValue headerValue19 = new BeerViewer.Views.Controls.GeneralTableView.HeaderValue();
			BeerViewer.Views.Controls.GeneralTableView.HeaderValue headerValue20 = new BeerViewer.Views.Controls.GeneralTableView.HeaderValue();
			BeerViewer.Views.Controls.GeneralTableView.HeaderValue headerValue21 = new BeerViewer.Views.Controls.GeneralTableView.HeaderValue();
			BeerViewer.Views.Controls.GeneralTableView.HeaderValue headerValue22 = new BeerViewer.Views.Controls.GeneralTableView.HeaderValue();
			BeerViewer.Views.Controls.GeneralTableView.HeaderValue headerValue23 = new BeerViewer.Views.Controls.GeneralTableView.HeaderValue();
			BeerViewer.Views.Controls.GeneralTableView.HeaderValue headerValue24 = new BeerViewer.Views.Controls.GeneralTableView.HeaderValue();
			BeerViewer.Views.Controls.GeneralTableView.HeaderValue headerValue25 = new BeerViewer.Views.Controls.GeneralTableView.HeaderValue();
			BeerViewer.Views.Controls.GeneralTableView.HeaderValue headerValue26 = new BeerViewer.Views.Controls.GeneralTableView.HeaderValue();
			BeerViewer.Views.Controls.GeneralTableView.HeaderValue headerValue27 = new BeerViewer.Views.Controls.GeneralTableView.HeaderValue();
			BeerViewer.Views.Controls.GeneralTableView.HeaderValue headerValue28 = new BeerViewer.Views.Controls.GeneralTableView.HeaderValue();
			BeerViewer.Views.Controls.GeneralTableView.HeaderValue headerValue29 = new BeerViewer.Views.Controls.GeneralTableView.HeaderValue();
			BeerViewer.Views.Controls.GeneralTableView.HeaderValue headerValue30 = new BeerViewer.Views.Controls.GeneralTableView.HeaderValue();
			this.layoutHomeportInfo = new System.Windows.Forms.FlowLayoutPanel();
			this.layoutButtons = new System.Windows.Forms.FlowLayoutPanel();
			this.btnShipList = new BeerViewer.Views.Controls.FlatButton();
			this.btnItemList = new BeerViewer.Views.Controls.FlatButton();
			this.btnCalculator = new BeerViewer.Views.Controls.FlatButton();
			this.btnAkashi = new BeerViewer.Views.Controls.FlatButton();
			this.layoutResources = new System.Windows.Forms.FlowLayoutPanel();
			this.comboResources1 = new BeerViewer.Views.Controls.FlatComboBox();
			this.comboResources2 = new BeerViewer.Views.Controls.FlatComboBox();
			this.layoutCountInfo = new System.Windows.Forms.FlowLayoutPanel();
			this.labelShipCount = new System.Windows.Forms.Label();
			this.labelSlotitemCount = new System.Windows.Forms.Label();
			this.tableHQRecord = new BeerViewer.Views.Controls.GeneralTableView();
			this.listQuests = new BeerViewer.Views.Controls.QuestsView();
			this.tableBuild = new BeerViewer.Views.Controls.GeneralTableView();
			this.tableRepair = new BeerViewer.Views.Controls.GeneralTableView();
			this.tableFleet = new BeerViewer.Views.Controls.GeneralTableView();
			this.layoutHomeportInfo.SuspendLayout();
			this.layoutButtons.SuspendLayout();
			this.layoutResources.SuspendLayout();
			this.layoutCountInfo.SuspendLayout();
			this.SuspendLayout();
			// 
			// layoutHomeportInfo
			// 
			this.layoutHomeportInfo.AutoSize = true;
			this.layoutHomeportInfo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.layoutHomeportInfo.Controls.Add(this.layoutButtons);
			this.layoutHomeportInfo.Controls.Add(this.layoutResources);
			this.layoutHomeportInfo.Controls.Add(this.layoutCountInfo);
			this.layoutHomeportInfo.Dock = System.Windows.Forms.DockStyle.Top;
			this.layoutHomeportInfo.Location = new System.Drawing.Point(0, 0);
			this.layoutHomeportInfo.Name = "layoutHomeportInfo";
			this.layoutHomeportInfo.Size = new System.Drawing.Size(384, 99);
			this.layoutHomeportInfo.TabIndex = 171;
			// 
			// layoutButtons
			// 
			this.layoutButtons.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.layoutButtons.AutoSize = true;
			this.layoutButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.layoutButtons.Controls.Add(this.btnShipList);
			this.layoutButtons.Controls.Add(this.btnItemList);
			this.layoutButtons.Controls.Add(this.btnCalculator);
			this.layoutButtons.Controls.Add(this.btnAkashi);
			this.layoutButtons.Location = new System.Drawing.Point(0, 0);
			this.layoutButtons.Margin = new System.Windows.Forms.Padding(0);
			this.layoutButtons.Name = "layoutButtons";
			this.layoutButtons.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.layoutButtons.Size = new System.Drawing.Size(332, 32);
			this.layoutButtons.TabIndex = 165;
			// 
			// btnShipList
			// 
			this.btnShipList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(39)))), ((int)(((byte)(47)))));
			this.btnShipList.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(83)))), ((int)(((byte)(83)))));
			this.btnShipList.DownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(117)))), ((int)(((byte)(142)))));
			this.btnShipList.DownBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(71)))), ((int)(((byte)(83)))));
			this.btnShipList.Enabled = false;
			this.btnShipList.Font = new System.Drawing.Font("맑은 고딕", 9F);
			this.btnShipList.ForeColor = System.Drawing.Color.White;
			this.btnShipList.Location = new System.Drawing.Point(8, 2);
			this.btnShipList.Margin = new System.Windows.Forms.Padding(2);
			this.btnShipList.Name = "btnShipList";
			this.btnShipList.OverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(89)))), ((int)(((byte)(89)))));
			this.btnShipList.OverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(71)))), ((int)(((byte)(83)))));
			this.btnShipList.Size = new System.Drawing.Size(76, 28);
			this.btnShipList.TabIndex = 3;
			this.btnShipList.Text = "함선목록";
			this.btnShipList.Click += new System.EventHandler(this.btnShipList_Click);
			// 
			// btnItemList
			// 
			this.btnItemList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(39)))), ((int)(((byte)(47)))));
			this.btnItemList.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(83)))), ((int)(((byte)(83)))));
			this.btnItemList.DownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(117)))), ((int)(((byte)(142)))));
			this.btnItemList.DownBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(71)))), ((int)(((byte)(83)))));
			this.btnItemList.Enabled = false;
			this.btnItemList.Font = new System.Drawing.Font("맑은 고딕", 9F);
			this.btnItemList.ForeColor = System.Drawing.Color.White;
			this.btnItemList.Location = new System.Drawing.Point(88, 2);
			this.btnItemList.Margin = new System.Windows.Forms.Padding(2);
			this.btnItemList.Name = "btnItemList";
			this.btnItemList.OverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(89)))), ((int)(((byte)(89)))));
			this.btnItemList.OverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(71)))), ((int)(((byte)(83)))));
			this.btnItemList.Size = new System.Drawing.Size(76, 28);
			this.btnItemList.TabIndex = 2;
			this.btnItemList.Text = "장비목록";
			this.btnItemList.Click += new System.EventHandler(this.btnItemList_Click);
			// 
			// btnCalculator
			// 
			this.btnCalculator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(39)))), ((int)(((byte)(47)))));
			this.btnCalculator.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(83)))), ((int)(((byte)(83)))));
			this.btnCalculator.DownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(117)))), ((int)(((byte)(142)))));
			this.btnCalculator.DownBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(71)))), ((int)(((byte)(83)))));
			this.btnCalculator.Font = new System.Drawing.Font("맑은 고딕", 9F);
			this.btnCalculator.ForeColor = System.Drawing.Color.White;
			this.btnCalculator.Location = new System.Drawing.Point(168, 2);
			this.btnCalculator.Margin = new System.Windows.Forms.Padding(2);
			this.btnCalculator.Name = "btnCalculator";
			this.btnCalculator.OverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(89)))), ((int)(((byte)(89)))));
			this.btnCalculator.OverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(71)))), ((int)(((byte)(83)))));
			this.btnCalculator.Size = new System.Drawing.Size(76, 28);
			this.btnCalculator.TabIndex = 0;
			this.btnCalculator.Text = "계산기";
			this.btnCalculator.Click += new System.EventHandler(this.btnCalculator_Click);
			// 
			// btnAkashi
			// 
			this.btnAkashi.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(39)))), ((int)(((byte)(47)))));
			this.btnAkashi.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(83)))), ((int)(((byte)(83)))));
			this.btnAkashi.DownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(117)))), ((int)(((byte)(142)))));
			this.btnAkashi.DownBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(71)))), ((int)(((byte)(83)))));
			this.btnAkashi.Font = new System.Drawing.Font("맑은 고딕", 9F);
			this.btnAkashi.ForeColor = System.Drawing.Color.White;
			this.btnAkashi.Location = new System.Drawing.Point(248, 2);
			this.btnAkashi.Margin = new System.Windows.Forms.Padding(2);
			this.btnAkashi.Name = "btnAkashi";
			this.btnAkashi.OverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(89)))), ((int)(((byte)(89)))));
			this.btnAkashi.OverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(71)))), ((int)(((byte)(83)))));
			this.btnAkashi.Size = new System.Drawing.Size(76, 28);
			this.btnAkashi.TabIndex = 1;
			this.btnAkashi.Text = "개수공창";
			this.btnAkashi.Visible = false;
			// 
			// layoutResources
			// 
			this.layoutResources.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.layoutResources.AutoSize = true;
			this.layoutResources.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.layoutResources.Controls.Add(this.comboResources1);
			this.layoutResources.Controls.Add(this.comboResources2);
			this.layoutResources.Location = new System.Drawing.Point(0, 32);
			this.layoutResources.Margin = new System.Windows.Forms.Padding(0);
			this.layoutResources.Name = "layoutResources";
			this.layoutResources.Padding = new System.Windows.Forms.Padding(4);
			this.layoutResources.Size = new System.Drawing.Size(304, 36);
			this.layoutResources.TabIndex = 150;
			// 
			// comboResources1
			// 
			this.comboResources1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(39)))), ((int)(((byte)(47)))));
			this.comboResources1.DropDownHeight = 2;
			this.comboResources1.FormattingEnabled = true;
			this.comboResources1.IntegralHeight = false;
			this.comboResources1.Location = new System.Drawing.Point(6, 6);
			this.comboResources1.Margin = new System.Windows.Forms.Padding(2);
			this.comboResources1.Name = "comboResources1";
			this.comboResources1.Size = new System.Drawing.Size(144, 24);
			this.comboResources1.TabIndex = 6;
			// 
			// comboResources2
			// 
			this.comboResources2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(39)))), ((int)(((byte)(47)))));
			this.comboResources2.DropDownHeight = 2;
			this.comboResources2.FormattingEnabled = true;
			this.comboResources2.IntegralHeight = false;
			this.comboResources2.Location = new System.Drawing.Point(154, 6);
			this.comboResources2.Margin = new System.Windows.Forms.Padding(2);
			this.comboResources2.Name = "comboResources2";
			this.comboResources2.Size = new System.Drawing.Size(144, 24);
			this.comboResources2.TabIndex = 7;
			// 
			// layoutCountInfo
			// 
			this.layoutCountInfo.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.layoutCountInfo.AutoSize = true;
			this.layoutCountInfo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.layoutCountInfo.Controls.Add(this.labelShipCount);
			this.layoutCountInfo.Controls.Add(this.labelSlotitemCount);
			this.layoutCountInfo.Location = new System.Drawing.Point(3, 71);
			this.layoutCountInfo.Name = "layoutCountInfo";
			this.layoutCountInfo.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this.layoutCountInfo.Size = new System.Drawing.Size(179, 25);
			this.layoutCountInfo.TabIndex = 151;
			// 
			// labelShipCount
			// 
			this.labelShipCount.AutoSize = true;
			this.labelShipCount.ForeColor = System.Drawing.Color.White;
			this.labelShipCount.Location = new System.Drawing.Point(0, 4);
			this.labelShipCount.Margin = new System.Windows.Forms.Padding(0);
			this.labelShipCount.Name = "labelShipCount";
			this.labelShipCount.Size = new System.Drawing.Size(92, 21);
			this.labelShipCount.TabIndex = 121;
			this.labelShipCount.Text = "소속칸무스: 0/0";
			this.labelShipCount.UseCompatibleTextRendering = true;
			// 
			// labelSlotitemCount
			// 
			this.labelSlotitemCount.AutoSize = true;
			this.labelSlotitemCount.ForeColor = System.Drawing.Color.White;
			this.labelSlotitemCount.Location = new System.Drawing.Point(100, 4);
			this.labelSlotitemCount.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
			this.labelSlotitemCount.Name = "labelSlotitemCount";
			this.labelSlotitemCount.Size = new System.Drawing.Size(79, 21);
			this.labelSlotitemCount.TabIndex = 122;
			this.labelSlotitemCount.Text = "보유장비: 0/0";
			this.labelSlotitemCount.UseCompatibleTextRendering = true;
			// 
			// tableHQRecord
			// 
			this.tableHQRecord.BackColor = System.Drawing.Color.Transparent;
			this.tableHQRecord.Dock = System.Windows.Forms.DockStyle.Top;
			this.tableHQRecord.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.tableHQRecord.ForeColor = System.Drawing.Color.White;
			this.tableHQRecord.Location = new System.Drawing.Point(0, 344);
			this.tableHQRecord.Margin = new System.Windows.Forms.Padding(0);
			this.tableHQRecord.Name = "tableHQRecord";
			this.tableHQRecord.Padding = new System.Windows.Forms.Padding(0, 0, 0, 8);
			this.tableHQRecord.Size = new System.Drawing.Size(384, 63);
			this.tableHQRecord.TabIndex = 176;
			headerValue16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
			headerValue16.ForeColor = System.Drawing.Color.White;
			headerValue16.Header = "이달";
			headerValue16.HeaderVisible = true;
			headerValue16.Value = "-";
			headerValue16.Visible = false;
			headerValue17.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
			headerValue17.ForeColor = System.Drawing.Color.White;
			headerValue17.Header = "오늘";
			headerValue17.HeaderVisible = true;
			headerValue17.Value = "-";
			headerValue17.Visible = false;
			headerValue18.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
			headerValue18.ForeColor = System.Drawing.Color.White;
			headerValue18.Header = "이번";
			headerValue18.HeaderVisible = true;
			headerValue18.Value = "-";
			headerValue18.Visible = false;
			this.tableHQRecord.TableCells = new BeerViewer.Views.Controls.GeneralTableView.HeaderValue[] {
        headerValue16,
        headerValue17,
        headerValue18};
			this.tableHQRecord.TableName = "전과";
			// 
			// listQuests
			// 
			this.listQuests.AutoSize = true;
			this.listQuests.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.listQuests.BackColor = System.Drawing.Color.Transparent;
			this.listQuests.Dock = System.Windows.Forms.DockStyle.Top;
			this.listQuests.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.listQuests.HeaderName = "임무";
			this.listQuests.Location = new System.Drawing.Point(0, 288);
			this.listQuests.Margin = new System.Windows.Forms.Padding(0);
			this.listQuests.MinimumSize = new System.Drawing.Size(0, 24);
			this.listQuests.Name = "listQuests";
			this.listQuests.Padding = new System.Windows.Forms.Padding(0, 0, 0, 8);
			this.listQuests.PlaceHolder = "게임 내 [임무(퀘스트)] 화면을 열어서, 임무 목록을 갱신 해 주십시오.";
			this.listQuests.Quests = null;
			this.listQuests.Size = new System.Drawing.Size(384, 56);
			this.listQuests.TabIndex = 175;
			// 
			// tableBuild
			// 
			this.tableBuild.BackColor = System.Drawing.Color.Transparent;
			this.tableBuild.Dock = System.Windows.Forms.DockStyle.Top;
			this.tableBuild.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.tableBuild.ForeColor = System.Drawing.Color.White;
			this.tableBuild.Location = new System.Drawing.Point(0, 225);
			this.tableBuild.Margin = new System.Windows.Forms.Padding(0);
			this.tableBuild.Name = "tableBuild";
			this.tableBuild.Padding = new System.Windows.Forms.Padding(0, 0, 0, 8);
			this.tableBuild.Size = new System.Drawing.Size(384, 63);
			this.tableBuild.TabIndex = 174;
			headerValue19.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
			headerValue19.ForeColor = System.Drawing.Color.White;
			headerValue19.Header = "-";
			headerValue19.HeaderVisible = true;
			headerValue19.Value = "-";
			headerValue19.Visible = false;
			headerValue20.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
			headerValue20.ForeColor = System.Drawing.Color.White;
			headerValue20.Header = "-";
			headerValue20.HeaderVisible = true;
			headerValue20.Value = "-";
			headerValue20.Visible = false;
			headerValue21.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
			headerValue21.ForeColor = System.Drawing.Color.White;
			headerValue21.Header = "-";
			headerValue21.HeaderVisible = true;
			headerValue21.Value = "-";
			headerValue21.Visible = false;
			headerValue22.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
			headerValue22.ForeColor = System.Drawing.Color.White;
			headerValue22.Header = "-";
			headerValue22.HeaderVisible = true;
			headerValue22.Value = "-";
			headerValue22.Visible = false;
			this.tableBuild.TableCells = new BeerViewer.Views.Controls.GeneralTableView.HeaderValue[] {
        headerValue19,
        headerValue20,
        headerValue21,
        headerValue22};
			this.tableBuild.TableName = "건조";
			// 
			// tableRepair
			// 
			this.tableRepair.BackColor = System.Drawing.Color.Transparent;
			this.tableRepair.Dock = System.Windows.Forms.DockStyle.Top;
			this.tableRepair.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.tableRepair.ForeColor = System.Drawing.Color.White;
			this.tableRepair.Location = new System.Drawing.Point(0, 162);
			this.tableRepair.Margin = new System.Windows.Forms.Padding(0);
			this.tableRepair.Name = "tableRepair";
			this.tableRepair.Padding = new System.Windows.Forms.Padding(0, 0, 0, 8);
			this.tableRepair.Size = new System.Drawing.Size(384, 63);
			this.tableRepair.TabIndex = 173;
			headerValue23.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
			headerValue23.ForeColor = System.Drawing.Color.White;
			headerValue23.Header = "-";
			headerValue23.HeaderVisible = true;
			headerValue23.Value = "-";
			headerValue23.Visible = false;
			headerValue24.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
			headerValue24.ForeColor = System.Drawing.Color.White;
			headerValue24.Header = "-";
			headerValue24.HeaderVisible = true;
			headerValue24.Value = "-";
			headerValue24.Visible = false;
			headerValue25.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
			headerValue25.ForeColor = System.Drawing.Color.White;
			headerValue25.Header = "-";
			headerValue25.HeaderVisible = true;
			headerValue25.Value = "-";
			headerValue25.Visible = false;
			headerValue26.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
			headerValue26.ForeColor = System.Drawing.Color.White;
			headerValue26.Header = "-";
			headerValue26.HeaderVisible = true;
			headerValue26.Value = "-";
			headerValue26.Visible = false;
			this.tableRepair.TableCells = new BeerViewer.Views.Controls.GeneralTableView.HeaderValue[] {
        headerValue23,
        headerValue24,
        headerValue25,
        headerValue26};
			this.tableRepair.TableName = "입거";
			// 
			// tableFleet
			// 
			this.tableFleet.BackColor = System.Drawing.Color.Transparent;
			this.tableFleet.Dock = System.Windows.Forms.DockStyle.Top;
			this.tableFleet.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.tableFleet.ForeColor = System.Drawing.Color.White;
			this.tableFleet.Location = new System.Drawing.Point(0, 99);
			this.tableFleet.Margin = new System.Windows.Forms.Padding(0);
			this.tableFleet.Name = "tableFleet";
			this.tableFleet.Padding = new System.Windows.Forms.Padding(0, 0, 0, 8);
			this.tableFleet.Size = new System.Drawing.Size(384, 63);
			this.tableFleet.TabIndex = 172;
			headerValue27.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
			headerValue27.ForeColor = System.Drawing.Color.White;
			headerValue27.Header = "-";
			headerValue27.HeaderVisible = true;
			headerValue27.Value = "-";
			headerValue27.Visible = false;
			headerValue28.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
			headerValue28.ForeColor = System.Drawing.Color.White;
			headerValue28.Header = "-";
			headerValue28.HeaderVisible = true;
			headerValue28.Value = "-";
			headerValue28.Visible = false;
			headerValue29.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
			headerValue29.ForeColor = System.Drawing.Color.White;
			headerValue29.Header = "-";
			headerValue29.HeaderVisible = true;
			headerValue29.Value = "-";
			headerValue29.Visible = false;
			headerValue30.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
			headerValue30.ForeColor = System.Drawing.Color.White;
			headerValue30.Header = "-";
			headerValue30.HeaderVisible = true;
			headerValue30.Value = "-";
			headerValue30.Visible = false;
			this.tableFleet.TableCells = new BeerViewer.Views.Controls.GeneralTableView.HeaderValue[] {
        headerValue27,
        headerValue28,
        headerValue29,
        headerValue30};
			this.tableFleet.TableName = "함대";
			// 
			// GeneralView
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.AutoScroll = true;
			this.AutoScrollMinSize = new System.Drawing.Size(340, 0);
			this.Controls.Add(this.tableHQRecord);
			this.Controls.Add(this.listQuests);
			this.Controls.Add(this.tableBuild);
			this.Controls.Add(this.tableRepair);
			this.Controls.Add(this.tableFleet);
			this.Controls.Add(this.layoutHomeportInfo);
			this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "GeneralView";
			this.Size = new System.Drawing.Size(384, 460);
			this.layoutHomeportInfo.ResumeLayout(false);
			this.layoutHomeportInfo.PerformLayout();
			this.layoutButtons.ResumeLayout(false);
			this.layoutResources.ResumeLayout(false);
			this.layoutCountInfo.ResumeLayout(false);
			this.layoutCountInfo.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.FlowLayoutPanel layoutHomeportInfo;
		private System.Windows.Forms.FlowLayoutPanel layoutCountInfo;
		private Controls.GeneralTableView tableFleet;
		private Controls.GeneralTableView tableRepair;
		private Controls.GeneralTableView tableBuild;
		private Controls.QuestsView listQuests;
		private Controls.GeneralTableView tableHQRecord;
		private System.Windows.Forms.FlowLayoutPanel layoutButtons;
		private Controls.FlatButton btnShipList;
		private Controls.FlatButton btnItemList;
		private Controls.FlatButton btnCalculator;
		private Controls.FlatButton btnAkashi;
		private System.Windows.Forms.FlowLayoutPanel layoutResources;
		private Controls.FlatComboBox comboResources1;
		private Controls.FlatComboBox comboResources2;
		private System.Windows.Forms.Label labelShipCount;
		private System.Windows.Forms.Label labelSlotitemCount;
	}
}
