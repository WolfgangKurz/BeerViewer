using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models
{
	/// <summary>
	/// 근대화개수 가능한지 볼 수 있는 형식
	/// </summary>
	public struct ModernizableStatus
	{
		public int Max { get; internal set; }
		public int Default { get; internal set; }
		public int Upgraded { get; internal set; }

		public int Current => this.Default + this.Upgraded;

		public int Shortfall => this.Max - this.Current;
		public bool IsMax => this.Current >= this.Max;

		internal ModernizableStatus(int[] status, int upgraded) : this()
		{
			if (status.Length == 2)
			{
				this.Default = status[0];
				this.Max = status[1];
			}
			this.Upgraded = upgraded;
		}

		public ModernizableStatus Update(int upgraded)
		{
			return new ModernizableStatus(new int[] { this.Default, this.Max }, upgraded);
		}

		public override string ToString()
		{
			return $"Status = {this.Default}->{this.Max}, Current = {this.Current}{(this.IsMax ? "(max)" : "")}";
		}

		#region Static Members
		public static ModernizableStatus Dummy { get; } = new ModernizableStatus(new[] { -1, -1 }, 0);
		#endregion
	}
}
