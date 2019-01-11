using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BeerViewer.Modules
{
	public class Logger
	{
		public delegate void LogEventHandler(string Log);
		public static event LogEventHandler Logged;

		private static Logger Instance { get; } = new Logger();

		private Dictionary<string, Queue<string>> RegisteredLoggers { get; }

		private Logger()
		{
			this.RegisteredLoggers = new Dictionary<string, Queue<string>>();
		}

		private void _Log(string Log)
		{
			var _Log = string.Format(
				"[{0}] {1}",
				DateTime.Now.ToString("HH:mm:ss"),
				Log
			);

			Logger.Logged?.Invoke(_Log);
			Debug.WriteLine(_Log);

			foreach (var kv in this.RegisteredLoggers)
				kv.Value.Enqueue(_Log);
		}
		private void _Log(string format, params object[] args)
			=> this._Log(string.Format(format, args));


		public static void Log(string Log)
			=> Logger.Instance._Log(Log);
		public static void Log(string format, params object[] args)
			=> Logger.Instance._Log(format, args);

		public static void Register(string Name)
		{
			Instance.RegisteredLoggers.Add(Name, new Queue<string>());
		}
		public static void Unregister(string Name)
			=> Instance.RegisteredLoggers.Remove(Name);

		public static string Fetch(string Name)
		{
			var item = Instance.RegisteredLoggers[Name];
			item.TrimExcess();

			return item.Count > 0
				? item.Dequeue()
				: null;
		}
	}
}
