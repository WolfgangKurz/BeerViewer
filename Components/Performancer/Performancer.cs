using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using BeerViewer.Network;
using BeerViewer.Modules;

namespace Performancer
{
	[Export(typeof(IBeerComponent))]
	[ExportMetadata("Guid", "8175CBCE-29DF-47CA-AB6C-AC8038A7DC9B")]
	[ExportMetadata("Name", "Performancer")]
	[ExportMetadata("Description", "Experience better gaming performance")]
	[ExportMetadata("Version", "1.0")]
	[ExportMetadata("Author", "WolfgangKurz")]
	[ExportMetadata("Permission", BeerComponentPermission.CP_NETWORK_MODIFIABLE)]
	public class Performancer : IBeerComponent
	{
		public string Name => "Performancer";

		public void Initialize()
		{
			Logger.Log(
				"Performancer ver " +
					this.GetType()
						.GetCustomAttributes(false)
						.Where(x => x.GetType() == typeof(ExportMetadataAttribute))
						.Select(x=>x as ExportMetadataAttribute)
						.FirstOrDefault(x=>x.Name == "Version")
						.Value
			);

			var proxy = Proxy.Instance;
			proxy.RegisterModifiable(r =>
			{
				var b = BlockURIs.IsBlockURI(r.Host, r.PathAndQuery);
				// if (b) Logger.Log($"Performancer Block: {r.Host}");
				return !b;
			});
			proxy.RegisterModifiable((s, b) =>
			{
				if (!s.Request.PathAndQuery.StartsWith("/kcs2/index.php"))
					return b;

				var utf8 = Encoding.UTF8;
				var str = utf8.GetString(b);

				str = str.Replace("KCS.init();", "PIXI.settings.RENDER_OPTIONS.clearBeforeRender=false; KCS.init();");

				return utf8.GetBytes(str);
			});
			proxy.RegisterModifiable((s, b) =>
			{
				if (!s.Request.PathAndQuery.Contains("/app_id=854854/"))
					return b;

				var utf8 = Encoding.UTF8;
				var str = utf8.GetString(b);

				var script = @"<script type=""text/javascript"">
setInterval(function(){
	var x = document.querySelectorAll(""[style=\""display: none; visibility: hidden;\""],[name=\""gdm_advFrame\""]"");
	for(var i=0; i<x.length; i++) x[i].parentNode.removeChild(x[i]);
}, 1000);
</script>";

				str = str.Replace("</body>", script + "</body>");

				return utf8.GetBytes(str);
			});
		}

		public void ReadyFailed()
		{
			// Nothing to do
		}
	}
}
