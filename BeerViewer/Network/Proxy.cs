using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Nekoxy;

namespace BeerViewer.Network
{
	public partial class Proxy
	{
		internal static bool IsInDesignMode => (LicenseManager.UsageMode == LicenseUsageMode.Designtime);

		internal class ProxyHandler
		{
			public string Where { get; set; }
			public Action<Session> Handler { get; set; }
		}

		public static Proxy Instance { get; } = new Proxy();

		internal ConcurrentDictionary<int, ProxyHandler> Handlers { get; private set; }

		private Proxy()
		{
			this.Handlers = new ConcurrentDictionary<int, ProxyHandler>();
			if (IsInDesignMode) return;

			HttpProxy.Startup(49217, false, true);
			HttpProxy.AfterSessionComplete += (_ =>
			{
				this.Handlers.Values.ToList().ForEach(x =>
				{
					if ((x.Where == null) || (_.Request.PathAndQuery == x.Where))
						x.Handler.Invoke(_);
				});
			});
		}
		~Proxy()
		{
			HttpProxy.Shutdown();
		}

		#region Event handler registers

		/// <summary>
		/// Register HTTP event handler without URL
		/// </summary>
		public Proxy Register(Action<Session> e)
		{
			return this.Register(null, e);
		}
		/// <summary>
		/// Register HTTP event handler with URL
		/// </summary>
		public Proxy Register(string Where, Action<Session> e)
		{
			if (IsInDesignMode) return this;

			try
			{
				this.Handlers.TryAdd(this.Handlers.Count, new ProxyHandler
				{
					Where = Where,
					Handler = e
				});
			}
			catch { }

			return this;
		}
		#endregion

		#region Unregister HTTP event handler
		/// <summary>
		/// Unregister HTTP event handler
		/// </summary>
		public Proxy Unregister(Action<Session> e)
		{
			var k = this.Handlers.FirstOrDefault(x => x.Value.Handler == e).Key;
			ProxyHandler h;

			this.Handlers.TryRemove(k, out h);
			return this;
		}
		#endregion
	}
}
