using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Modules.Communication
{
	internal class ObservedTreeData : IDisposable
	{
		#region Properties
		public bool IsRoot { get; }

		/// <summary>
		/// Observing target property's or field's name at <see cref="Object"/>
		/// </summary>
		public ObjectPathLevel ObservingInfo { get; }

		/// <summary>
		/// Name of <see cref="ObservingInfo"/>
		/// </summary>
		public string Name => this.ObservingInfo.Name;

		/// <summary>
		/// Index of <see cref="ObservingInfo"/>
		/// </summary>
		public int Index => this.ObservingInfo.Index;

		/// <summary>
		/// IsArray of <see cref="ObservingInfo"/>
		/// </summary>
		public bool IsArray => this.ObservingInfo.IsArray;

		/// <summary>
		/// Path name
		/// </summary>
		public string Path => this.ObservingInfo.ToString();

		/// <summary>
		/// Object to attach (May not be <see cref="INotifyPropertyChanged"/>)
		/// </summary>
		public object Object { get; }

		#region Child property
		private ObservedTreeData _child;
		/// <summary>
		/// Related childs
		/// </summary>
		public ObservedTreeData Child {
			get { return this._child; }
			set
			{
				if(this._child != value)
				{
					this._child?.Dispose();
					this._child = value;
				}
			}
		}
		#endregion

		/// <summary>
		/// When property changed
		/// </summary>
		protected PropertyChangedEventHandler EventHandler { get; private set; }
		#endregion

		public ObservedTreeData(ObjectPathLevel ObservingInfo, object Object, PropertyChangedEventHandler Handler, bool IsRoot = false)
		{
			this.IsRoot = IsRoot;
			this.ObservingInfo = ObservingInfo;
			this.Object = Object;

			if (Object != null && typeof(INotifyPropertyChanged).IsAssignableFrom(Object.GetType()))
			{
				if (Handler == null) throw new ArgumentNullException(nameof(Handler));
				this.EventHandler = Handler;

				var notifier = (INotifyPropertyChanged)Object;
				notifier.PropertyChanged += EventHandler;
			}
		}

		~ObservedTreeData() {
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
			}

			if(this.EventHandler != null)
			{
				((INotifyPropertyChanged)Object).PropertyChanged -= EventHandler;
				this.EventHandler = null;
			}

			this.Child = null;
		}

		public override string ToString()
			=> $"{{ObservingInfo: {ObservingInfo.ToString()}, Object: {Object}}}";
	}
}
