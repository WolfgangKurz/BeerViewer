using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BeerViewer.Core;
using BeerViewer.Models;
using BeerViewer.Views.Controls;

namespace BeerViewer.Views.Contents
{
	public partial class FleetsView : UserControl
	{
		public FleetsView()
		{
			InitializeComponent();

			if (!Helper.IsInDesignMode)
				layoutFleets.Controls.Clear();

			Action UpdateLayout = null;
			UpdateLayout = () =>
			{
				if (this.InvokeRequired)
				{
					this.Invoke(UpdateLayout);
					return;
				}

				if (Settings.ContentLayoutMode.Value && Settings.VerticalMode.Value)
					layoutFleets.FlowDirection = FlowDirection.LeftToRight;
				else
					layoutFleets.FlowDirection = FlowDirection.TopDown;
			};
			Settings.VerticalMode.PropertyEvent(nameof(Settings.VerticalMode.Value), () => UpdateLayout());
			Settings.ContentLayoutMode.PropertyEvent(nameof(Settings.ContentLayoutMode.Value), () => UpdateLayout());
			UpdateLayout();
		}

		public void SetHomeport(Homeport homeport)
		{
			if (homeport == null) return;

			// Fleets & Expedition
			homeport.Organization.PropertyEvent(nameof(homeport.Organization.Fleets), () =>
			{
				var fleets = homeport.Organization.Fleets.Select(x => x.Value);

				layoutFleets.Invoke(() =>
				{
					// 기존 목록 제거
					layoutFleets.Controls.Clear();

					foreach (var fleet in fleets.OrderBy(x => x.Id))
					{
						if (fleet == null) continue;

						FleetView fleetView = new FleetView();
						// fleetView.Dock = DockStyle.Top;
						fleetView.AutoSize = true;
						fleetView.Padding = new Padding(0, 0, 8, 8);
						fleetView.SetFleet(fleet);

						layoutFleets.Controls.Add(fleetView);
					}
				});
			}, true);
		}
	}
}
