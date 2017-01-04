using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nekoxy;
using BeerViewer.Models.Raw;

namespace BeerViewer.Core
{
	internal partial class Proxy
	{
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
			if (Helper.IsInDesignMode) return;

			HttpProxy.Startup(49217, false, true);
			HttpProxy.AfterSessionComplete += (_ =>
			{
				var handlers = this.Handlers.ToArray();
				foreach (var x in handlers)
				{
					if ((x.Where == null) || (_.Request.PathAndQuery == x.Where))
						x.Handler.Invoke(_);
				};
			});
		}
		~Proxy()
		{
			HttpProxy.Shutdown();
		}

		#region 이벤트 핸들러 등록
		/// <summary>
		/// 키 없는 HTTP 이벤트 핸들러를 등록
		/// </summary>
		public Proxy Register(Action<Session> e)
		{
			return this.Register(null, e);
		}
		/// <summary>
		/// 키와 도착지 있는 HTTP 이벤트 핸들러를 등록
		/// </summary>
		public Proxy Register(string Where, Action<Session> e)
		{
			if (Helper.IsInDesignMode) return this;

			try
			{
				this.Handlers.Add(new Core.Proxy.ProxyHandler
				{
					Where = Where,
					Handler = e
				});
			}
			catch { }

			return this;
		}
		#endregion

		#region 이벤트 핸들러 제거
		/// <summary>
		/// HTTP 이벤트 핸들러를 제거
		/// </summary>
		public Proxy Unregister(Action<Session> e)
		{
			this.Handlers.Remove(this.Handlers.First(x => x.Handler == e));
			return this;
		}
		#endregion
	}

	public static class ProxySession
	{
		public static SvData TryParse(this Session x)
		{
			SvData result;
			return SvData.TryParse(x, out result) && result.IsSuccess
				? result : null;
		}
		public static SvData<T> TryParse<T>(this Session x)
		{
			SvData<T> result;
			return SvData.TryParse(x, out result) && result.IsSuccess
				? result : null;
		}
	}
}
