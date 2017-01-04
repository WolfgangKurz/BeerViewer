using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using BeerViewer.Win32;

namespace BeerViewer.Views.Controls
{
	[ToolboxBitmap(typeof(TextBox))]
	public class FlatTextBox : Control
	{
		private PromptedTextBox baseTextBox;

		#region PromptText
		[Browsable(true)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[DefaultValue("")]
		[Category("Appearance")]
		public string PromptText
		{
			get { return baseTextBox.PromptText; }
			set { baseTextBox.PromptText = value; }
		}
		#endregion

		#region Multiline
		[DefaultValue(false)]
		public bool Multiline
		{
			get { return baseTextBox.Multiline; }
			set { baseTextBox.Multiline = value; }
		}
		#endregion

		#region Text
		public override string Text
		{
			get { return baseTextBox.Text; }
			set { baseTextBox.Text = value; }
		}
		#endregion

		#region PromptColor
		[Category("Appearance")]
		public Color PromptColor
		{
			get { return baseTextBox.PromptColor; }
			set { baseTextBox.PromptColor = value; }
		}
		#endregion

		#region Lines
		public string[] Lines
		{
			get { return baseTextBox.Lines; }
			set { baseTextBox.Lines = value; }
		}
		#endregion

		#region SelectedText
		[Browsable(false)]
		public string SelectedText
		{
			get { return baseTextBox.SelectedText; }
			set { baseTextBox.Text = value; }
		}
		#endregion

		#region ReadOnly
		[DefaultValue(false)]
		public bool ReadOnly
		{
			get { return baseTextBox.ReadOnly; }
			set
			{
				baseTextBox.ReadOnly = value;
			}
		}
		#endregion

		#region PasswordChar
		public char PasswordChar
		{
			get { return baseTextBox.PasswordChar; }
			set { baseTextBox.PasswordChar = value; }
		}
		#endregion

		#region UseSystemPasswordChar
		[DefaultValue(false)]
		public bool UseSystemPasswordChar
		{
			get { return baseTextBox.UseSystemPasswordChar; }
			set { baseTextBox.UseSystemPasswordChar = value; }
		}
		#endregion

		#region TextAlign
		[DefaultValue(HorizontalAlignment.Left)]
		public HorizontalAlignment TextAlign
		{
			get { return baseTextBox.TextAlign; }
			set { baseTextBox.TextAlign = value; }
		}
		#endregion

		#region SelectionStart
		public int SelectionStart
		{
			get { return baseTextBox.SelectionStart; }
			set { baseTextBox.SelectionStart = value; }
		}
		#endregion

		#region SelectionLength
		public int SelectionLength
		{
			get { return baseTextBox.SelectionLength; }
			set { baseTextBox.SelectionLength = value; }
		}
		#endregion

		#region TabStop
		[DefaultValue(true)]
		public new bool TabStop
		{
			get { return baseTextBox.TabStop; }
			set { baseTextBox.TabStop = value; }
		}
		#endregion

		#region MaxLength
		public int MaxLength
		{
			get { return baseTextBox.MaxLength; }
			set { baseTextBox.MaxLength = value; }
		}
		#endregion

		#region ScrollBars
		public ScrollBars ScrollBars
		{
			get { return baseTextBox.ScrollBars; }
			set { baseTextBox.ScrollBars = value; }
		}
		#endregion

		#region AutoCompleteMode
		[DefaultValue(AutoCompleteMode.None)]
		public AutoCompleteMode AutoCompleteMode
		{
			get { return baseTextBox.AutoCompleteMode; }
			set { baseTextBox.AutoCompleteMode = value; }
		}
		#endregion

		#region AutoCompleteSource
		[DefaultValue(AutoCompleteSource.None)]
		public AutoCompleteSource AutoCompleteSource
		{
			get { return baseTextBox.AutoCompleteSource; }
			set { baseTextBox.AutoCompleteSource = value; }
		}
		#endregion

		#region AutoCompleteCustomSource
		public AutoCompleteStringCollection AutoCompleteCustomSource
		{
			get { return baseTextBox.AutoCompleteCustomSource; }
			set { baseTextBox.AutoCompleteCustomSource = value; }
		}
		#endregion

		#region ShortcutsEnabled
		public bool ShortcutsEnabled
		{
			get { return baseTextBox.ShortcutsEnabled; }
			set { baseTextBox.ShortcutsEnabled = value; }
		}
		#endregion

		#region BackColor (Hide Value)
		[Browsable(false)]
		public new Color BackColor
		{
			get { return baseTextBox.BackColor; }
			set { baseTextBox.BackColor = value; }
		}
		#endregion

		#region Font
		public new Font Font
		{
			get { return base.Font; }
			set
			{
				base.Font = value;
				baseTextBox.Font = value;
			}
		}
		#endregion

		private bool isHovered = false;

		public FlatTextBox()
		{
			SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);
			base.TabStop = false;

			CreateBaseTextBox();
			UpdateBaseTextBox();
			AddEventHandler();

			this.BackColor = Color.FromArgb(0x27, 0x27, 0x2F);
		}

