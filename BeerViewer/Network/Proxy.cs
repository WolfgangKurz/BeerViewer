using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Nekoxy;

namespace BeerViewer.Network
{
	internal partial class Proxy
	{
		private static bool IsInDesignMode => (LicenseManager.UsageMode == LicenseUsageMode.Designtime);

		private class ProxyHandler
		{
			public string Where { get; set; }
			public Action<Session> Handler { get; set; }
		}

		public static Proxy Instance { get; } = new Proxy();

		private List<ProxyHandler> Handlers;
		private object HandlerLock { get; } = new object();

		private Proxy()
		{
			this.Handlers = new List<ProxyHandler>();
			if (IsInDesignMode) return;

			HttpProxy.Startup(49217, false, true);
			HttpProxy.AfterSessionComplete += (_ =>
				this.Handlers.ForEach(x =>
				{
					if ((x.Where == null) || (_.Request.PathAndQuery == x.Where))
						x.Handler.Invoke(_);
				})
			);
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
				this.Handlers.Add(new Proxy.ProxyHandler
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
			this.Handlers.Remove(this.Handlers.First(x => x.Handler == e));
			return this;
		}
		#endregion
	}
}
