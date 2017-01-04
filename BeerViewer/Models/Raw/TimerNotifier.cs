using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Timer = System.Timers.Timer;
using ElapsedEventHandler = System.Timers.ElapsedEventHandler;

namespace BeerViewer.Models.Raw
{
	public class TimerNotifier : Notifier, IDisposable
	{
		#region Static Members
		private static readonly Timer timer;

		static TimerNotifier()
		{
			timer = new Timer();
			timer.Interval = 1000;
			timer.Start();
		}
		#endregion

		private readonly ElapsedEventHandler EventHandler;

		public TimerNotifier()
		{
			EventHandler = (o, e) => this.Tick();
			timer.Elapsed += EventHandler;
			this.Tick();
		}
		protected virtual void Tick() { }

		public virtual void Dispose()
		{
			timer.Elapsed -= EventHandler;
		}
	}
}
