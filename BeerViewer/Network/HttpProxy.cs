using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrotiNet;
using Nekoxy;

namespace BeerViewer.Network
{
	/// <summary>
	/// HTTP proxy server
	/// HTTP protocol only, HTTPS is not supported
	/// </summary>
	public static class HttpProxy
	{
		private static SafeTcpServer server;

		/// <summary>
		/// Fire when before fire before tunnel request body
		/// </summary>
		public static event Func<HttpRequest, bool> BeforeRequest;

		/// <summary>
		/// Fire when before fire before tunnel response body
		/// </summary>
		public static event Func<Session, byte[], byte[]> BeforeResponse;

		/// <summary>
		/// Fire when after sent HTTP response to client.
		/// </summary>
		public static event Action<Session> AfterSessionComplete;

		/// <summary>
		/// Fire when succeeded to read request header data.
		/// Before receive body.
		/// </summary>
		public static event Action<HttpRequest> AfterReadRequestHeaders;

		/// <summary>
		/// レスポンスヘッダを読み込み完了した際に発生。
		/// ボディは受信前。
		/// </summary>
		public static event Action<HttpResponse> AfterReadResponseHeaders;

		/// <summary>
		/// 上流プロキシ設定。
		/// </summary>
		public static ProxyConfig UpstreamProxyConfig
		{
			get { return ModifiableProxy.UpstreamProxyConfig; }
			set { ModifiableProxy.UpstreamProxyConfig = value; }
		}

		/// <summary>
		/// プロキシサーバーが Listening 中かどうかを取得。
		/// </summary>
		public static bool IsInListening => server != null && server.IsListening;

		/// <summary>
		/// 指定ポートで Listening を開始する。
		/// Shutdown() を呼び出さずに2回目の Startup() を呼び出した場合、InvalidOperationException が発生する。
		/// </summary>
		/// <param name="listeningPort">Listeningするポート。</param>
		/// <param name="useIpV6">falseの場合、127.0.0.1で待ち受ける。trueの場合、::1で待ち受ける。既定false。</param>
		/// <param name="isSetProxyInProcess">trueの場合、プロセス内IEプロキシの設定を実施し、HTTP通信をNekoxyに向ける。既定true。</param>
		public static void Startup(int listeningPort, bool useIpV6 = false, bool isSetProxyInProcess = true)
		{
			if (server != null) throw new InvalidOperationException("Calling Startup() twice without calling Shutdown() is not permitted.");

			ModifiableProxy.AfterSessionComplete += InvokeAfterSessionComplete;
			ModifiableProxy.AfterReadRequestHeaders += InvokeAfterReadRequestHeaders;
			ModifiableProxy.AfterReadResponseHeaders += InvokeAfterReadResponseHeaders;
			ModifiableProxy.BeforeRequest += InvokeBeforeRequest;
			ModifiableProxy.BeforeResponse += InvokeBeforeResponse;
			ListeningPort = listeningPort;
			try
			{
				if (isSetProxyInProcess)
					WinInetUtil.SetProxyInProcessForNekoxy(listeningPort);

				server = new SafeTcpServer(listeningPort, useIpV6);
				server.Start(ModifiableProxy.CreateProxy);
				server.InitListenFinished.WaitOne();
				if (server.InitListenException != null) throw server.InitListenException;
			}
			catch (Exception)
			{
				Shutdown();
				throw;
			}
		}

		/// <summary>
		/// Listening しているスレッドを終了し、ソケットを閉じる。
		/// </summary>
		public static void Shutdown()
		{
			ModifiableProxy.AfterSessionComplete -= InvokeAfterSessionComplete;
			ModifiableProxy.AfterReadRequestHeaders -= InvokeAfterReadRequestHeaders;
			ModifiableProxy.AfterReadResponseHeaders -= InvokeAfterReadResponseHeaders;
			ModifiableProxy.BeforeRequest -= InvokeBeforeRequest;
			ModifiableProxy.BeforeResponse -= InvokeBeforeResponse;
			server?.Shutdown();
			server = null;
		}

		internal static int ListeningPort { get; set; }

		private static void InvokeAfterSessionComplete(Session session)
			=> AfterSessionComplete?.Invoke(session);

		private static void InvokeAfterReadRequestHeaders(HttpRequest request)
			=> AfterReadRequestHeaders?.Invoke(request);

		private static void InvokeAfterReadResponseHeaders(HttpResponse response)
			=> AfterReadResponseHeaders?.Invoke(response);

		private static bool InvokeBeforeRequest(HttpRequest request)
			=> BeforeRequest?.Invoke(request) ?? true;

		private static byte[] InvokeBeforeResponse(Session session, byte[] body)
			=> BeforeResponse?.Invoke(session, body);
	}
}