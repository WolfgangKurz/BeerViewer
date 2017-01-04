using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BeerViewer.Models;

namespace BeerViewer.Views.Catalogs
{
	public partial class catalogSlotitems : Form
	{
		private Homeport homeport { get; set; }

		public catalogSlotitems()
		{
			InitializeComponent();
		}

		public void SetHomeport(Homeport homeport)
		{
			this.homeport = homeport;
			if (homeport == null) return;

			homeport.Organization.PropertyEvent(nameof(homeport.Organization.Ships), () => slotitemListTable.SetHomeport(this.homeport), true);
		}
	}
}
