using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Diagnostics;

using BeerViewer.Modules;

namespace BeerViewer.Network
{
	public partial class Proxy : IDisposable
	{
		internal static bool IsInDesignMode => (LicenseManager.UsageMode == LicenseUsageMode.Designtime);

		internal class ProxyHandler
		{
			public string Where { get; set; }
			public Action<Session> Handler { get; set; }
		}
		internal class ModifiableProxyHandler
		{
			public string Where { get; set; } = null;
			public Func<HttpRequest, bool> Handler_Request { get; set; } = null;
			public Func<Session, byte[], byte[]> Handler_Response { get; set; } = null;
		}

		public static Proxy Instance { get; } = new Proxy();

		internal ConcurrentDictionary<int, ProxyHandler> Handlers { get; }
		internal ConcurrentDictionary<int, ModifiableProxyHandler> ModifiableHandlers { get; }

		private Proxy()
		{
			this.Handlers = new ConcurrentDictionary<int, ProxyHandler>();
			this.ModifiableHandlers = new ConcurrentDictionary<int, ModifiableProxyHandler>();
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

			HttpProxy.BeforeRequest += _ =>
				this.ModifiableHandlers
					.Select(x => x.Value)
					.Where(x => x.Handler_Request != null)
					.Where(x => (x.Where == null) || (_.PathAndQuery == x.Where))
					.All(x => x.Handler_Request.Invoke(_));

			HttpProxy.BeforeResponse += (_, b) =>
			{
				var list = this.ModifiableHandlers
					.Select(x => x.Value)
					.Where(x => x.Handler_Response != null)
					.Where(x => (x.Where == null) || (_.Request.PathAndQuery == x.Where));

				foreach (var h in list)
					b = h.Handler_Response.Invoke(_, b);

				return b;
			};
		}
		~Proxy()
		{
			this.Dispose(false);
		}

		private bool PermissionCheck(BeerComponentPermission pm_require)
		{
			try
			{
				var entry = Assembly.GetEntryAssembly();

				var frames = new StackTrace().GetFrames();
				var prev = frames[1].GetMethod();

				var frame = frames.Skip(2)
					.Select(x => x.GetMethod())
					.SkipWhile(x => (x.Name == prev.Name) && (x.Module.Assembly == prev.Module.Assembly))
					.FirstOrDefault();

				if (frame.Module.Assembly == entry)
				{
					// Caller is BeerViewer. Skip check permission
				}
				else
				{
					var perm = (BeerComponentPermission)
						frame.DeclaringType.GetCustomAttributes(false)
							.Where(x => x.GetType() == typeof(ExportMetadataAttribute))
							.Select(x => x as ExportMetadataAttribute)
							.FirstOrDefault(x => x.Name == "Permission")
							.Value;

					if ((perm & pm_require) != pm_require)
						return false;
				}
			}
			catch
			{
				return false; // No permissions or Cannot measure caller
			}
			return true;
		}

		#region Event handler registers

		#region Default Handlers
		/// <summary>
		/// Register HTTP event handler without URL
		/// </summary>
		public Proxy Register(Action<Session> e)
			=> this.Register(null, e);

		/// <summary>
		/// Register HTTP event handler with URL
		/// </summary>
		public Proxy Register(string Where, Action<Session> e)
		{
			if (IsInDesignMode) return this;

			if (!this.PermissionCheck(BeerComponentPermission.CP_NETWORK))
				throw new InvalidOperationException("Not allowed operation");

			try
			{
				this.Handlers.TryAdd(
					this.Handlers.Count,
					new ProxyHandler
					{
						Where = Where,
						Handler = e
					}
				);
			}
			catch { }

			return this;
		}
		#endregion

		#region BeforeResponse Handlers
		public Proxy RegisterModifiable(Func<Session, byte[], byte[]> e)
			=> this.RegisterModifiable(null, e);

		public Proxy RegisterModifiable(string Where, Func<Session, byte[], byte[]> e)
		{
			if (IsInDesignMode) return this;

			if (!this.PermissionCheck(BeerComponentPermission.CP_NETWORK_MODIFIABLE))
				throw new InvalidOperationException("Not allowed operation");

			try
			{
				this.ModifiableHandlers.TryAdd(
					this.ModifiableHandlers.Count,
					new ModifiableProxyHandler
					{
						Where = Where,
						Handler_Response = e
					}
				);
			}
			catch { }

			return this;
		}
		#endregion

		#region BeforeRequest Handlers
		public Proxy RegisterModifiable(Func<HttpRequest, bool> e)
			=> this.RegisterModifiable(null, e);

		public Proxy RegisterModifiable(string Where, Func<HttpRequest, bool> e)
		{
			if (IsInDesignMode) return this;

			if (!this.PermissionCheck(BeerComponentPermission.CP_NETWORK_MODIFIABLE))
				throw new InvalidOperationException("Not allowed operation");

			try
			{
				this.ModifiableHandlers.TryAdd(
					this.ModifiableHandlers.Count,
					new ModifiableProxyHandler
					{
						Where = Where,
						Handler_Request = e
					}
				);
			}
			catch { }

			return this;
		}
		#endregion

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

		#region Dispose
		private bool disposed;

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.disposed) return;
			if (disposing)
			{
				// IDisposable 인터페이스를 구현하는 멤버들을 여기서 정리합니다.
			}
			// .NET Framework에 의하여 관리되지 않는 외부 리소스들을 여기서 정리합니다.
			HttpProxy.Shutdown();
			this.disposed = true;
		}
		#endregion
	}
}
