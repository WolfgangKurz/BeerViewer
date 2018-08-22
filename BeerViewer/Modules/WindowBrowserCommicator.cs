using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BeerViewer.Framework;

namespace BeerViewer.Modules
{
	public class WindowBrowserCommicator
	{
		private Form Owner { get; }
		private IntPtr OwnerHandle { get; }

		private FrameworkBrowser Browser { get; }

		private Action OnInitialized;

		internal WindowBrowserCommicator(Form Owner, FrameworkBrowser Browser, Action OnInitialized)
		{
			if (Owner == null) throw new ArgumentNullException(nameof(Owner));
			if (Browser == null) throw new ArgumentNullException(nameof(Browser));

			this.Owner = Owner;
			this.OwnerHandle = Owner?.Handle ?? IntPtr.Zero;

			this.Browser = Browser;

			this.OnInitialized = OnInitialized;
		}

		public void initialized() => this.OnInitialized?.Invoke();

		public void systemCall(string command)
		{
			switch (command.ToLower())
			{
				case "minimize":
					this.Owner.Invoke(() => this.Owner.WindowState = FormWindowState.Minimized);
					break;
				case "maximize":
					this.Owner.Invoke(() =>
					{
						if (this.Owner.WindowState == FormWindowState.Maximized)
							this.Owner.WindowState = FormWindowState.Normal;
						else
							this.Owner.WindowState = FormWindowState.Maximized;
					});
					break;
				case "close":
					this.Owner.Invoke(() => this.Owner.Close());
					break;
			}
		}

		internal async Task<object> CallScript(string name, params string[] args)
		{
			var browser = this.Browser.GetBrowser();
			if (browser == null) throw new NullReferenceException(nameof(browser));

			var result = await browser.MainFrame?.EvaluateScriptAsync(
				string.Format(
					"{0}({1})",
					name,
					string.Join(",", args.Select(x => $"\"{x}\""))
				)
			);
			if (!result.Success)
				throw new Exception($"CallScript error: {result.Message}");

			return result.Result;
		}
		internal Task<object> CallbackScript(string name, params string[] args)
		{
			var _args = new string[] { name }.Concat(args).ToArray();
			return this.CallScript("window.CALLBACK.call", _args);
		}
	}
}
