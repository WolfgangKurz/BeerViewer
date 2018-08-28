﻿using System;

using Timer = System.Timers.Timer;
using ElapsedEventHandler = System.Timers.ElapsedEventHandler;

namespace BeerViewer.Models.Raw
{
	public class TimerNotifier : DisposableNotifier, IDisposable
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

		public new virtual void Dispose()
		{
			timer.Elapsed -= EventHandler;
			base.Dispose();
		}
	}
}