		#region BaseText
		public event EventHandler AcceptsTabChanged;
		private void BaseTextBoxAcceptsTabChanged(object sender, EventArgs e)
		{
			AcceptsTabChanged?.Invoke(this, e);
		}

		private void BaseTextBoxSizeChanged(object sender, EventArgs e)
		{
			base.OnSizeChanged(e);
		}

		private void BaseTextBoxCursorChanged(object sender, EventArgs e)
		{
			base.OnCursorChanged(e);
		}

		private void BaseTextBoxContextMenuStripChanged(object sender, EventArgs e)
		{
			base.OnContextMenuStripChanged(e);
		}

		private void BaseTextBoxContextMenuChanged(object sender, EventArgs e)
		{
			base.OnContextMenuChanged(e);
		}

		private void BaseTextBoxClientSizeChanged(object sender, EventArgs e)
		{
			base.OnClientSizeChanged(e);
		}

		private void BaseTextBoxClick(object sender, EventArgs e)
		{
			base.OnClick(e);
		}

		private void BaseTextBoxChangeUiCues(object sender, UICuesEventArgs e)
		{
			base.OnChangeUICues(e);
		}

		private void BaseTextBoxCausesValidationChanged(object sender, EventArgs e)
		{
			base.OnCausesValidationChanged(e);
		}

		private void BaseTextBoxKeyUp(object sender, KeyEventArgs e)
		{
			base.OnKeyUp(e);
		}

		private void BaseTextBoxKeyPress(object sender, KeyPressEventArgs e)
		{
			base.OnKeyPress(e);
		}

		private void BaseTextBoxKeyDown(object sender, KeyEventArgs e)
		{
			base.OnKeyDown(e);
		}

		bool _cleared = false;
		bool _withtext = false;

		private void BaseTextBoxTextChanged(object sender, EventArgs e)
		{
			base.OnTextChanged(e);

			if (baseTextBox.Text != "" && !_withtext)
			{
				_withtext = true;
				_cleared = false;
				Invalidate();
			}

			if (baseTextBox.Text == "" && !_cleared)
			{
				_withtext = false;
				_cleared = true;
				Invalidate();
			}
		}

		private void AddEventHandler()
		{
			baseTextBox.AcceptsTabChanged += BaseTextBoxAcceptsTabChanged;

			baseTextBox.CausesValidationChanged += BaseTextBoxCausesValidationChanged;
			baseTextBox.ChangeUICues += BaseTextBoxChangeUiCues;
			baseTextBox.Click += BaseTextBoxClick;
			baseTextBox.ClientSizeChanged += BaseTextBoxClientSizeChanged;
			baseTextBox.ContextMenuChanged += BaseTextBoxContextMenuChanged;
			baseTextBox.ContextMenuStripChanged += BaseTextBoxContextMenuStripChanged;
			baseTextBox.CursorChanged += BaseTextBoxCursorChanged;

			baseTextBox.KeyDown += BaseTextBoxKeyDown;
			baseTextBox.KeyPress += BaseTextBoxKeyPress;
			baseTextBox.KeyUp += BaseTextBoxKeyUp;

			baseTextBox.MouseLeave += (s, e) => OnMouseLeave(e);
			baseTextBox.SizeChanged += BaseTextBoxSizeChanged;

			baseTextBox.TextChanged += BaseTextBoxTextChanged;
			baseTextBox.GotFocus += baseTextBox_GotFocus;
			baseTextBox.LostFocus += baseTextBox_LostFocus;
		}

		void baseTextBox_LostFocus(object sender, EventArgs e)
		{
			Invalidate();
			this.InvokeLostFocus(this, e);
		}

		void baseTextBox_GotFocus(object sender, EventArgs e)
		{
			Invalidate();
			this.InvokeGotFocus(this, e);
		}

		private void UpdateBaseTextBox()
		{
			if (baseTextBox == null) return;

			baseTextBox.Font = this.Font;
			baseTextBox.Location = new Point(6, 4);
			baseTextBox.Size = new Size(Width - 12, Height - 8);
		}

		private class PromptedTextBox : TextBox
		{
			private const int OCM_COMMAND = 0x2111;
			private const int WM_PAINT = 15;

			private bool drawPrompt => this.Text.Length == 0;

			private string promptText = "";
			[Browsable(true)]
			[EditorBrowsable(EditorBrowsableState.Always)]
			[DefaultValue("")]
			public string PromptText
			{
				get { return promptText; }
				set
				{
					promptText = value.Trim();
					Invalidate();
				}
			}

			private Color _PromptColor = Color.FromArgb(0x98, 0x98, 0x98);
			public Color PromptColor
			{
				get { return _PromptColor; }
				set
				{
					_PromptColor = value;
					this.Invalidate();
				}
			}

