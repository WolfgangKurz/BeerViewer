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
		}

		public void SetHomeport(Homeport homeport)
		{
			if (homeport == null) return;

			// Fleets & Expedition
			homeport.Organization.PropertyEvent(nameof(homeport.Organization.Fleets), () =>
			{
				var fleets = homeport.Organization.Fleets.Select(x => x.Value);

				this.Invoke(() =>
				{
					// 기존 목록 제거
					layoutFleets.Controls.Clear();

					foreach (var fleet in fleets.OrderBy(x => x.Id))
					{
						if (fleet == null) continue;

						FleetView fleetView = new FleetView();
						// fleetView.Dock = DockStyle.Top;
						fleetView.AutoSize = true;
						fleetView.Padding = new Padding { Bottom = 8 };
						fleetView.SetFleet(fleet);

						layoutFleets.Controls.Add(fleetView);
					}
				});
			}, true);
		}
	}
}
