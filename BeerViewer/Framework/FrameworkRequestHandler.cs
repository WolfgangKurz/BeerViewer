using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Libraries;

using CefSharp;
using CefSharp.Handler;

namespace BeerViewer.Framework
{
	internal class FrameworkRequestHandler : DefaultRequestHandler
	{
		public override IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
		{
			if (request.Url.Contains("/login/")) return new FrameworkWelcomeFilter();

			return base.GetResourceResponseFilter(browserControl, browser, frame, request, response);
		}
	}

	internal class FrameworkWelcomeFilter : FindReplaceResponseFilter
	{
		public override string findString => "<div id=\"translate_welcome_div\"></div>";
		public override string replacementString => "<div id=\"translate_welcome_div\"></div><style>#welcome{display:none!important}</style>";
	}
}
