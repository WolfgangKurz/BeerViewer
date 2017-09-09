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
		public string Name => throw new NotImplementedException();

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
			proxy.RegisterModifiable("/gadget/js/kcs_flash.js", (s, b) =>
			{
				Logger.Log("Flash quality adjusting");

				var utf8 = Encoding.UTF8;

				var str = utf8.GetString(b);
				str = new Regex("(\"quality\" +: +\")high\"")
					.Replace(str, "$1low\"");

				return utf8.GetBytes(str);
			});
		}

		public void ReadyFailed()
		{
			// Nothing to do
		}
	}
}
