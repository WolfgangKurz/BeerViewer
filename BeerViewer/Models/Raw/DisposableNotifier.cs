using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Models;
using MultipleDisposable = BeerViewer.Models.Raw.Disposable.MultipleDisposable;

namespace BeerViewer.Models.Raw.Disposable
{
	public class MultipleDisposable : IDisposable
	{
		private readonly List<IDisposable> DisposableList;

		public MultipleDisposable()
		{
			this.DisposableList = new List<IDisposable>();
		}

		public void Add(IDisposable disposable)
		{
			this.DisposableList.Add(disposable);
		}
		public void Dispose()
		{
			foreach (var item in this.DisposableList) item.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}

namespace BeerViewer.Models.Raw
{
	public class DisposableNotifier : Notifier, IDisposable
	{
		protected MultipleDisposable CompositeDisposable { get; }

		public DisposableNotifier()
		{
			this.CompositeDisposable = new MultipleDisposable();
		}

		public void Dispose()
		{
			this.Dispose(true);
			this.CompositeDisposable.Dispose();

			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) { }
	}

	public class PropertyChangedEventListener : Dictionary<string, PropertyChangedEventHandler>, IDisposable
	{
		private PropertyChangedEventHandler EventHandler { get; }
		private Notifier notifier { get; }

		public PropertyChangedEventListener(Notifier notifier)
		{
			this.notifier = notifier;

			EventHandler = (s, e) => this[e.PropertyName]?.Invoke(s, e);
			notifier.PropertyChanged += EventHandler;
		}

		public void Dispose()
		{
			notifier.PropertyChanged -= EventHandler;
			this.Clear();
		}
	}

	public class WeakEventListener<THandler, TEventArgs> : IDisposable where TEventArgs : EventArgs
	{
		private Action<THandler> remove { get; }
		private THandler converted { get; }

		public WeakEventListener(Func<EventHandler<TEventArgs>, THandler> conversion, Action<THandler> add, Action<THandler> remove, EventHandler<TEventArgs> handler)
		{
			EventHandler<TEventArgs> inner_handler = (s, e) => handler?.Invoke(s, e);
			converted = conversion.Invoke(inner_handler);

			add?.Invoke(converted);
			this.remove = remove;
		}
		public void Dispose()
		{
			remove?.Invoke(converted);
		}
	}
}
