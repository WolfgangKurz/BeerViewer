using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

using BeerViewer.Framework;
using BeerViewer.Models;
using CefSharp;

namespace BeerViewer.Modules.Communication
{
	public class WindowBrowserCommicator
	{
		private Form Owner { get; }
		private IntPtr OwnerHandle { get; }
		private FrameworkBrowser Browser { get; }
		private Action OnInitialized;

		private ObservedTreeManager ObserveObjectManager;

		internal WindowBrowserCommicator(Form Owner, FrameworkBrowser Browser, Action OnInitialized)
		{
			if (Owner == null) throw new ArgumentNullException(nameof(Owner));
			if (Browser == null) throw new ArgumentNullException(nameof(Browser));

			this.Owner = Owner;
			this.OwnerHandle = Owner?.Handle ?? IntPtr.Zero;

			this.Browser = Browser;

			this.OnInitialized = OnInitialized;

			this.ObserveObjectManager = new ObservedTreeManager();
		}

		public void Initialized() => this.OnInitialized?.Invoke();
		public void SystemCall(string command)
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

		private async Task<object> CallRawScript(string script)
		{
			var browser = this.Browser.GetBrowser();
			if (browser == null) throw new NullReferenceException(nameof(browser));

			System.Diagnostics.Debug.WriteLine(script);

			var result = await browser.MainFrame?.EvaluateScriptAsync(script);
			if (!result.Success)
				throw new Exception($"CallScript error: {result.Message}");

			return result.Result;
		}
		internal Task<object> CallScript(string name, params string[] args)
		{
			return CallRawScript(
				string.Format(
					"{0}({1})",
					name,
					string.Join(",", args.Select(x => $"\"{x}\""))
				)
			);
		}
		internal Task<object> CallbackScript(string name, params string[] args)
		{
			var _args = new string[] { name }.Concat(args).ToArray();
			return this.CallScript("window.CALLBACK.call", _args);
		}

		/// <summary>
		/// Register object to use in browser with "window.API.observeData(namespace, path, callback)"
		/// or get data with "window.API.getData(namespace, path)".
		/// When the <see cref="INotifyPropertyChanged.PropertyChanged"/> fired, given callback will be called.
		/// Callback will get "newValue, Namespace, Path" as parameters.
		/// </summary>
		/// <param name="ObjectName"></param>
		/// <param name="ObjectToObserve"></param>
		internal void RegisterObserveObject(string Namespace, object ObjectToRegister)
		{
			this.ObserveObjectManager.AddNamespaceObject(Namespace, ObjectToRegister);
		}

		public void ObserveData(string ns, string path, IJavascriptCallback callback)
		{
			try
			{
				this.ObserveObjectManager.RegisterTree(ns, path, x =>
				{
					if (callback != null && callback.CanExecute)
					{
						try
						{
							callback.ExecuteAsync(x, ns, path);
						}
						catch (NotSupportedException)
						{
							callback.ExecuteAsync(new object(), ns, path);
						}
					}
				});
			}
			catch { }
		}
		public object GetData(string ns, string path)
		{
			try
			{
				return this.ObserveObjectManager.GetData(ns, path);
			}
			catch { }

			return null;
		}
	}
}
