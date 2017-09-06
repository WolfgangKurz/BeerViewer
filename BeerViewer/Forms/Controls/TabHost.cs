using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using BeerViewer.Framework;

namespace BeerViewer.Forms.Controls
{
	internal class TabHost : FrameworkControl
	{
		internal class TabItem
		{
			public string Name { get; set; }
			public FrameworkControl LinkedControl { get; set; }

			public TabItem() { }
			public TabItem(string Name, FrameworkControl LinkedControl) : this()
			{
				this.Name = Name;
				this.LinkedControl = LinkedControl;
			}
		}

		public IReadOnlyList<TabItem> TabItems { get; protected set; }
		protected double TabWidth => (double)this.Width / (this.TabItems.Count == 0 ? 1 : this.TabItems.Count);

		#region TabIndex
		public int TabIndex
		{
			get { return this._TabIndex; }
			set
			{
				if (this._TabIndex != value)
				{
					this._TabIndex = value;
					this.Invalidate();

					TabItems.ForEach(x =>
					{
						if (x.LinkedControl != null)
							x.LinkedControl.Visible = false;
					});
					if (TabItems[value].LinkedControl != null)
						TabItems[value].LinkedControl.Visible = true;

					this.OnTabIndexChanged();
				}
			}
		}
		private int _TabIndex { get; set; }

		public event EventHandler TabIndexChanged;
		#endregion


		#region Initializers
		public TabHost() : base()
			=> this.Initialize();

		public TabHost(FrameworkRenderer Renderer) : base(Renderer)
			=> this.Initialize();

		public TabHost(int X, int Y) : base(X, Y)
			=> this.Initialize();

		public TabHost(FrameworkRenderer Renderer, int X, int Y) : base(Renderer, X, Y)
			=> this.Initialize();

		public TabHost(int X, int Y, int Width, int Height) : base(X, Y, Width, Height)
			=> this.Initialize();

		public TabHost(FrameworkRenderer Renderer, int X, int Y, int Width, int Height) : base(Renderer, X, Y, Width, Height)
			=> this.Initialize();
		#endregion

		private void Initialize()
		{
			Point hoverPoint = Point.Empty;
			int HoverIndex = -1;

			this.MouseMove += (s, e) =>
			{
				var idx = (int)(e.X / this.TabWidth);
				if (idx != HoverIndex)
				{
					HoverIndex = idx;
					this.Invalidate();
				}

				hoverPoint = new Point(e.X, e.Y);

				if (e.Button == MouseButtons.Left)
					this.TabIndex = idx.InRange(0, this.TabItems.Count - 1);
			};
			this.MouseDown += (s, e) =>
			{
				int idx = (int)(e.X / this.TabWidth);
				this.TabIndex = idx.InRange(0, this.TabItems.Count - 1);
			};

			this.Paint += (s, e) =>
			{
				var g = e.Graphics;

				g.FillRectangle(Constants.brushHoverFace, this.ClientBound);

				for (var i = 0; i < TabItems.Count; i++)
				{
					var x = TabItems[i];
					var ItemBound = new Rectangle(
						(int)(i * TabWidth), 0,
						(int)TabWidth, this.Height
					);

					if (this.TabIndex == i || (this.IsHover && HoverIndex == i))
						g.FillRectangle(Constants.brushActiveFace, ItemBound);

					g.DrawString(
						x.Name,
						Constants.fontDefault,
						Brushes.White,
						ItemBound,
						new StringFormat
						{
							FormatFlags = StringFormatFlags.NoWrap,
							Alignment = StringAlignment.Center,
							LineAlignment = StringAlignment.Center
						}
					);
				}
			};

			this.TabItems = new List<TabItem>();
		}

		public int AddTab(TabItem item)
		{
			(this.TabItems as List<TabItem>).Add(item);
			this.Invalidate();

			return this.TabItems.Count;
		}
		public int RemoveTab(int index)
		{
			(this.TabItems as List<TabItem>).RemoveAt(index);

			if (this.TabIndex >= this.TabItems.Count)
				this.TabIndex = this.TabItems.Count - 1;

			this.Invalidate();

			return this.TabItems.Count;
		}

		public void OnTabIndexChanged()
		{
			this.TabIndexChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}
