namespace BeerViewer.Views.Catalogs
{
	partial class catalogShips
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(catalogShips));
			this.layoutFilters = new System.Windows.Forms.FlowLayoutPanel();
			this.layoutShipType = new System.Windows.Forms.FlowLayoutPanel();
			this.layoutShipTypeHeader = new System.Windows.Forms.FlowLayoutPanel();
			this.labelShipType = new System.Windows.Forms.Label();
			this.chkShipTypeAll = new BeerViewer.Views.Controls.FlatCheckBox();
			this.layoutShipTypeList = new System.Windows.Forms.FlowLayoutPanel();
			this.layoutShipTypePreset = new System.Windows.Forms.FlowLayoutPanel();
			this.layoutShipFilter = new System.Windows.Forms.FlowLayoutPanel();
			this.layoutLevelFilter = new System.Windows.Forms.Panel();
			this.btnLevelAbove2 = new BeerViewer.Views.Controls.FlatButton();
			this.btnLevel1 = new BeerViewer.Views.Controls.FlatButton();
			this.btnLevelAll = new BeerViewer.Views.Controls.FlatButton();
			this.labelLevelFromTo = new System.Windows.Forms.Label();
			this.textLevelTo = new BeerViewer.Views.Controls.FlatTextBox();
			this.textLevelFrom = new BeerViewer.Views.Controls.FlatTextBox();
			this.labelLevels = new System.Windows.Forms.Label();
			this.layoutLockExpFilter = new System.Windows.Forms.Panel();
			this.groupLockFilter = new System.Windows.Forms.Panel();
			this.radioLockNo = new BeerViewer.Views.Controls.FlatRadioButton();
			this.radioLockYes = new BeerViewer.Views.Controls.FlatRadioButton();
			this.radioLockAll = new BeerViewer.Views.Controls.FlatRadioButton();
			this.chkExceptExpedition = new BeerViewer.Views.Controls.FlatCheckBox();
			this.labelExpedition = new System.Windows.Forms.Label();
			this.labelLock = new System.Windows.Forms.Label();
			this.layoutSpeedPowerUpFilter = new System.Windows.Forms.Panel();
			this.groupPowerupFilter = new System.Windows.Forms.Panel();
			this.radioPowerUpNotEnd = new BeerViewer.Views.Controls.FlatRadioButton();
			this.radioPowerUpEnd = new BeerViewer.Views.Controls.FlatRadioButton();
			this.radioPowerUpAll = new BeerViewer.Views.Controls.FlatRadioButton();
			this.groupSpeedFilter = new System.Windows.Forms.Panel();
			this.labelPowerupFilter = new System.Windows.Forms.Label();
			this.labelSpeedFilter = new System.Windows.Forms.Label();
			this.shipListTable = new BeerViewer.Views.Controls.ShipListTable();
			this.expandFilter = new BeerViewer.Views.Controls.FlatExpanderButton();
			this.chkSpeedSuperfast = new BeerViewer.Views.Controls.FlatCheckBox();
			this.chkSpeedFastPlus = new BeerViewer.Views.Controls.FlatCheckBox();
			this.chkSpeedFast = new BeerViewer.Views.Controls.FlatCheckBox();
			this.chkSpeedSlow = new BeerViewer.Views.Controls.FlatCheckBox();
			this.layoutFilters.SuspendLayout();
			this.layoutShipType.SuspendLayout();
			this.layoutShipTypeHeader.SuspendLayout();
			this.layoutShipFilter.SuspendLayout();
			this.layoutLevelFilter.SuspendLayout();
			this.layoutLockExpFilter.SuspendLayout();
			this.groupLockFilter.SuspendLayout();
			this.layoutSpeedPowerUpFilter.SuspendLayout();
			this.groupPowerupFilter.SuspendLayout();
			this.groupSpeedFilter.SuspendLayout();
			this.SuspendLayout();
			// 
			// layoutFilters
			// 
			this.layoutFilters.AutoSize = true;
			this.layoutFilters.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.layoutFilters.Controls.Add(this.layoutShipType);
			this.layoutFilters.Controls.Add(this.layoutShipFilter);
			this.layoutFilters.Dock = System.Windows.Forms.DockStyle.Top;
			this.layoutFilters.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.layoutFilters.Location = new System.Drawing.Point(8, 35);
			this.layoutFilters.Margin = new System.Windows.Forms.Padding(0);
			this.layoutFilters.Name = "layoutFilters";
			this.layoutFilters.Size = new System.Drawing.Size(979, 124);
			this.layoutFilters.TabIndex = 15;
			this.layoutFilters.WrapContents = false;
			// 
			// layoutShipType
			// 
			this.layoutShipType.AutoSize = true;
			this.layoutShipType.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.layoutShipType.Controls.Add(this.layoutShipTypeHeader);
			this.layoutShipType.Controls.Add(this.layoutShipTypeList);
			this.layoutShipType.Controls.Add(this.layoutShipTypePreset);
			this.layoutShipType.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.layoutShipType.Location = new System.Drawing.Point(0, 0);
			this.layoutShipType.Margin = new System.Windows.Forms.Padding(0);
			this.layoutShipType.Name = "layoutShipType";
			this.layoutShipType.Padding = new System.Windows.Forms.Padding(5, 5, 5, 13);
			this.layoutShipType.Size = new System.Drawing.Size(125, 43);
			this.layoutShipType.TabIndex = 7;
			this.layoutShipType.WrapContents = false;
			// 
			// layoutShipTypeHeader
			// 
			this.layoutShipTypeHeader.AutoSize = true;
			this.layoutShipTypeHeader.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.layoutShipTypeHeader.Controls.Add(this.labelShipType);
			this.layoutShipTypeHeader.Controls.Add(this.chkShipTypeAll);
			this.layoutShipTypeHeader.Location = new System.Drawing.Point(5, 5);
			this.layoutShipTypeHeader.Margin = new System.Windows.Forms.Padding(0);
			this.layoutShipTypeHeader.Name = "layoutShipTypeHeader";
			this.layoutShipTypeHeader.Size = new System.Drawing.Size(115, 21);
			this.layoutShipTypeHeader.TabIndex = 14;
			// 
			// labelShipType
			// 
			this.labelShipType.AutoSize = true;
			this.labelShipType.Location = new System.Drawing.Point(0, 2);
			this.labelShipType.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
			this.labelShipType.Name = "labelShipType";
			this.labelShipType.Size = new System.Drawing.Size(38, 15);
			this.labelShipType.TabIndex = 14;
			this.labelShipType.Text = "함종 :";
			// 
			// chkShipTypeAll
			// 
			this.chkShipTypeAll.AutoSize = true;
			this.chkShipTypeAll.Checked = true;
			this.chkShipTypeAll.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkShipTypeAll.Location = new System.Drawing.Point(41, 3);
			this.chkShipTypeAll.Name = "chkShipTypeAll";
			this.chkShipTypeAll.Size = new System.Drawing.Size(71, 15);
			this.chkShipTypeAll.TabIndex = 15;
			this.chkShipTypeAll.Text = "모두선택";
			this.chkShipTypeAll.UseVisualStyleBackColor = true;
			// 
			// layoutShipTypeList
			// 
			this.layoutShipTypeList.AutoSize = true;
			this.layoutShipTypeList.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.layoutShipTypeList.Location = new System.Drawing.Point(5, 26);
			this.layoutShipTypeList.Margin = new System.Windows.Forms.Padding(0);
			this.layoutShipTypeList.Name = "layoutShipTypeList";
			this.layoutShipTypeList.Padding = new System.Windows.Forms.Padding(2);
			this.layoutShipTypeList.Size = new System.Drawing.Size(4, 4);
			this.layoutShipTypeList.TabIndex = 11;
			// 
			// layoutShipTypePreset
			// 
			this.layoutShipTypePreset.AutoSize = true;
			this.layoutShipTypePreset.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.layoutShipTypePreset.Location = new System.Drawing.Point(5, 30);
			this.layoutShipTypePreset.Margin = new System.Windows.Forms.Padding(0);
			this.layoutShipTypePreset.Name = "layoutShipTypePreset";
			this.layoutShipTypePreset.Size = new System.Drawing.Size(0, 0);
			this.layoutShipTypePreset.TabIndex = 13;
			// 
			// layoutShipFilter
			// 
			this.layoutShipFilter.AutoSize = true;
			this.layoutShipFilter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.layoutShipFilter.Controls.Add(this.layoutLevelFilter);
			this.layoutShipFilter.Controls.Add(this.layoutLockExpFilter);
			this.layoutShipFilter.Controls.Add(this.layoutSpeedPowerUpFilter);
			this.layoutShipFilter.Location = new System.Drawing.Point(0, 43);
			this.layoutShipFilter.Margin = new System.Windows.Forms.Padding(0);
			this.layoutShipFilter.Name = "layoutShipFilter";
			this.layoutShipFilter.Padding = new System.Windows.Forms.Padding(5, 5, 5, 13);
			this.layoutShipFilter.Size = new System.Drawing.Size(783, 81);
			this.layoutShipFilter.TabIndex = 9;
			// 
			// layoutLevelFilter
			// 
			this.layoutLevelFilter.AutoSize = true;
			this.layoutLevelFilter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.layoutLevelFilter.Controls.Add(this.btnLevelAbove2);
			this.layoutLevelFilter.Controls.Add(this.btnLevel1);
			this.layoutLevelFilter.Controls.Add(this.btnLevelAll);
			this.layoutLevelFilter.Controls.Add(this.labelLevelFromTo);
			this.layoutLevelFilter.Controls.Add(this.textLevelTo);
			this.layoutLevelFilter.Controls.Add(this.textLevelFrom);
			this.layoutLevelFilter.Controls.Add(this.labelLevels);
			this.layoutLevelFilter.Location = new System.Drawing.Point(9, 9);
			this.layoutLevelFilter.Margin = new System.Windows.Forms.Padding(4);
			this.layoutLevelFilter.Name = "layoutLevelFilter";
			this.layoutLevelFilter.Padding = new System.Windows.Forms.Padding(2);
			this.layoutLevelFilter.Size = new System.Drawing.Size(243, 55);
			this.layoutLevelFilter.TabIndex = 0;
			// 
			// btnLevelAbove2
			// 
			this.btnLevelAbove2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(39)))), ((int)(((byte)(47)))));
			this.btnLevelAbove2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(83)))), ((int)(((byte)(83)))));
			this.btnLevelAbove2.DownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(117)))), ((int)(((byte)(142)))));
			this.btnLevelAbove2.DownBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(71)))), ((int)(((byte)(83)))));
			this.btnLevelAbove2.Font = new System.Drawing.Font("맑은 고딕", 9F);
			this.btnLevelAbove2.ForeColor = System.Drawing.Color.White;
			this.btnLevelAbove2.Location = new System.Drawing.Point(170, 28);
			this.btnLevelAbove2.Margin = new System.Windows.Forms.Padding(2);
			this.btnLevelAbove2.Name = "btnLevelAbove2";
			this.btnLevelAbove2.OverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(89)))), ((int)(((byte)(89)))));
			this.btnLevelAbove2.OverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(71)))), ((int)(((byte)(83)))));
			this.btnLevelAbove2.Size = new System.Drawing.Size(69, 23);
			this.btnLevelAbove2.TabIndex = 8;
			this.btnLevelAbove2.Text = "Lv. 2 이상";
			// 
			// btnLevel1
			// 
			this.btnLevel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(39)))), ((int)(((byte)(47)))));
			this.btnLevel1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(83)))), ((int)(((byte)(83)))));
			this.btnLevel1.DownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(117)))), ((int)(((byte)(142)))));
			this.btnLevel1.DownBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(71)))), ((int)(((byte)(83)))));
			this.btnLevel1.Font = new System.Drawing.Font("맑은 고딕", 9F);
			this.btnLevel1.ForeColor = System.Drawing.Color.White;
			this.btnLevel1.Location = new System.Drawing.Point(121, 28);
			this.btnLevel1.Margin = new System.Windows.Forms.Padding(2);
			this.btnLevel1.Name = "btnLevel1";
			this.btnLevel1.OverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(89)))), ((int)(((byte)(89)))));
			this.btnLevel1.OverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(71)))), ((int)(((byte)(83)))));
			this.btnLevel1.Size = new System.Drawing.Size(45, 23);
			this.btnLevel1.TabIndex = 7;
			this.btnLevel1.Text = "Lv. 1";
			// 
			// btnLevelAll
			// 
			this.btnLevelAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(39)))), ((int)(((byte)(47)))));
			this.btnLevelAll.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(83)))), ((int)(((byte)(83)))));
			this.btnLevelAll.DownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(117)))), ((int)(((byte)(142)))));
			this.btnLevelAll.DownBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(71)))), ((int)(((byte)(83)))));
			this.btnLevelAll.Font = new System.Drawing.Font("맑은 고딕", 9F);
			this.btnLevelAll.ForeColor = System.Drawing.Color.White;
			this.btnLevelAll.Location = new System.Drawing.Point(72, 28);
			this.btnLevelAll.Margin = new System.Windows.Forms.Padding(2);
			this.btnLevelAll.Name = "btnLevelAll";
			this.btnLevelAll.OverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(89)))), ((int)(((byte)(89)))));
			this.btnLevelAll.OverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(71)))), ((int)(((byte)(83)))));
			this.btnLevelAll.Size = new System.Drawing.Size(45, 23);
			this.btnLevelAll.TabIndex = 6;
			this.btnLevelAll.Text = "모두";
			// 
			// labelLevelFromTo
			// 
			this.labelLevelFromTo.AutoSize = true;
			this.labelLevelFromTo.Location = new System.Drawing.Point(111, 4);
			this.labelLevelFromTo.Name = "labelLevelFromTo";
			this.labelLevelFromTo.Size = new System.Drawing.Size(13, 21);
			this.labelLevelFromTo.TabIndex = 5;
			this.labelLevelFromTo.Text = "~";
			this.labelLevelFromTo.UseCompatibleTextRendering = true;
			// 
			// textLevelTo
			// 
			this.textLevelTo.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.textLevelTo.Lines = new string[] {
        "155"};
			this.textLevelTo.Location = new System.Drawing.Point(126, 2);
			this.textLevelTo.MaxLength = 32767;
			this.textLevelTo.Name = "textLevelTo";
			this.textLevelTo.PasswordChar = '\0';
			this.textLevelTo.PromptColor = System.Drawing.Color.FromArgb(((int)(((byte)(152)))), ((int)(((byte)(152)))), ((int)(((byte)(152)))));
			this.textLevelTo.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.textLevelTo.SelectedText = "";
			this.textLevelTo.SelectionLength = 0;
			this.textLevelTo.SelectionStart = 0;
			this.textLevelTo.ShortcutsEnabled = true;
			this.textLevelTo.Size = new System.Drawing.Size(37, 23);
			this.textLevelTo.TabIndex = 4;
			this.textLevelTo.Text = "155";
			// 
			// textLevelFrom
			// 
			this.textLevelFrom.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.textLevelFrom.Lines = new string[] {
        "2"};
			this.textLevelFrom.Location = new System.Drawing.Point(72, 2);
			this.textLevelFrom.MaxLength = 32767;
			this.textLevelFrom.Name = "textLevelFrom";
			this.textLevelFrom.PasswordChar = '\0';
			this.textLevelFrom.PromptColor = System.Drawing.Color.FromArgb(((int)(((byte)(152)))), ((int)(((byte)(152)))), ((int)(((byte)(152)))));
			this.textLevelFrom.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.textLevelFrom.SelectedText = "";
			this.textLevelFrom.SelectionLength = 0;
			this.textLevelFrom.SelectionStart = 0;
			this.textLevelFrom.ShortcutsEnabled = true;
			this.textLevelFrom.Size = new System.Drawing.Size(37, 23);
			this.textLevelFrom.TabIndex = 3;
			this.textLevelFrom.Text = "2";
			// 
			// labelLevels
			// 
			this.labelLevels.AutoSize = true;
			this.labelLevels.Location = new System.Drawing.Point(2, 4);
			this.labelLevels.Margin = new System.Windows.Forms.Padding(0);
			this.labelLevels.Name = "labelLevels";
			this.labelLevels.Size = new System.Drawing.Size(65, 21);
			this.labelLevels.TabIndex = 2;
			this.labelLevels.Text = "레벨 범위 :";
			this.labelLevels.UseCompatibleTextRendering = true;
			// 
			// layoutLockExpFilter
			// 
			this.layoutLockExpFilter.AutoSize = true;
			this.layoutLockExpFilter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.layoutLockExpFilter.Controls.Add(this.groupLockFilter);
			this.layoutLockExpFilter.Controls.Add(this.chkExceptExpedition);
			this.layoutLockExpFilter.Controls.Add(this.labelExpedition);
			this.layoutLockExpFilter.Controls.Add(this.labelLock);
			this.layoutLockExpFilter.Location = new System.Drawing.Point(260, 9);
			this.layoutLockExpFilter.Margin = new System.Windows.Forms.Padding(4);
			this.layoutLockExpFilter.Name = "layoutLockExpFilter";
			this.layoutLockExpFilter.Padding = new System.Windows.Forms.Padding(2);
			this.layoutLockExpFilter.Size = new System.Drawing.Size(222, 48);
			this.layoutLockExpFilter.TabIndex = 1;
			// 
			// groupLockFilter
			// 
			this.groupLockFilter.AutoSize = true;
			this.groupLockFilter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.groupLockFilter.Controls.Add(this.radioLockNo);
			this.groupLockFilter.Controls.Add(this.radioLockYes);
			this.groupLockFilter.Controls.Add(this.radioLockAll);
			this.groupLockFilter.Location = new System.Drawing.Point(49, 2);
			this.groupLockFilter.Margin = new System.Windows.Forms.Padding(0);
			this.groupLockFilter.Name = "groupLockFilter";
			this.groupLockFilter.Size = new System.Drawing.Size(171, 21);
			this.groupLockFilter.TabIndex = 7;
			// 
			// radioLockNo
			// 
			this.radioLockNo.AutoSize = true;
			this.radioLockNo.Location = new System.Drawing.Point(109, 3);
			this.radioLockNo.Name = "radioLockNo";
			this.radioLockNo.Size = new System.Drawing.Size(59, 15);
			this.radioLockNo.TabIndex = 14;
			this.radioLockNo.Text = "안채움";
			this.radioLockNo.UseVisualStyleBackColor = true;
			// 
			// radioLockYes
			// 
			this.radioLockYes.AutoSize = true;
			this.radioLockYes.Location = new System.Drawing.Point(56, 3);
			this.radioLockYes.Name = "radioLockYes";
			this.radioLockYes.Size = new System.Drawing.Size(47, 15);
			this.radioLockYes.TabIndex = 13;
			this.radioLockYes.Text = "채움";
			this.radioLockYes.UseVisualStyleBackColor = true;
			// 
			// radioLockAll
			// 
			this.radioLockAll.AutoSize = true;
			this.radioLockAll.Checked = true;
			this.radioLockAll.Location = new System.Drawing.Point(3, 3);
			this.radioLockAll.Name = "radioLockAll";
			this.radioLockAll.Size = new System.Drawing.Size(47, 15);
			this.radioLockAll.TabIndex = 12;
			this.radioLockAll.TabStop = true;
			this.radioLockAll.Text = "전부";
			this.radioLockAll.UseVisualStyleBackColor = true;
			// 
			// chkExceptExpedition
			// 
			this.chkExceptExpedition.AutoSize = true;
			this.chkExceptExpedition.Location = new System.Drawing.Point(41, 26);
			this.chkExceptExpedition.Name = "chkExceptExpedition";
			this.chkExceptExpedition.Size = new System.Drawing.Size(151, 15);
			this.chkExceptExpedition.TabIndex = 6;
			this.chkExceptExpedition.Text = "원정중인 칸무스를 제외";
			this.chkExceptExpedition.UseVisualStyleBackColor = true;
			// 
			// labelExpedition
			// 
			this.labelExpedition.AutoSize = true;
			this.labelExpedition.Location = new System.Drawing.Point(2, 25);
			this.labelExpedition.Margin = new System.Windows.Forms.Padding(0);
			this.labelExpedition.Name = "labelExpedition";
			this.labelExpedition.Size = new System.Drawing.Size(36, 21);
			this.labelExpedition.TabIndex = 5;
			this.labelExpedition.Text = "원정 :";
			this.labelExpedition.UseCompatibleTextRendering = true;
			// 
			// labelLock
			// 
			this.labelLock.AutoSize = true;
			this.labelLock.Location = new System.Drawing.Point(2, 4);
			this.labelLock.Margin = new System.Windows.Forms.Padding(0);
			this.labelLock.Name = "labelLock";
			this.labelLock.Size = new System.Drawing.Size(49, 21);
			this.labelLock.TabIndex = 2;
			this.labelLock.Text = "자물쇠 :";
			this.labelLock.UseCompatibleTextRendering = true;
			// 
			// layoutSpeedPowerUpFilter
			// 
			this.layoutSpeedPowerUpFilter.AutoSize = true;
			this.layoutSpeedPowerUpFilter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.layoutSpeedPowerUpFilter.Controls.Add(this.groupPowerupFilter);
			this.layoutSpeedPowerUpFilter.Controls.Add(this.groupSpeedFilter);
			this.layoutSpeedPowerUpFilter.Controls.Add(this.labelPowerupFilter);
			this.layoutSpeedPowerUpFilter.Controls.Add(this.labelSpeedFilter);
			this.layoutSpeedPowerUpFilter.Location = new System.Drawing.Point(490, 9);
			this.layoutSpeedPowerUpFilter.Margin = new System.Windows.Forms.Padding(4);
			this.layoutSpeedPowerUpFilter.Name = "layoutSpeedPowerUpFilter";
			this.layoutSpeedPowerUpFilter.Padding = new System.Windows.Forms.Padding(2);
			this.layoutSpeedPowerUpFilter.Size = new System.Drawing.Size(284, 48);
			this.layoutSpeedPowerUpFilter.TabIndex = 2;
			// 
			// groupPowerupFilter
			// 
			this.groupPowerupFilter.AutoSize = true;
			this.groupPowerupFilter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.groupPowerupFilter.Controls.Add(this.radioPowerUpNotEnd);
			this.groupPowerupFilter.Controls.Add(this.radioPowerUpEnd);
			this.groupPowerupFilter.Controls.Add(this.radioPowerUpAll);
			this.groupPowerupFilter.Location = new System.Drawing.Point(75, 23);
			this.groupPowerupFilter.Margin = new System.Windows.Forms.Padding(0);
			this.groupPowerupFilter.Name = "groupPowerupFilter";
			this.groupPowerupFilter.Size = new System.Drawing.Size(207, 21);
			this.groupPowerupFilter.TabIndex = 9;
			// 
			// radioPowerUpNotEnd
			// 
			this.radioPowerUpNotEnd.AutoSize = true;
			this.radioPowerUpNotEnd.Location = new System.Drawing.Point(133, 2);
			this.radioPowerUpNotEnd.Name = "radioPowerUpNotEnd";
			this.radioPowerUpNotEnd.Size = new System.Drawing.Size(71, 15);
			this.radioPowerUpNotEnd.TabIndex = 14;
			this.radioPowerUpNotEnd.Text = "개수미완";
			this.radioPowerUpNotEnd.UseVisualStyleBackColor = true;
			// 
			// radioPowerUpEnd
			// 
			this.radioPowerUpEnd.AutoSize = true;
			this.radioPowerUpEnd.Location = new System.Drawing.Point(56, 3);
			this.radioPowerUpEnd.Name = "radioPowerUpEnd";
			this.radioPowerUpEnd.Size = new System.Drawing.Size(71, 15);
			this.radioPowerUpEnd.TabIndex = 13;
			this.radioPowerUpEnd.Text = "개수완료";
			this.radioPowerUpEnd.UseVisualStyleBackColor = true;
			// 
			// radioPowerUpAll
			// 
			this.radioPowerUpAll.AutoSize = true;
			this.radioPowerUpAll.Checked = true;
			this.radioPowerUpAll.Location = new System.Drawing.Point(3, 3);
			this.radioPowerUpAll.Name = "radioPowerUpAll";
			this.radioPowerUpAll.Size = new System.Drawing.Size(47, 15);
			this.radioPowerUpAll.TabIndex = 12;
			this.radioPowerUpAll.TabStop = true;
			this.radioPowerUpAll.Text = "전부";
			this.radioPowerUpAll.UseVisualStyleBackColor = true;
			// 
			// groupSpeedFilter
			// 
			this.groupSpeedFilter.AutoSize = true;
			this.groupSpeedFilter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.groupSpeedFilter.Controls.Add(this.chkSpeedSlow);
			this.groupSpeedFilter.Controls.Add(this.chkSpeedFast);
			this.groupSpeedFilter.Controls.Add(this.chkSpeedFastPlus);
			this.groupSpeedFilter.Controls.Add(this.chkSpeedSuperfast);
			this.groupSpeedFilter.Location = new System.Drawing.Point(38, 2);
			this.groupSpeedFilter.Margin = new System.Windows.Forms.Padding(0);
			this.groupSpeedFilter.Name = "groupSpeedFilter";
			this.groupSpeedFilter.Size = new System.Drawing.Size(232, 21);
			this.groupSpeedFilter.TabIndex = 8;
			// 
			// labelPowerupFilter
			// 
			this.labelPowerupFilter.AutoSize = true;
			this.labelPowerupFilter.Location = new System.Drawing.Point(2, 25);
			this.labelPowerupFilter.Margin = new System.Windows.Forms.Padding(0);
			this.labelPowerupFilter.Name = "labelPowerupFilter";
			this.labelPowerupFilter.Size = new System.Drawing.Size(73, 21);
			this.labelPowerupFilter.TabIndex = 5;
			this.labelPowerupFilter.Text = "근대화개수 :";
			this.labelPowerupFilter.UseCompatibleTextRendering = true;
			// 
			// labelSpeedFilter
			// 
			this.labelSpeedFilter.AutoSize = true;
			this.labelSpeedFilter.Location = new System.Drawing.Point(2, 4);
			this.labelSpeedFilter.Margin = new System.Windows.Forms.Padding(0);
			this.labelSpeedFilter.Name = "labelSpeedFilter";
			this.labelSpeedFilter.Size = new System.Drawing.Size(36, 21);
			this.labelSpeedFilter.TabIndex = 2;
			this.labelSpeedFilter.Text = "속력 :";
			this.labelSpeedFilter.UseCompatibleTextRendering = true;
			// 
			// shipListTable
			// 
			this.shipListTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.shipListTable.ExceptExpedition = false;
			this.shipListTable.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.shipListTable.LevelFrom = 0;
			this.shipListTable.LevelTo = 0;
			this.shipListTable.Location = new System.Drawing.Point(8, 159);
			this.shipListTable.Margin = new System.Windows.Forms.Padding(0);
			this.shipListTable.Name = "shipListTable";
			this.shipListTable.ShipTypes = null;
			this.shipListTable.Size = new System.Drawing.Size(979, 375);
			this.shipListTable.TabIndex = 16;
			// 
			// expandFilter
			// 
			this.expandFilter.Dock = System.Windows.Forms.DockStyle.Top;
			this.expandFilter.DownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(117)))), ((int)(((byte)(142)))));
			this.expandFilter.Expanded = true;
			this.expandFilter.Font = new System.Drawing.Font("맑은 고딕", 10F);
			this.expandFilter.Location = new System.Drawing.Point(8, 8);
			this.expandFilter.Margin = new System.Windows.Forms.Padding(0);
			this.expandFilter.Name = "expandFilter";
			this.expandFilter.OverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(89)))), ((int)(((byte)(89)))));
			this.expandFilter.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
			this.expandFilter.Size = new System.Drawing.Size(979, 27);
			this.expandFilter.TabIndex = 1;
			this.expandFilter.Text = "필터 설정";
			// 
			// chkSpeedSuperfast
			// 
			this.chkSpeedSuperfast.AutoSize = true;
			this.chkSpeedSuperfast.Checked = true;
			this.chkSpeedSuperfast.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkSpeedSuperfast.Location = new System.Drawing.Point(3, 3);
			this.chkSpeedSuperfast.Name = "chkSpeedSuperfast";
			this.chkSpeedSuperfast.Size = new System.Drawing.Size(59, 15);
			this.chkSpeedSuperfast.TabIndex = 7;
			this.chkSpeedSuperfast.Text = "초고속";
			this.chkSpeedSuperfast.UseVisualStyleBackColor = true;
			// 
			// chkSpeedFastPlus
			// 
			this.chkSpeedFastPlus.AutoSize = true;
			this.chkSpeedFastPlus.Checked = true;
			this.chkSpeedFastPlus.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkSpeedFastPlus.Location = new System.Drawing.Point(68, 3);
			this.chkSpeedFastPlus.Name = "chkSpeedFastPlus";
			this.chkSpeedFastPlus.Size = new System.Drawing.Size(55, 15);
			this.chkSpeedFastPlus.TabIndex = 8;
			this.chkSpeedFastPlus.Text = "고속+";
			this.chkSpeedFastPlus.UseVisualStyleBackColor = true;
			// 
			// chkSpeedFast
			// 
			this.chkSpeedFast.AutoSize = true;
			this.chkSpeedFast.Checked = true;
			this.chkSpeedFast.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkSpeedFast.Location = new System.Drawing.Point(129, 3);
			this.chkSpeedFast.Name = "chkSpeedFast";
			this.chkSpeedFast.Size = new System.Drawing.Size(47, 15);
			this.chkSpeedFast.TabIndex = 9;
			this.chkSpeedFast.Text = "고속";
			this.chkSpeedFast.UseVisualStyleBackColor = true;
			// 
			// chkSpeedSlow
			// 
			this.chkSpeedSlow.AutoSize = true;
			this.chkSpeedSlow.Checked = true;
			this.chkSpeedSlow.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkSpeedSlow.Location = new System.Drawing.Point(182, 3);
			this.chkSpeedSlow.Name = "chkSpeedSlow";
			this.chkSpeedSlow.Size = new System.Drawing.Size(47, 15);
			this.chkSpeedSlow.TabIndex = 10;
			this.chkSpeedSlow.Text = "저속";
			this.chkSpeedSlow.UseVisualStyleBackColor = true;
			// 
			// catalogShips
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(72)))));
			this.ClientSize = new System.Drawing.Size(995, 542);
			this.Controls.Add(this.shipListTable);
			this.Controls.Add(this.layoutFilters);
			this.Controls.Add(this.expandFilter);
			this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.ForeColor = System.Drawing.Color.White;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "catalogShips";
			this.Padding = new System.Windows.Forms.Padding(8);
			this.Text = "소유 칸무스 목록";
			this.layoutFilters.ResumeLayout(false);
			this.layoutFilters.PerformLayout();
			this.layoutShipType.ResumeLayout(false);
			this.layoutShipType.PerformLayout();
			this.layoutShipTypeHeader.ResumeLayout(false);
			this.layoutShipTypeHeader.PerformLayout();
			this.layoutShipFilter.ResumeLayout(false);
			this.layoutShipFilter.PerformLayout();
			this.layoutLevelFilter.ResumeLayout(false);
			this.layoutLevelFilter.PerformLayout();
			this.layoutLockExpFilter.ResumeLayout(false);
			this.layoutLockExpFilter.PerformLayout();
			this.groupLockFilter.ResumeLayout(false);
			this.groupLockFilter.PerformLayout();
			this.layoutSpeedPowerUpFilter.ResumeLayout(false);
			this.layoutSpeedPowerUpFilter.PerformLayout();
			this.groupPowerupFilter.ResumeLayout(false);
			this.groupPowerupFilter.PerformLayout();
			this.groupSpeedFilter.ResumeLayout(false);
			this.groupSpeedFilter.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private Controls.FlatExpanderButton expandFilter;
		private System.Windows.Forms.FlowLayoutPanel layoutFilters;
		private System.Windows.Forms.FlowLayoutPanel layoutShipType;
		private System.Windows.Forms.FlowLayoutPanel layoutShipTypeHeader;
		private System.Windows.Forms.Label labelShipType;
		private Controls.FlatCheckBox chkShipTypeAll;
		private System.Windows.Forms.FlowLayoutPanel layoutShipTypeList;
		private System.Windows.Forms.FlowLayoutPanel layoutShipTypePreset;
		private System.Windows.Forms.FlowLayoutPanel layoutShipFilter;
		private System.Windows.Forms.Panel layoutLevelFilter;
		private Controls.FlatButton btnLevelAbove2;
		private Controls.FlatButton btnLevel1;
		private Controls.FlatButton btnLevelAll;
		private System.Windows.Forms.Label labelLevelFromTo;
		private Controls.FlatTextBox textLevelTo;
		private Controls.FlatTextBox textLevelFrom;
		private System.Windows.Forms.Label labelLevels;
		private System.Windows.Forms.Panel layoutLockExpFilter;
		private System.Windows.Forms.Panel groupLockFilter;
		private Controls.FlatRadioButton radioLockNo;
		private Controls.FlatRadioButton radioLockYes;
		private Controls.FlatRadioButton radioLockAll;
		private Controls.FlatCheckBox chkExceptExpedition;
		private System.Windows.Forms.Label labelExpedition;
		private System.Windows.Forms.Label labelLock;
		private System.Windows.Forms.Panel layoutSpeedPowerUpFilter;
		private System.Windows.Forms.Panel groupPowerupFilter;
		private Controls.FlatRadioButton radioPowerUpNotEnd;
		private Controls.FlatRadioButton radioPowerUpEnd;
		private Controls.FlatRadioButton radioPowerUpAll;
		private System.Windows.Forms.Panel groupSpeedFilter;
		private System.Windows.Forms.Label labelPowerupFilter;
		private System.Windows.Forms.Label labelSpeedFilter;
		private Controls.ShipListTable shipListTable;
		private Controls.FlatCheckBox chkSpeedSlow;
		private Controls.FlatCheckBox chkSpeedFast;
		private Controls.FlatCheckBox chkSpeedFastPlus;
		private Controls.FlatCheckBox chkSpeedSuperfast;
	}
}