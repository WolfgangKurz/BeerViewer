using System;
using Nekoxy;

using BeerViewer.Models.Raw;

namespace BeerViewer.Network
{
	public static class ProxyExtension
	{
		public static SvData TryParse(this Session x, bool IsSuccess = true)
		{
			SvData result;
			return SvData.TryParse(x, out result)
				? ((!IsSuccess || result.IsSuccess) ? result : null)
				: null;
		}
		public static SvData<T> TryParse<T>(this Session x, bool IsSuccess = true)
		{
			SvData<T> result;
			return SvData.TryParse(x, out result)
				? ((!IsSuccess || result.IsSuccess) ? result : null)
				: null;
		}

		public static Proxy Register<T>(this Proxy proxy, string Where, Action<SvData<T>> e)
		{
			if (Proxy.IsInDesignMode) return proxy;

			try
			{
				proxy.Handlers.TryAdd(proxy.Handlers.Count, new Proxy.ProxyHandler
				{
					Where = Where,
					Handler = (s =>
					{
						var x = s.TryParse<T>();
						if (x == null) return;

						e?.Invoke(x);
					})
				});
			}
			catch { }

			return proxy;
		}
	}
}
