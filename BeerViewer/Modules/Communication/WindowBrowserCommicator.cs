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
			if (string.IsNullOrWhiteSpace(Namespace)) throw new ArgumentException(nameof(Namespace));
			if (ObjectToRegister == null) throw new ArgumentNullException(nameof(ObjectToRegister));

			this.RegisteredObserveObjects.Add(Namespace, ObjectToRegister);
		}

		public object ObserveData(string ns, string path, IJavascriptCallback callback)
		{
			object result = null;
			try
			{
				result = GetRegisteredData(ns, path, out object parent, out string propName);

				if (parent != null && typeof(INotifyPropertyChanged).IsAssignableFrom(parent.GetType()))
				{
					var p = (INotifyPropertyChanged)parent;
					p.PropertyChanged += async (s, e) =>
					{
						if (e.PropertyName == propName && callback.CanExecute)
							await callback.ExecuteAsync(
								GetRegisteredData(ns, path, out _, out _),
								ns,
								path
							);
					};
				}
			}
			catch { }

			Task.Run(() => callback.ExecuteAsync(result));
			return result;
		}
		public object GetData(string ns, string path)
		{
			try
			{
				return GetRegisteredData(ns, path, /* ignore out */ out _, out _);
			}
			catch { }

			return null;
		}

		internal object GetRegisteredData(string ns, string path, out object parent, out string propertyName)
		{
			if (!this.RegisteredObserveObjects.ContainsKey(ns))
				throw new Exception($"Namespace \"{ns}\" not registered");

			var obj = this.RegisteredObserveObjects[ns];
			var levels = this.ParsePathLevels(path); // A.B[2].C -> [A:literal, B:array[2], C:literal]

			propertyName = levels.Last().Name;

			parent = null;
			for (var i = 0; i < levels.Length; i++)
			{
				if (obj == null)
				{
					System.Diagnostics.Debug.WriteLine($"Path \"{ns}.{path}\" maybe fine but found null tree");

					if (i == levels.Length - 1)
						parent = null;

					break;
				}

				var level = levels[i];

				var type = obj.GetType();
				var member = type.GetMember(level.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.GetProperty)
					.FirstOrDefault();
				if (member == null)
					throw new Exception($"Path \"{path}\" not exists");

				parent = obj;

				if (member.MemberType == MemberTypes.Field)
					obj = ((FieldInfo)member).GetValue(obj);
				else if (member.MemberType == MemberTypes.Property)
					obj = ((PropertyInfo)member).GetValue(obj);
				else
					throw new NotImplementedException("Only Field or Property can be measured");

				if (level.IsArray)
				{
					type = obj.GetType();
					var indexer = type.GetMethod("get_Item", BindingFlags.Public | BindingFlags.Instance);
					if (indexer == null)
						throw new Exception("Tried to access as Array to not Array object");

					obj = indexer.Invoke(obj, new object[] { level.Index });
				}
			}

			/// TODO: Convert class to json object
			return obj;
		}

		private struct PathLevel
		{
			public string Name;
			public bool IsArray;
			public int Index;

			public override string ToString()
				=> $"{{Name: {Name}, IsArray: {IsArray}, Index: {Index}}}";
		}
		private PathLevel[] ParsePathLevels(string path)
		{
			var list = new List<PathLevel>();
			var nameBuffer = new StringBuilder();
			var indexBuffer = new StringBuilder();

			var arraying = false;
			var current = new PathLevel();
			for (var i = 0; i < path.Length; i++)
			{
				var c = path[i];

				if (arraying)
				{
					if (c == ']')
					{
						if (!int.TryParse(indexBuffer.ToString(), out current.Index))
							throw new Exception("Path parse error: Cannot parse index");

						indexBuffer.Clear();
						arraying = false;
					}
					else if (c < '0' || c > '9')
						throw new ArgumentException("Path parse error: Invalid index value");
					indexBuffer.Append(c);
				}
				else if (c == '.')
				{
					current.Name = nameBuffer.ToString();
					nameBuffer.Clear();

					list.Add(current);
					current = new PathLevel();
				}
				else if (c == '[')
				{
					current.IsArray = true;
					arraying = true;
				}
				else
					nameBuffer.Append(c);
			}

			current.Name = nameBuffer.ToString();
			list.Add(current);

			return list.ToArray();
		}
	}
}
