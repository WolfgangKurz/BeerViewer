namespace BeerViewer.Views.Catalogs
{
	partial class catalogSlotitems
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(catalogSlotitems));
			this.slotitemListTable = new BeerViewer.Views.Controls.SlotitemListTable();
			this.SuspendLayout();
			// 
			// slotitemListTable
			// 
			this.slotitemListTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.slotitemListTable.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.slotitemListTable.Location = new System.Drawing.Point(8, 8);
			this.slotitemListTable.Name = "slotitemListTable";
			this.slotitemListTable.Size = new System.Drawing.Size(679, 491);
			this.slotitemListTable.TabIndex = 1;
			// 
			// catalogSlotitems
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(72)))));
			this.ClientSize = new System.Drawing.Size(695, 507);
			this.Controls.Add(this.slotitemListTable);
			this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.ForeColor = System.Drawing.Color.White;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "catalogSlotitems";
			this.Padding = new System.Windows.Forms.Padding(8);
			this.Text = "소유 장비 목록";
			this.ResumeLayout(false);

		}

		#endregion

		private Controls.SlotitemListTable slotitemListTable;
	}
}