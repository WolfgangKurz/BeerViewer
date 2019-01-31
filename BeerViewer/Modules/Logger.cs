using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BeerViewer.Modules
{
	public class LogData
	{
		public string Format { get; }
		public object[] Arguments { get; }

		public LogData(string format, object[] arguments)
		{
			this.Format = format;
			this.Arguments = arguments;
		}
	}
	public class Logger
	{
		public delegate void LogEventHandler(string format, params object[] args);
		public static event LogEventHandler Logged;

		private static Logger Instance { get; } = new Logger();

		private Dictionary<string, Queue<LogData>> RegisteredLoggers { get; }

		private Logger()
		{
			this.RegisteredLoggers = new Dictionary<string, Queue<LogData>>();
		}

		private void _Log(string format, params object[] args)
		{
			Logger.Logged?.Invoke(format, args);

			foreach (var kv in this.RegisteredLoggers)
				kv.Value.Enqueue(new LogData(format, args));

			Debug.WriteLine(string.Format(format, args));
		}


		public static void Log(string Log)
			=> Logger.Instance._Log(Log);
		public static void Log(string format, params object[] args)
			=> Logger.Instance._Log(format, args);

		public static void Register(string Name)
		{
			Instance.RegisteredLoggers.Add(Name, new Queue<LogData>());
		}
		public static void Unregister(string Name)
			=> Instance.RegisteredLoggers.Remove(Name);

		public static LogData Fetch(string Name)
		{
			var item = Instance.RegisteredLoggers[Name];
			item.TrimExcess();

			return item.Count > 0
				? item.Dequeue()
				: null;
		}
	}
}
