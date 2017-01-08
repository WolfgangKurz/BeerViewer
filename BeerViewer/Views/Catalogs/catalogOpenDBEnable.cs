using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BeerViewer.Core;

namespace BeerViewer.Views.Catalogs
{
	public partial class catalogOpenDBEnable : Form
	{
		public catalogOpenDBEnable()
		{
			InitializeComponent();

			this.btnUse.Click += (s, e) =>
			{
				Settings.OpenDB_Enabled.Value = true;
				this.Close();
			};
			this.btnNotUse.Click += (s, e) =>
			{
				Settings.OpenDB_Enabled.Value = false;
				this.Close();
			};
		}
	}
}