			public PromptedTextBox()
			{
				SetStyle(ControlStyles.DoubleBuffer |
					ControlStyles.OptimizedDoubleBuffer, true);
			}

			private void DrawTextPrompt()
			{
				using (Graphics graphics = CreateGraphics())
					DrawTextPrompt(graphics);
			}

			private void DrawTextPrompt(Graphics g)
			{
				TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis;
				Rectangle clientRectangle = ClientRectangle;

				switch (TextAlign)
				{
					case HorizontalAlignment.Left:
						clientRectangle.Offset(1, 0);
						break;

					case HorizontalAlignment.Right:
						flags |= TextFormatFlags.Right;
						clientRectangle.Offset(-2, 0);
						break;

					case HorizontalAlignment.Center:
						flags |= TextFormatFlags.HorizontalCenter;
						clientRectangle.Offset(1, 0);
						break;
				}

				SolidBrush drawBrush = new SolidBrush(PromptColor);
				TextRenderer.DrawText(
					g,
					promptText,
					this.Font,
					clientRectangle,
					_PromptColor,
					this.BackColor,
					flags
				);
			}

			protected override void OnPaint(PaintEventArgs e)
			{
				base.OnPaint(e);
				if (drawPrompt)
					DrawTextPrompt(e.Graphics);
			}

			protected override void OnCreateControl()
			{
				base.OnCreateControl();
			}

			protected override void OnTextAlignChanged(EventArgs e)
			{
				base.OnTextAlignChanged(e);
				Invalidate();
			}

			protected override void OnTextChanged(EventArgs e)
			{
				base.OnTextChanged(e);
				Invalidate();
			}

			protected override void WndProc(ref Message m)
			{
				base.WndProc(ref m);
				if (((m.Msg == WM_PAINT) || (m.Msg == OCM_COMMAND)) && (drawPrompt && !GetStyle(ControlStyles.UserPaint)))
					DrawTextPrompt();
			}

			protected override void OnLostFocus(EventArgs e)
			{
				base.OnLostFocus(e);
			}
		}
		#endregion

		public void Select(int start, int length)
		{
			baseTextBox.Select(start, length);
		}

		public void SelectAll()
		{
			baseTextBox.SelectAll();
		}

		public void Clear()
		{
			baseTextBox.Clear();
		}

		public void AppendText(string text)
		{
			baseTextBox.AppendText(text);
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			this.isHovered = true;
			this.Invalidate();
			base.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(EventArgs e)
		{
			var pos = this.PointToClient(Cursor.Position);

			if (!this.ClientRectangle.Contains(pos))
				this.isHovered = false;

			this.Invalidate();
			base.OnMouseLeave(e);
		}
		protected override void OnGotFocus(EventArgs e)
		{
			baseTextBox.Focus();
			this.Invalidate();
			base.OnGotFocus(e);
		}
		protected override void OnLostFocus(EventArgs e)
		{
			this.Invalidate();
			base.OnLostFocus(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			try { OnPaintForeground(e); }
			catch { Invalidate(); }
		}

		protected virtual void OnPaintForeground(PaintEventArgs e)
		{
			var g = e.Graphics;
			if (this.isHovered || this.Focused || this.baseTextBox.Focused)
				BackColor = Color.FromArgb(0x47, 0x47, 0x4F);
			else
				this.BackColor = Color.FromArgb(0x27, 0x27, 0x2F);

			g.Clear(BackColor);
			baseTextBox.ForeColor = Color.FromArgb(0xFF, 0xFF, 0xFF);

			Color borderColor = Color.FromArgb(0x44, 0x44, 0x4A);
			using (Pen p = new Pen(borderColor))
				g.DrawRectangle(p, new Rectangle(0, 0, Width - 1, Height - 1));
		}
		public override void Refresh()
		{
			base.Refresh();
			UpdateBaseTextBox();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			UpdateBaseTextBox();
		}

		[DefaultValue(CharacterCasing.Normal)]
		public CharacterCasing CharacterCasing
		{
			get { return baseTextBox.CharacterCasing; }
			set { baseTextBox.CharacterCasing = value; }
		}

		private void CreateBaseTextBox()
		{
			if (baseTextBox != null) return;

			baseTextBox = new PromptedTextBox();

			baseTextBox.BorderStyle = BorderStyle.None;
			baseTextBox.Font = this.Font;
			baseTextBox.Location = new Point(6, 4);
			baseTextBox.Size = new Size(Width - 12, Height - 8);

			Size = new Size(baseTextBox.Width + 12, baseTextBox.Height + 8);

			baseTextBox.TabStop = true;
			//baseTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;

			Controls.Add(baseTextBox);
			this.BackColor = Color.FromArgb(0x27, 0x27, 0x2F);
		}
	}
}
