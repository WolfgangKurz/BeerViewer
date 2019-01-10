﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

using BeerViewer.Framework;
using BeerViewer.Models;
using BeerViewer.Network;
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
			this.Owner = Owner ?? throw new ArgumentNullException(nameof(Owner));
			this.OwnerHandle = Owner?.Handle ?? IntPtr.Zero;

			this.Browser = Browser ?? throw new ArgumentNullException(nameof(Browser));

			this.OnInitialized = OnInitialized;

			this.ObserveObjectManager = new ObservedTreeManager();
		}

		/// <summary>
		/// Notify system initialized.
		/// </summary>
		public void Initialized()
		{
			if (this.OnInitialized == null) throw new Exception("Initialized twice");

			this.OnInitialized.Invoke();
			this.OnInitialized = null; // Cannot initialized twice
		}

		/// <summary>
		/// Call system command.
		/// </summary>
		/// <param name="command">Command to call</param>
		public bool SystemCall(string command)
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
			return true;
		}

		/// <summary>
		/// Evaluation raw javascript string.
		/// </summary>
		/// <param name="script">Raw javascript string</param>
		/// <returns>Script result</returns>
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

		/// <summary>
		/// Call javascript function with arguments.
		/// </summary>
		/// <param name="name">Javascript function name</param>
		/// <param name="args">Arguments</param>
		/// <returns>Script result</returns>
		internal Task<object> CallScript(string name, params object[] args)
		{
			return CallRawScript(
				string.Format(
					"{0}({1})",
					name,
					string.Join(",", args.Select(x =>
					{
						var type = x.GetType();
						if (type == typeof(string)) return $"\"{(x as string)}\"";
						else if (type == typeof(bool)) return (bool)x ? "true" : "false";
						else return x.ToString();
					}))
				)
			);
		}

		/// <summary>
		/// Call window.CALLBACK callbacks.
		/// Modules need to register callback with window.CALLBACK.register(callbackname, callbackfunc).
		/// </summary>
		/// <param name="name">Callback name</param>
		/// <param name="args">Arguments</param>
		/// <returns>When the result is false, registered callback function not found</returns>
		internal Task<object> CallbackScript(string name, params object[] args)
		{
			var _args = new object[] { name }.Concat(args).ToArray();
			return this.CallScript("window.CALLBACK.call", _args);
		}

		/// <summary>
		/// Register object to use in browser with "window.API.observeData(namespace, path, callback)"
		/// or get data with "window.API.getData(namespace, path)".
		/// </summary>
		/// <param name="Namespace">Namespace name</param>
		/// <param name="ObjectToObserve">Namespace object</param>
		internal void RegisterObserveObject(string Namespace, object ObjectToRegister)
		{
			this.ObserveObjectManager.AddNamespaceObject(Namespace, ObjectToRegister);
		}

		/// <summary>
		/// Observe data tree on specific namespace.
		/// When the <see cref="INotifyPropertyChanged.PropertyChanged"/> has fired, given callback will be called.
		/// Callback will get "newValue, Namespace, Path" as arguments.
		/// </summary>
		/// <param name="ns">Namespace on target value</param>
		/// <param name="path">Path of target value</param>
		/// <param name="callback">Callback when <see cref="INotifyPropertyChanged.PropertyChanged"/> fired</param>
		/// <param name="ObserveOnly">If true, callback parameter is empty object always.</param>
		public void ObserveData(string ns, string path, IJavascriptCallback callback, bool ObserveOnly = false)
		{
			try
			{
				this.ObserveObjectManager.RegisterTree(ns, path, x =>
				{
					if (callback != null && callback.CanExecute)
					{
						try
						{
							if (Serialization.Serializable(x) && !ObserveOnly)
								callback.ExecuteAsync(x, ns, path);
							else
								callback.ExecuteAsync(x == null ? null : new object(), ns, path);
						}
						catch (NotSupportedException e)
						{
							Logger.Log(e.ToString());
							callback.ExecuteAsync(x == null ? null : new object(), ns, path);
						}
					}
				});
			}
			catch { }
		}

		/// <summary>
		/// Returns data on specific namespace.
		/// </summary>
		/// <param name="ns">Namespace of target value</param>
		/// <param name="path">Path of target value</param>
		/// <returns>Measured data, can be json or null</returns>
		public object GetData(string ns, string path)
		{
			try
			{
				var result = this.ObserveObjectManager.GetData(ns, path);

				if (Serialization.Serializable(result))
					return result;
				else
					return result == null ? null : new object();
			}
			catch { }

			return null;
		}

		/// <summary>
		/// Returns localized text.
		/// If not found, returns inputted <paramref name="Text"/> argument.
		/// </summary>
		/// <param name="Text">Text to localize</param>
		/// <returns>Localized, or inputted text</returns>
		public string i18n(string Text)
		{
			if (string.IsNullOrEmpty(Text)) return "";
			return Modules.i18n.Current[Text] ?? Text;
		}

		/// <summary>
		/// Returns localized text set.
		/// </summary>
		/// <returns>Registered i18n text dictionary</returns>
		public Dictionary<string, string> i18nSet()
		{
			return Modules.i18n.Current.Table as Dictionary<string, string>;
		}

		/// <summary>
		/// Get available module list.
		/// </summary>
		/// <returns>Module list</returns>
		public ModuleInfo[] GetModuleList()
		{
			var output = new List<ModuleInfo>();
			var baseDir = Path.Combine(
				Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
				"WindowFrame",
				"modules"
			);
			var dirs = Directory.GetDirectories(baseDir);
			foreach(var dir in dirs)
			{
				var moduleName = Path.GetFileName(dir);
				var scriptFile = Path.Combine(dir, moduleName + ".js");
				var templateFile = Path.Combine(dir, moduleName + ".html");
				var styleFile = Path.Combine(dir, moduleName + ".css");

				if (!File.Exists(scriptFile))
				{
					Logger.Log("Module directory '{0}' found but '{0}.js' not found", moduleName, scriptFile);
					continue;
				}

				output.Add(new ModuleInfo
				{
					Name = moduleName,
					Template = File.Exists(templateFile) ? File.ReadAllText(templateFile) : "",
					Scripted = File.Exists(scriptFile),
					Styled = File.Exists(styleFile),
				});
			}
			return output.ToArray();
		}

		/// <summary>
		/// Open DevTools for BeerViewer
		/// </summary>
		public void DevTools()
		{
			Browser.ShowDevTools();
		}

		/// <summary>
		/// Log to logger
		/// </summary>
		/// <param name="Text">Text to log</param>
		public void Log(string Text)
		{
			Logger.Log(Text);
		}

		/// <summary>
		/// Convert HTTP Request <see cref="NameValueCollection"/> object to <see cref="IDictionary{string, string}"/>
		/// </summary>
		/// <param name="c">Request data to convert</param>
		/// <returns>Converted Dictionary data</returns>
		private IDictionary<string, object> ConvertRequest(NameValueCollection c)
		{
			var output = new Dictionary<string, object>();

			var n = c.Count;
			for(var i=0; i<n; i++)
			{
				var key = c.Keys[i];
				var value = c.GetValues(i);

				if (value == null)
					output.Add(key, null);
				else if (value.Length > 1)
				{
					var list = new List<string>();
					foreach (var j in value)
						list.Add(j);
					output.Add(key, list);
				}
				else if (value.Length == 1)
					output.Add(key, value[0]);
			}
			return output;
		}

		/// <summary>
		/// Observe HTTP response.
		/// </summary>
		/// <param name="url">Url to watch</param>
		/// <param name="callback">Callback when get HTTP response with <paramref name="url"/>.</param>
		public void SubscribeHTTP(string url, IJavascriptCallback callback)
		{
			if (callback == null || !callback.CanExecute) return;

			Proxy.Instance.Register(url, e =>
			{
				var x = e.TryParse();
				if (x == null) return;

				if (callback != null && callback.CanExecute)
					callback.ExecuteAsync(e.Response.BodyAsString, ConvertRequest(x.Request));
			});
		}
	}
}
