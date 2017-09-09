using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpRequest = Nekoxy.HttpRequest;
using HttpResponse = Nekoxy.HttpResponse;

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
			=> string.Format(
					"{0}{1}{1}{2}",
					this.Request,
					Environment.NewLine,
					this.Response
				);
	}
}
