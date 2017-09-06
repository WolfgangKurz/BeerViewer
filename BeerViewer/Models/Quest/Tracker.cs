using System;
using System.Linq;

using BeerViewer.Models.Enums;

namespace BeerViewer.Models.Quest
{
	public abstract class Tracker
	{
		public abstract QuestType Type { get; }
		public abstract int Id { get; }

		public bool IsTracking { get; set; }

		public abstract void RegisterEvent(TrackManager manager);
		public abstract void ResetQuest();

		public event EventHandler ProcessChanged;
		protected void OnProcessChanged()
			=> this.ProcessChanged?.Invoke(this, EventArgs.Empty);

		public abstract int GetCurrent();
		public abstract int GetMaximum();

		public abstract string SerializeData();
		public abstract void DeserializeData(string data);
	}
	public abstract class CountableTracker : Tracker
	{
		protected QuestProgress lastProgress { get; set; }
		public abstract int Maximum { get; }

		#region Current Property
		private int _Current;
		public int Current
		{
			get { return this._Current; }
			protected set
			{
				if (this._Current != value)
				{
					this._Current = value;
					this.OnProcessChanged();
				}
			}
		}
		#endregion

		public override void ResetQuest() => this.Current = 0;

		public override int GetCurrent() => this.Current;
		public override int GetMaximum() => this.Maximum;

		public override string SerializeData() => this.Current.ToString();
		public override void DeserializeData(string data)
		{
			int v = 0;
			int.TryParse(data, out v);
			this.Current = v;
		}

		protected virtual int CheckCut50 => (int)Math.Ceiling(this.Maximum * 0.5);
		protected virtual int CheckCut80 => (int)Math.Ceiling(this.Maximum * 0.8);

		protected void CheckUnderOver(QuestProgress progress)
		{
			if (this.lastProgress == progress) return;
			this.lastProgress = progress;

			int cut50 = this.CheckCut50;
			int cut80 = this.CheckCut80;

			switch (progress)
			{
				case QuestProgress.None:
					if (this.Current >= cut50)
						this.Current = cut50 - 1;
					break;
				case QuestProgress.Progress50:
					if (this.Current >= cut80)
						this.Current = cut80 - 1;
					else if (this.Current < cut50)
						this.Current = cut50;
					break;
				case QuestProgress.Progress80:
					if (this.Current < cut80)
						this.Current = cut80;
					break;
				case QuestProgress.Complete:
					this.Current = this.Maximum;
					break;
			}
			this.OnProcessChanged();
		}
	}
	public abstract class MultipleTracker : Tracker
	{
		protected class EventArray<T>
		{
			private Action _caller;
			private T[] _array;

			public T this[int idx]
			{
				get { return _array[idx]; }
				set
				{
					if(object.Equals(this[idx], value))
					{
						_array[idx] = value;
						_caller?.Invoke();
					}
				}
			}
			public T[] Array => _array;

			public EventArray(int Length, Action caller)
			{
				_array = new T[Length];
				_caller = caller;
			}
			public EventArray(T[] Origin, Action caller)
			{
				_array = Origin;
				_caller = caller;
			}

			public void Reset()
				=> _array = new T[_array.Length];
		}

		protected QuestProgress lastProgress { get; set; }
		public abstract int[] Maximum { get; }

		#region Current Property
		protected EventArray<int> Current { get; private set; }
		public int[] CurrentArray
		{
			get { return this.Current.Array; } // Cannot set
		}
		#endregion

		public MultipleTracker()
		{
			this.Current = new EventArray<int>(Maximum.Length, OnProcessChanged);
		}

		public override void ResetQuest() => this.Current.Reset();

		public override int GetCurrent() => this.CurrentArray.Sum();
		public override int GetMaximum() => this.Maximum.Sum();

		public override string SerializeData() => string.Join(",", this.CurrentArray);
		public override void DeserializeData(string data)
		{
			var parts = data
				.Split(new char[] { ',' })
				.Select(x =>
				{
					int r = 0;
					bool s = int.TryParse(x, out r);
					return new { r = r, s = s };
				})
				.Where(x => x.s)
				.Select(x => x.r)
				.ToArray();

			if (parts.Length != this.Maximum.Length) return; // Size not match

			this.Current = new EventArray<int>(parts, OnProcessChanged);
		}
	}
}
