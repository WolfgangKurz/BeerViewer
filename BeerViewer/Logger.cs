using System.Diagnostics;

namespace BeerViewer
{
	internal class Logger
	{
		public delegate void LogEventHandler(string Log);
		public event LogEventHandler Logged;

		public static Logger Instance { get; } = new Logger();

		public void Log(string Log)
		{
			this.Logged?.Invoke(Log);
			Debug.WriteLine(Log);
		}
		public void Log(string format, params object[] args)
			=> this.Log(string.Format(format, args));
	}
}
