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

namespace BeerViewer.Modules.Communication
{
	public class WindowBrowserCommicator
	{
		private Form Owner { get; }
		private IntPtr OwnerHandle { get; }
		private FrameworkBrowser Browser { get; }
		private Action OnInitialized;

		private Dictionary<string, object> RegisteredObserveObjects;

		internal WindowBrowserCommicator(Form Owner, FrameworkBrowser Browser, Action OnInitialized)
		{
			if (Owner == null) throw new ArgumentNullException(nameof(Owner));
			if (Browser == null) throw new ArgumentNullException(nameof(Browser));

			this.Owner = Owner;
			this.OwnerHandle = Owner?.Handle ?? IntPtr.Zero;

			this.Browser = Browser;

			this.OnInitialized = OnInitialized;

			this.RegisteredObserveObjects = new Dictionary<string, object>();
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
		/// </summary>
		/// <param name="ObjectName"></param>
		/// <param name="ObjectToObserve"></param>
		internal void RegisterObserveObject(string Namespace, object ObjectToRegister)
		{
			if (string.IsNullOrWhiteSpace(Namespace)) throw new ArgumentException(nameof(Namespace));
			if (ObjectToRegister == null) throw new ArgumentNullException(nameof(ObjectToRegister));

			this.RegisteredObserveObjects.Add(Namespace, ObjectToRegister);
		}

		public object observeData(string ns, string path, object callback)
		{
			object result = null;
			try
			{
				result = GetRegisteredData(ns, path);

				/// TODO: attach <see cref="INotifyPropertyChanged.PropertyChanged"/> event
			}
			catch { }

			return result;
		}
		public object getData(string ns, string path)
		{
			try
			{
				return GetRegisteredData(ns, path);
			}
			catch { }

			return null;
		}

		internal object GetRegisteredData(string ns, string path)
		{
			if (!this.RegisteredObserveObjects.ContainsKey(ns))
				throw new Exception($"Namespace \"{ns}\" not registered");

			var obj = this.RegisteredObserveObjects[ns];
			var levels = this.ParsePathLevels(path); // A.B[2].C -> [A:literal, B:array[2], C:literal]

			for (var i = 0; i < levels.Length; i++)
			{
				var level = levels[i];

				var type = obj.GetType();
				var member = type.GetMember(level.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.GetProperty)
					.FirstOrDefault();
				if (member == null)
					throw new Exception($"Path \"{path}\" not exists");

				if (member.MemberType == MemberTypes.Field)
					obj = ((FieldInfo)member).GetValue(obj);
				else if (member.MemberType == MemberTypes.Property)
					obj = ((PropertyInfo)member).GetValue(obj);
				else
					throw new NotImplementedException("Only Field or Property can be measured");

				if (level.IsArray)
				{
					type = obj.GetType();
					if (!type.IsArray)
						throw new Exception("Tried to access as Array to not Array object");

					obj = ((Array)obj).GetValue(level.Index);
				}
			}

			return null;
		}

		private struct PathLevel
		{
			public string Name;
			public bool IsArray;
			public int Index;
		}
		private PathLevel[] ParsePathLevels(string path)
		{
			throw new NotImplementedException();
		}
	}
}
