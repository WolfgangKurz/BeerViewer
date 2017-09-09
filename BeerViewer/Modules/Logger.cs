using System.Diagnostics;

namespace BeerViewer.Modules
{
	public class Logger
	{
		public delegate void LogEventHandler(string Log);
		public static event LogEventHandler Logged;

		private static Logger Instance { get; } = new Logger();

		private Logger() { }

		private void _Log(string Log)
		{
			Logger.Logged?.Invoke(Log);
			Debug.WriteLine(Log);
		}
		private void _Log(string format, params object[] args)
			=> this._Log(string.Format(format, args));


		public static void Log(string Log)
			=> Logger.Instance._Log(Log);
		public static void Log(string format, params object[] args)
			=> Logger.Instance._Log(format, args);
	}
}
