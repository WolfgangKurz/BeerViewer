using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrotiNet;

namespace BeerViewer.Network
{
	public class Session
	{
		/// <summary>
		/// HTTP Request data
		/// </summary>
		public HttpRequest Request { get; internal set; }

		/// <summary>
		/// HTTP Response data
		/// </summary>
		public HttpResponse Response { get; internal set; }

		public override string ToString()
			=> $"{this.Request}{Environment.NewLine}" +
			   $"{this.Response}{Environment.NewLine}";
	}

	public class HttpRequest
	{
		public HttpRequest(HttpRequestLine requestLine, HttpHeaders headers, byte[] body)
		{
			this.RequestLine = requestLine;
			this.Headers = headers;
			this.Body = body;
		}

		public HttpRequestLine RequestLine { get; }
		public HttpHeaders Headers { get; }

		public string Host => this.Headers?.Host;
		public byte[] Body { get; }

		public string PathAndQuery
			=> this.RequestLine.URI.Contains("://") && Uri.IsWellFormedUriString(this.RequestLine.URI, UriKind.Absolute) ? new Uri(this.RequestLine.URI).PathAndQuery
			: this.RequestLine.URI.Contains("/") ? this.RequestLine.URI
			: string.Empty;

		public Encoding Charset => this.Headers.GetEncoding();
		public string BodyAsString => this.Body?.ToString(this.Charset);

		public override string ToString()
			=> $"{this.RequestLine}{Environment.NewLine}" +
			   $"{this.Headers.HeadersInOrder}{Environment.NewLine}" +
			   $"{this.BodyAsString}{Environment.NewLine}";
	}
	public class HttpResponse
	{
		public HttpResponse(HttpStatusLine statusLine, HttpHeaders headers, byte[] body)
		{
			this.StatusLine = statusLine;
			this.Headers = headers;
			this.Body = body;
		}

		public HttpStatusLine StatusLine { get; }
		public HttpHeaders Headers { get; }
		public byte[] Body { get; }

		public string ContentType
			=> this.Headers.Headers.ContainsKey("content-type")
			? this.Headers.Headers["content-type"]
			: string.Empty;

		public string MimeType => this.ContentType.GetMimeType();
		public Encoding Charset => this.Headers.GetEncoding();
		public string BodyAsString => this.Body?.ToString(this.Charset);

		public override string ToString()
			=> $"{this.StatusLine}{Environment.NewLine}" +
			   $"{this.Headers.HeadersInOrder}{Environment.NewLine}" +
			   $"{this.BodyAsString}{Environment.NewLine}";
	}
}
