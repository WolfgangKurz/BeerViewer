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
using BeerViewer.Models;

namespace BeerViewer.Views.Catalogs
{
	public partial class catalogCalc : Form
	{
		private class DisplayShip
		{
			public Ship Ship { get; set; }

			public override string ToString()
			{
				return Ship?.LvName;
			}
		}

		public static Dictionary<string, int> MapExpTable = new Dictionary<string, int>
		{
			{"1-1", 30}, {"1-2", 50}, {"1-3", 80}, {"1-4", 100}, {"1-5", 150},
			{"2-1", 120}, {"2-2", 150}, {"2-3", 200},{"2-4", 300},{"2-5", 250},
			{"3-1", 310}, {"3-2", 320}, {"3-3", 330}, {"3-4", 350},{"3-5",400},
			{"4-1", 310}, {"4-2", 320}, {"4-3", 330}, {"4-4", 340},
			{"5-1", 360}, {"5-2", 380}, {"5-3", 400}, {"5-4", 420}, {"5-5", 450},
			{"6-1", 380}, {"6-2", 420}
		};
		public string[] Ranks = new string[] { "S", "A", "B", "C", "D", "E" };

		private Homeport homeport { get; set; }
		private Ship CurrentShip => (comboShip.SelectedItem as DisplayShip)?.Ship;

		public catalogCalc()
		{
			InitializeComponent();

			foreach (var rank in Ranks)
				this.comboRank.Items.Add(rank);
			this.comboRank.SelectedIndex = 0;

			foreach (var Map in MapExpTable)
				this.comboMap.Items.Add(Map.Key);
			this.comboMap.SelectedIndex = 0;

			for (var i = 1; i <= 155; i++)
			{
				this.comboStartLevel.Items.Add(i.ToString());
				this.comboGoalLevel.Items.Add(i.ToString());
			}

			this.comboShip.PromptText = "칸무스를 선택하세요";
			this.comboShip.SelectedIndexChanged += (s, e) => SelectShip();

			this.comboRank.SelectedIndexChanged += (s, e) => Calculate();
			this.comboMap.SelectedIndexChanged += (s, e) => Calculate();
			this.comboStartLevel.SelectedIndexChanged += (s, e) => SelectLevel();
			this.comboGoalLevel.SelectedIndexChanged += (s, e) => SelectLevel();
			chkFlagship.CheckedChanged += (s, e) => Calculate();
			chkMVP.CheckedChanged += (s, e) => Calculate();
		}
		public void SetHomeport(Homeport homeport)
		{
			this.homeport = homeport;
			if (homeport == null) return;

			homeport.Organization.PropertyEvent(nameof(homeport.Organization.Ships), () =>
			{
				Action UpdateShips = () =>
				{
					var list = homeport.Organization.Ships.OrderByDescending(x => x.Value.Exp);
					comboShip.Items.Clear();
					foreach (var ship in list)
						comboShip.Items.Add(new DisplayShip { Ship = ship.Value });
				};

				if (this.InvokeRequired)
					this.Invoke(UpdateShips);
				else
					UpdateShips();
			}, true);
		}

		private void SelectShip()
		{
			this.comboStartLevel.SelectedIndex = this.CurrentShip.Level - 1; ;
			if (this.CurrentShip.Info.NextRemodelingLevel.HasValue)
			{
				if (this.CurrentShip.Info.NextRemodelingLevel.Value > this.comboStartLevel.SelectedIndex + 1)
					this.comboGoalLevel.SelectedIndex = this.CurrentShip.Info.NextRemodelingLevel.Value - 1;
			}
			else
				this.comboGoalLevel.SelectedIndex = Math.Min(this.CurrentShip.Level + 1, 155) - 1;

			this.textCurrentExp.Text = this.CurrentShip.Exp.ToString();
			SelectLevel();
		}
		private void SelectLevel()
		{
			int CurrentLevel = 0, GoalLevel = 0;
			CurrentLevel = (this.comboStartLevel.SelectedIndex + 1);
			GoalLevel = (this.comboGoalLevel.SelectedIndex + 1);

			if (CurrentLevel > 0)
				this.textCurrentExp.Text = Experience.shipTable[CurrentLevel].Total.ToString();
			if (GoalLevel > 0)
				this.textGoalExp.Text = Experience.shipTable[GoalLevel].Total.ToString();

			Calculate();
		}

		private void Calculate()
		{
			int CurrentLevel = 0, GoalLevel = 0;
			int GoalExp = 0, CurrentExp = 0;
			int SortieExp = 0, RemainingExp = 0, RunCount = 0;

			bool b1 = int.TryParse(this.textGoalExp.Text, out GoalExp);
			bool b2 = int.TryParse(this.textCurrentExp.Text, out CurrentExp);
			bool b3 = comboStartLevel.SelectedIndex >= 0;
			bool b4 = comboGoalLevel.SelectedIndex >= 0;

			if (!b1 || !b2 || !b3 || !b4 || (GoalExp < CurrentExp))
				return;

			CurrentLevel = (this.comboStartLevel.SelectedIndex + 1);
			GoalLevel = (this.comboGoalLevel.SelectedIndex + 1);

			var RankTable = new Dictionary<string, double>
				{
					{"S", 1.2 },
					{"A", 1.0 },
					{"B", 1.0 },
					{"C", 0.8 },
					{"D", 0.7 },
					{"E", 0.5 }
				};

			// Lawl at that this inline conditional.
			double Multiplier = (this.chkFlagship.Checked ? 1.5 : 1)
				* (this.chkMVP.Checked ? 2 : 1)
				* RankTable[this.comboRank.SelectedItem as string];

			SortieExp = (int)Math.Round(
				MapExpTable[this.comboMap.SelectedItem as string] * Multiplier,
				0,
				MidpointRounding.AwayFromZero
			);
			RemainingExp = GoalExp - CurrentExp;
			RunCount = Convert.ToInt32(Math.Ceiling(
				Convert.ToDecimal(RemainingExp) / Convert.ToDecimal(SortieExp)
			));

			this.textMapExp.Text = SortieExp.ToString();
			this.textLeftExp.Text = RemainingExp.ToString();
			this.textTimes.Text = RunCount.ToString();
		}
	}
}
