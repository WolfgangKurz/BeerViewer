namespace BeerViewer.Models
{
	public struct LimitedValue
	{
		public double Percentage
			=> (Maximum - Minimum) != 0
				? (double)(Current - Minimum) / (Maximum - Minimum)
				: 0;

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
			=> new LimitedValue(cur, this.Maximum, this.Minimum);
	}
	
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
