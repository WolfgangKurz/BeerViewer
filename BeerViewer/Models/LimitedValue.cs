using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models
{
	/// <summary>
	/// 상한이 있는 데이터
	/// </summary>
	public struct LimitedValue
	{
		public int Current { get; }
		public int Maximum { get; }
		
		public int Minimum { get; }

		public LimitedValue(int cur, int max, int min) : this()
		{
			this.Current = cur;
			this.Maximum = max;
			this.Minimum = min;
		}

		public LimitedValue Update(int cur)
		{
			return new LimitedValue(cur, this.Maximum, this.Minimum);
		}
	}

	/// <summary>
	/// 상한이 있는 데이터
	/// </summary>
	public struct LimitedValue<T> where T : struct
	{
		public T Current { get; private set; }
		public T Maximum { get; private set; }
		public T Minimum { get; private set; }

		public LimitedValue(T cur, T max, T min) : this()
		{
			this.Current = cur;
			this.Maximum = max;
			this.Minimum = min;
		}
	}
}
