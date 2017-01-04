namespace BeerViewer.Views.Catalogs
{
	partial class catalogCalc
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(catalogCalc));
			this.labelInfo1 = new System.Windows.Forms.Label();
			this.labelInfo2 = new System.Windows.Forms.Label();
			this.labelStart = new System.Windows.Forms.Label();
			this.labelGoal = new System.Windows.Forms.Label();
			this.labelMap = new System.Windows.Forms.Label();
			this.labelRank = new System.Windows.Forms.Label();
			this.labelSeparator2 = new System.Windows.Forms.Label();
			this.labelSeparator1 = new System.Windows.Forms.Label();
			this.labelLeftExp = new System.Windows.Forms.Label();
			this.labelMapExp = new System.Windows.Forms.Label();
			this.labelTimes = new System.Windows.Forms.Label();
			this.textGoalExp = new BeerViewer.Views.Controls.FlatTextBox();
			this.textCurrentExp = new BeerViewer.Views.Controls.FlatTextBox();
			this.chkMVP = new BeerViewer.Views.Controls.FlatCheckBox();
			this.chkFlagship = new BeerViewer.Views.Controls.FlatCheckBox();
			this.textTimes = new BeerViewer.Views.Controls.FlatTextBox();
			this.textMapExp = new BeerViewer.Views.Controls.FlatTextBox();
			this.textLeftExp = new BeerViewer.Views.Controls.FlatTextBox();
			this.comboRank = new BeerViewer.Views.Controls.FlatComboBox();
			this.comboMap = new BeerViewer.Views.Controls.FlatComboBox();
			this.comboGoalLevel = new BeerViewer.Views.Controls.FlatComboBox();
			this.comboStartLevel = new BeerViewer.Views.Controls.FlatComboBox();
			this.comboShip = new BeerViewer.Views.Controls.FlatComboBox();
			this.SuspendLayout();
			// 
			// labelInfo1
			// 
			this.labelInfo1.AutoSize = true;
			this.labelInfo1.Font = new System.Drawing.Font("맑은 고딕", 9F);
			this.labelInfo1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(200)))), ((int)(((byte)(74)))));
			this.labelInfo1.Location = new System.Drawing.Point(8, 9);
			this.labelInfo1.Name = "labelInfo1";
			this.labelInfo1.Size = new System.Drawing.Size(236, 15);
			this.labelInfo1.TabIndex = 0;
			this.labelInfo1.Text = "※ 목표레벨은 개조레벨로 자동설정됩니다!";
			// 
			// labelInfo2
			// 
			this.labelInfo2.AutoSize = true;
			this.labelInfo2.Font = new System.Drawing.Font("맑은 고딕", 9F);
			this.labelInfo2.Location = new System.Drawing.Point(22, 28);
			this.labelInfo2.Name = "labelInfo2";
			this.labelInfo2.Size = new System.Drawing.Size(337, 15);
			this.labelInfo2.TabIndex = 1;
			this.labelInfo2.Text = "개조가 모두 끝난 칸무스는 현재레벨+1로 목표가 설정됩니다.";
			// 
			// labelStart
			// 
			this.labelStart.AutoSize = true;
			this.labelStart.Location = new System.Drawing.Point(12, 104);
			this.labelStart.Name = "labelStart";
			this.labelStart.Size = new System.Drawing.Size(59, 15);
			this.labelStart.TabIndex = 3;
			this.labelStart.Text = "현재 레벨";
			// 
			// labelGoal
			// 
			this.labelGoal.AutoSize = true;
			this.labelGoal.Location = new System.Drawing.Point(12, 139);
			this.labelGoal.Name = "labelGoal";
			this.labelGoal.Size = new System.Drawing.Size(59, 15);
			this.labelGoal.TabIndex = 5;
			this.labelGoal.Text = "목표 레벨";
			// 
			// labelMap
			// 
			this.labelMap.AutoSize = true;
			this.labelMap.Location = new System.Drawing.Point(12, 174);
			this.labelMap.Name = "labelMap";
			this.labelMap.Size = new System.Drawing.Size(31, 15);
			this.labelMap.TabIndex = 7;
			this.labelMap.Text = "해역";
			// 
			// labelRank
			// 
			this.labelRank.AutoSize = true;
			this.labelRank.Location = new System.Drawing.Point(127, 174);
			this.labelRank.Name = "labelRank";
			this.labelRank.Size = new System.Drawing.Size(31, 15);
			this.labelRank.TabIndex = 9;
			this.labelRank.Text = "랭크";
			// 
			// labelSeparator2
			// 
			this.labelSeparator2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
			this.labelSeparator2.Location = new System.Drawing.Point(12, 203);
			this.labelSeparator2.Name = "labelSeparator2";
			this.labelSeparator2.Size = new System.Drawing.Size(344, 1);
			this.labelSeparator2.TabIndex = 11;
			// 
			// labelSeparator1
			// 
			this.labelSeparator1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
			this.labelSeparator1.Location = new System.Drawing.Point(12, 54);
			this.labelSeparator1.Name = "labelSeparator1";
			this.labelSeparator1.Size = new System.Drawing.Size(344, 1);
			this.labelSeparator1.TabIndex = 12;
			// 
			// labelLeftExp
			// 
			this.labelLeftExp.AutoSize = true;
			this.labelLeftExp.Location = new System.Drawing.Point(12, 251);
			this.labelLeftExp.Name = "labelLeftExp";
			this.labelLeftExp.Size = new System.Drawing.Size(71, 15);
			this.labelLeftExp.TabIndex = 14;
			this.labelLeftExp.Text = "남은 경험치";
			// 
			// labelMapExp
			// 
			this.labelMapExp.AutoSize = true;
			this.labelMapExp.Location = new System.Drawing.Point(12, 216);
			this.labelMapExp.Name = "labelMapExp";
			this.labelMapExp.Size = new System.Drawing.Size(71, 15);
			this.labelMapExp.TabIndex = 13;
			this.labelMapExp.Text = "해역 경험치";
			// 
			// labelTimes
			// 
			this.labelTimes.AutoSize = true;
			this.labelTimes.Location = new System.Drawing.Point(155, 216);
			this.labelTimes.Name = "labelTimes";
			this.labelTimes.Size = new System.Drawing.Size(13, 15);
			this.labelTimes.TabIndex = 18;
			this.labelTimes.Text = "x";
			// 
			// textGoalExp
			// 
			this.textGoalExp.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.textGoalExp.Lines = new string[] {
        "0"};
			this.textGoalExp.Location = new System.Drawing.Point(147, 134);
			this.textGoalExp.MaxLength = 32767;
			this.textGoalExp.Name = "textGoalExp";
			this.textGoalExp.PasswordChar = '\0';
			this.textGoalExp.PromptColor = System.Drawing.Color.FromArgb(((int)(((byte)(152)))), ((int)(((byte)(152)))), ((int)(((byte)(152)))));
			this.textGoalExp.ReadOnly = true;
			this.textGoalExp.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.textGoalExp.SelectedText = "";
			this.textGoalExp.SelectionLength = 0;
			this.textGoalExp.SelectionStart = 0;
			this.textGoalExp.ShortcutsEnabled = true;
			this.textGoalExp.Size = new System.Drawing.Size(81, 25);
			this.textGoalExp.TabIndex = 23;
			this.textGoalExp.Text = "0";
			// 
			// textCurrentExp
			// 
			this.textCurrentExp.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.textCurrentExp.Lines = new string[] {
        "0"};
			this.textCurrentExp.Location = new System.Drawing.Point(147, 99);
			this.textCurrentExp.MaxLength = 32767;
			this.textCurrentExp.Name = "textCurrentExp";
			this.textCurrentExp.PasswordChar = '\0';
			this.textCurrentExp.PromptColor = System.Drawing.Color.FromArgb(((int)(((byte)(152)))), ((int)(((byte)(152)))), ((int)(((byte)(152)))));
			this.textCurrentExp.ReadOnly = true;
			this.textCurrentExp.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.textCurrentExp.SelectedText = "";
			this.textCurrentExp.SelectionLength = 0;
			this.textCurrentExp.SelectionStart = 0;
			this.textCurrentExp.ShortcutsEnabled = true;
			this.textCurrentExp.Size = new System.Drawing.Size(81, 25);
			this.textCurrentExp.TabIndex = 22;
			this.textCurrentExp.Text = "0";
			// 
			// chkMVP
			// 
			this.chkMVP.AutoSize = true;
			this.chkMVP.Location = new System.Drawing.Point(238, 139);
			this.chkMVP.Name = "chkMVP";
			this.chkMVP.Size = new System.Drawing.Size(49, 15);
			this.chkMVP.TabIndex = 21;
			this.chkMVP.Text = "MVP";
			this.chkMVP.UseVisualStyleBackColor = false;
			// 
			// chkFlagship
			// 
			this.chkFlagship.AutoSize = true;
			this.chkFlagship.Location = new System.Drawing.Point(238, 114);
			this.chkFlagship.Name = "chkFlagship";
			this.chkFlagship.Size = new System.Drawing.Size(47, 15);
			this.chkFlagship.TabIndex = 20;
			this.chkFlagship.Text = "기함";
			this.chkFlagship.UseVisualStyleBackColor = false;
			// 
			// textTimes
			// 
			this.textTimes.Font = new System.Drawing.Font("맑은 고딕", 9F);
			this.textTimes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.textTimes.Lines = new string[] {
        "0"};
			this.textTimes.Location = new System.Drawing.Point(174, 211);
			this.textTimes.MaxLength = 32767;
			this.textTimes.Name = "textTimes";
			this.textTimes.PasswordChar = '\0';
			this.textTimes.PromptColor = System.Drawing.Color.FromArgb(((int)(((byte)(152)))), ((int)(((byte)(152)))), ((int)(((byte)(152)))));
			this.textTimes.ReadOnly = true;
			this.textTimes.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.textTimes.SelectedText = "";
			this.textTimes.SelectionLength = 0;
			this.textTimes.SelectionStart = 0;
			this.textTimes.ShortcutsEnabled = true;
			this.textTimes.Size = new System.Drawing.Size(40, 25);
			this.textTimes.TabIndex = 19;
			this.textTimes.Text = "0";
			// 
			// textMapExp
			// 
			this.textMapExp.Font = new System.Drawing.Font("맑은 고딕", 9F);
			this.textMapExp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.textMapExp.Lines = new string[] {
        "0"};
			this.textMapExp.Location = new System.Drawing.Point(89, 211);
			this.textMapExp.MaxLength = 32767;
			this.textMapExp.Name = "textMapExp";
			this.textMapExp.PasswordChar = '\0';
			this.textMapExp.PromptColor = System.Drawing.Color.FromArgb(((int)(((byte)(152)))), ((int)(((byte)(152)))), ((int)(((byte)(152)))));
			this.textMapExp.ReadOnly = true;
			this.textMapExp.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.textMapExp.SelectedText = "";
			this.textMapExp.SelectionLength = 0;
			this.textMapExp.SelectionStart = 0;
			this.textMapExp.ShortcutsEnabled = true;
			this.textMapExp.Size = new System.Drawing.Size(64, 25);
			this.textMapExp.TabIndex = 17;
			this.textMapExp.Text = "0";
			// 
			// textLeftExp
			// 
			this.textLeftExp.Font = new System.Drawing.Font("맑은 고딕", 9F);
			this.textLeftExp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this.textLeftExp.Lines = new string[] {
        "0"};
			this.textLeftExp.Location = new System.Drawing.Point(89, 246);
			this.textLeftExp.MaxLength = 32767;
			this.textLeftExp.Name = "textLeftExp";
			this.textLeftExp.PasswordChar = '\0';
			this.textLeftExp.PromptColor = System.Drawing.Color.FromArgb(((int)(((byte)(152)))), ((int)(((byte)(152)))), ((int)(((byte)(152)))));
			this.textLeftExp.ReadOnly = true;
			this.textLeftExp.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.textLeftExp.SelectedText = "";
			this.textLeftExp.SelectionLength = 0;
			this.textLeftExp.SelectionStart = 0;
			this.textLeftExp.ShortcutsEnabled = true;
			this.textLeftExp.Size = new System.Drawing.Size(64, 25);
			this.textLeftExp.TabIndex = 16;
			this.textLeftExp.Text = "0";
			// 
			// comboRank
			// 
			this.comboRank.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.comboRank.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(39)))), ((int)(((byte)(47)))));
			this.comboRank.DropDownHeight = 361;
			this.comboRank.DropDownWidth = 130;
			this.comboRank.Font = new System.Drawing.Font("맑은 고딕", 9F);
			this.comboRank.FormattingEnabled = true;
			this.comboRank.IntegralHeight = false;
			this.comboRank.ItemHeight = 19;
			this.comboRank.Location = new System.Drawing.Point(164, 169);
			this.comboRank.Name = "comboRank";
			this.comboRank.Size = new System.Drawing.Size(64, 25);
			this.comboRank.TabIndex = 10;
			// 
			// comboMap
			// 
			this.comboMap.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.comboMap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(39)))), ((int)(((byte)(47)))));
			this.comboMap.DropDownHeight = 361;
			this.comboMap.DropDownWidth = 130;
			this.comboMap.Font = new System.Drawing.Font("맑은 고딕", 9F);
			this.comboMap.FormattingEnabled = true;
			this.comboMap.IntegralHeight = false;
			this.comboMap.ItemHeight = 19;
			this.comboMap.Location = new System.Drawing.Point(49, 169);
			this.comboMap.Name = "comboMap";
			this.comboMap.Size = new System.Drawing.Size(64, 25);
			this.comboMap.TabIndex = 8;
			// 
			// comboGoalLevel
			// 
			this.comboGoalLevel.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.comboGoalLevel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(39)))), ((int)(((byte)(47)))));
			this.comboGoalLevel.DropDownHeight = 361;
			this.comboGoalLevel.DropDownWidth = 130;
			this.comboGoalLevel.Font = new System.Drawing.Font("맑은 고딕", 9F);
			this.comboGoalLevel.FormattingEnabled = true;
			this.comboGoalLevel.IntegralHeight = false;
			this.comboGoalLevel.ItemHeight = 19;
			this.comboGoalLevel.Location = new System.Drawing.Point(77, 134);
			this.comboGoalLevel.Name = "comboGoalLevel";
			this.comboGoalLevel.Size = new System.Drawing.Size(64, 25);
			this.comboGoalLevel.TabIndex = 6;
			// 
			// comboStartLevel
			// 
			this.comboStartLevel.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.comboStartLevel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(39)))), ((int)(((byte)(47)))));
			this.comboStartLevel.DropDownHeight = 361;
			this.comboStartLevel.DropDownWidth = 130;
			this.comboStartLevel.Font = new System.Drawing.Font("맑은 고딕", 9F);
			this.comboStartLevel.FormattingEnabled = true;
			this.comboStartLevel.IntegralHeight = false;
			this.comboStartLevel.ItemHeight = 19;
			this.comboStartLevel.Location = new System.Drawing.Point(77, 99);
			this.comboStartLevel.Name = "comboStartLevel";
			this.comboStartLevel.Size = new System.Drawing.Size(64, 25);
			this.comboStartLevel.TabIndex = 4;
			// 
			// comboShip
			// 
			this.comboShip.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.comboShip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(39)))), ((int)(((byte)(47)))));
			this.comboShip.DropDownHeight = 361;
			this.comboShip.DropDownWidth = 130;
			this.comboShip.Font = new System.Drawing.Font("맑은 고딕", 9F);
			this.comboShip.FormattingEnabled = true;
			this.comboShip.IntegralHeight = false;
			this.comboShip.ItemHeight = 19;
			this.comboShip.Location = new System.Drawing.Point(12, 64);
			this.comboShip.Name = "comboShip";
			this.comboShip.Size = new System.Drawing.Size(160, 25);
			this.comboShip.TabIndex = 2;
			// 
			// catalogCalc
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(72)))));
			this.ClientSize = new System.Drawing.Size(368, 297);
			this.Controls.Add(this.textGoalExp);
			this.Controls.Add(this.textCurrentExp);
			this.Controls.Add(this.chkMVP);
			this.Controls.Add(this.chkFlagship);
			this.Controls.Add(this.textTimes);
			this.Controls.Add(this.labelTimes);
			this.Controls.Add(this.textMapExp);
			this.Controls.Add(this.textLeftExp);
			this.Controls.Add(this.labelLeftExp);
			this.Controls.Add(this.labelMapExp);
			this.Controls.Add(this.labelSeparator1);
			this.Controls.Add(this.labelSeparator2);
			this.Controls.Add(this.comboRank);
			this.Controls.Add(this.labelRank);
			this.Controls.Add(this.comboMap);
			this.Controls.Add(this.labelMap);
			this.Controls.Add(this.comboGoalLevel);
			this.Controls.Add(this.labelGoal);
			this.Controls.Add(this.comboStartLevel);
			this.Controls.Add(this.labelStart);
			this.Controls.Add(this.comboShip);
			this.Controls.Add(this.labelInfo2);
			this.Controls.Add(this.labelInfo1);
			this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.ForeColor = System.Drawing.Color.White;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "catalogCalc";
			this.Text = "계산기";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelInfo1;
		private System.Windows.Forms.Label labelInfo2;
		private BeerViewer.Views.Controls.FlatComboBox comboShip;
		private System.Windows.Forms.Label labelStart;
		private BeerViewer.Views.Controls.FlatComboBox comboStartLevel;
		private BeerViewer.Views.Controls.FlatComboBox comboGoalLevel;
		private System.Windows.Forms.Label labelGoal;
		private BeerViewer.Views.Controls.FlatComboBox comboMap;
		private System.Windows.Forms.Label labelMap;
		private BeerViewer.Views.Controls.FlatComboBox comboRank;
		private System.Windows.Forms.Label labelRank;
		private System.Windows.Forms.Label labelSeparator2;
		private System.Windows.Forms.Label labelSeparator1;
		private System.Windows.Forms.Label labelLeftExp;
		private System.Windows.Forms.Label labelMapExp;
		private Controls.FlatTextBox textLeftExp;
		private Controls.FlatTextBox textMapExp;
		private System.Windows.Forms.Label labelTimes;
		private Controls.FlatTextBox textTimes;
		private Controls.FlatCheckBox chkFlagship;
		private Controls.FlatCheckBox chkMVP;
		private Controls.FlatTextBox textCurrentExp;
		private Controls.FlatTextBox textGoalExp;
	}
}