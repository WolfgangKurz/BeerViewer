using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Diagnostics;

using BeerViewer.Modules;
using System.Net.Sockets;

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

		public int ListeningPort { get; private set; } = 49217;

		public static Proxy Instance { get; } = new Proxy();

		internal ProxyHandlerContainer<ProxyHandler> Handlers { get; }
		internal ProxyHandlerContainer<ModifiableProxyHandler> ModifiableHandlers { get; }

		private Proxy()
		{
			this.Handlers = new ProxyHandlerContainer<ProxyHandler> ();
			this.ModifiableHandlers = new ProxyHandlerContainer< ModifiableProxyHandler >();
			if (IsInDesignMode) return;

			HttpProxy.AfterSessionComplete += (_ =>
			{
				this.Handlers.Values.ToList().ForEach(x =>
				{
					if ((x.Where == null) || (_.Request.PathAndQuery == x.Where))
						x.Handler.Invoke(_);
				});
			});

			bool AlterPort = false;
			do
			{
				try
				{
					HttpProxy.Startup(this.ListeningPort, false, true);
					break;
				}
				catch (SocketException)
				{
					// Port already using, try next port
					this.ListeningPort++;
					AlterPort = true;
				}
			} while (true);

			if (AlterPort)
				Logger.Log($"Default port already in using, use {this.ListeningPort} instead.");

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
		public int Register(Action<Session> e)
			=> this.Register(null, e);

		/// <summary>
		/// Register HTTP event handler with URL
		/// </summary>
		public int Register(string Where, Action<Session> e)
		{
			if (IsInDesignMode) return -1;

			if (!this.PermissionCheck(BeerComponentPermission.CP_NETWORK))
				throw new InvalidOperationException("Not allowed operation");

			try
			{
				var key = this.Handlers.TryAdd(
					new ProxyHandler
					{
						Where = Where,
						Handler = e
					}
				);
				return key;
			}
			catch { }

			return -1;
		}
		#endregion

		#region BeforeResponse Handlers
		public int RegisterModifiable(Func<Session, byte[], byte[]> e)
			=> this.RegisterModifiable(null, e);

		public int RegisterModifiable(string Where, Func<Session, byte[], byte[]> e)
		{
			if (IsInDesignMode) return -1;

			if (!this.PermissionCheck(BeerComponentPermission.CP_NETWORK_MODIFIABLE))
				throw new InvalidOperationException("Not allowed operation");

			try
			{
				var key = this.ModifiableHandlers.TryAdd(
					new ModifiableProxyHandler
					{
						Where = Where,
						Handler_Response = e
					}
				);
				return key;
			}
			catch { }

			return -1;
		}
		#endregion

		#region BeforeRequest Handlers
		public int RegisterModifiable(Func<HttpRequest, bool> e)
			=> this.RegisterModifiable(null, e);

		public int RegisterModifiable(string Where, Func<HttpRequest, bool> e)
		{
			if (IsInDesignMode) return -1;

			if (!this.PermissionCheck(BeerComponentPermission.CP_NETWORK_MODIFIABLE))
				throw new InvalidOperationException("Not allowed operation");

			try
			{
				var key = this.ModifiableHandlers.TryAdd(
					new ModifiableProxyHandler
					{
						Where = Where,
						Handler_Request = e
					}
				);
				return key;
			}
			catch { }

			return -1;
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
			this.Handlers.TryRemove(k, out _);
			return this;
		}

		/// <summary>
		/// Unregister HTTP event handler with key
		/// </summary>
		public Proxy Unregister(int key)
		{
			this.Handlers.TryRemove(key, out _);
			return this;
		}

		/// <summary>
		/// Unregister HTTP event modifiable handler
		/// </summary>
		public Proxy UnregisterModifiable(Func<HttpRequest, bool> e)
		{
			var k = this.ModifiableHandlers.FirstOrDefault(x => x.Value.Handler_Request == e).Key;
			this.Handlers.TryRemove(k, out _);
			return this;
		}

		/// <summary>
		/// Unregister HTTP event modifiable handler
		/// </summary>
		public Proxy UnregisterModifiable(Func<Session, byte[], byte[]> e)
		{
			var k = this.ModifiableHandlers.FirstOrDefault(x => x.Value.Handler_Response == e).Key;
			this.Handlers.TryRemove(k, out _);
			return this;
		}

		/// <summary>
		/// Unregister HTTP event modifiable handler with key
		/// </summary>
		public Proxy UnregisterModifiable(int key)
		{
			this.ModifiableHandlers.TryRemove(key, out _);
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
			}

			HttpProxy.Shutdown();
			this.disposed = true;
		}
		#endregion
	}
}
