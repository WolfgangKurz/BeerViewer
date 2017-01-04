using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

using BeerViewer.Core;
using BeerViewer.Views.Catalogs;

namespace BeerViewer
{
	public partial class frmMain : Form
	{
		public static frmMain Instance { get; private set; }
		public string CurrentTab { get; private set; }

		private Dictionary<Label, Control> tabList { get; set; }
		public void UpdateTab(string TabName)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(() => UpdateTab(TabName));
				return;
			}

			CurrentTab = TabName;
			foreach (var item in tabList)
			{
				if (layoutTab.Controls.Contains(item.Value))
					layoutTab.Controls.Remove(item.Value);

				item.Key.BackColor = Color.Transparent;
				item.Value.Visible = false;
			}

			var TabButton = tabList.FirstOrDefault(x => TabName == x.Key.Name.Substring(3)).Key;
			if (TabButton == null) return;

			TabButton.BackColor = Color.FromArgb(81, 117, 142);
			tabList[TabButton].Visible = true;

			layoutTab.Controls.Add(tabList[TabButton]);
			layoutTab.SetRow(tabList[TabButton], 1);
		}
		public void BuildTab(Label x, Control y)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(() => BuildTab(x, y));
				return;
			}

			var TabName = x.Name.Substring(3); // tabXXXX -> XXXX
			x.BackColor = Color.Transparent;
			x.Cursor = Cursors.Hand;
			tabList.Add(x, y);

			x.MouseEnter += (s, e) =>
			{
				if (CurrentTab == TabName) return;
				x.BackColor = Color.FromArgb(48, 144, 144, 144);
			};
			x.MouseLeave += (s, e) =>
			{
				if (CurrentTab == TabName) return;
				x.BackColor = Color.Transparent;
			};
			x.MouseDown += (s, e) => UpdateTab(TabName);
		}

		public frmMain()
		{
			frmMain.Instance = this;
			MessageProvider.Instance.SetProvider(this);

			InitializeComponent();
			InitializeTabs();

			Helper.SetRegistryFeatureBrowserEmulation();
			Helper.PrepareBrowser(this.browserMain);
			panelBrowser.Resize += (s, e) =>
				browserMain.Left = panelBrowser.ClientSize.Width / 2 - browserMain.ClientSize.Width / 2;

			Translator.Initialize();

			DataStorage.Instance.Initialize();
			DataStorage.Instance.PropertyEvent(nameof(DataStorage.Initialized), () =>
			{
				this.contentGeneral.SetHomeport(DataStorage.Instance.Homeport);
				this.contentFleets.SetHomeport(DataStorage.Instance.Homeport);
			});

			Settings.VerticalMode.PropertyEvent(nameof(Settings.VerticalMode.Value), () =>
			{
				Action LayoutChange = () =>
				{
					if (Settings.VerticalMode.Value)
						panelBrowser.Dock = DockStyle.Top;
					else
						panelBrowser.Dock = DockStyle.Left;
				};

				if (this.InvokeRequired)
					this.Invoke(LayoutChange);
				else
					LayoutChange();
			}, true);
		}
		private void InitializeTabs()
		{
			tabList = new Dictionary<Label, Control>();

			CurrentTab = "";
			BuildTab(tabGeneral, contentGeneral);
			BuildTab(tabFleets, contentFleets);
			BuildTab(tabBattle, contentBattle);
			BuildTab(tabSettings, contentSettings);
			UpdateTab("General");
		}

		private void btnScreenshot_Click(object sender, EventArgs e)
		{
			var Captured = Helper.Capture(this.browserMain);
			if (Captured == null)
				MessageProvider.Instance.Submit("스크린샷 저장에 실패했습니다", "BeerViewer");

			else
			{
				var filename = $"BeerViewer-{DateTimeOffset.Now.LocalDateTime.ToString("yyyyMMdd-HHmmss-ff")}.png";
				Captured.Save(filename, ImageFormat.Png);

				MessageProvider.Instance.Submit("스크린샷을 저장했습니다: " + filename, "BeerViewer");
			}

			Helper.SetCritical(true);
		}
	}
}
