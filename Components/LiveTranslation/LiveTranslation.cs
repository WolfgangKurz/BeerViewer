using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using BeerViewer.Network;
using BeerViewer.Modules;

namespace LiveTranslation
{
	[Export(typeof(IBeerComponent))]
	[ExportMetadata("Guid", "91A946EE-1FDC-4C5D-8E1A-F3ECE38B42AA")]
	[ExportMetadata("Name", "LiveTranslation")]
	[ExportMetadata("Description", "In-game translation for korean user")]
	[ExportMetadata("Version", "1.1")]
	[ExportMetadata("Author", "WolfgangKurz")]
	[ExportMetadata("Permission", BeerComponentPermission.CP_NETWORK_MODIFIABLE)]
	public class LiveTranslation : IBeerComponent
	{
		public string Name => "LiveTranslation";

		public void Initialize()
		{
			Logger.Log(
				"LiveTranslation ver " +
					this.GetType()
						.GetCustomAttributes(false)
						.Where(x => x.GetType() == typeof(ExportMetadataAttribute))
						.Select(x => x as ExportMetadataAttribute)
						.FirstOrDefault(x => x.Name == "Version")
						.Value
			);

			var proxy = Proxy.Instance;
			proxy.RegisterModifiable((s, b) =>
			{
				if (s.Request.PathAndQuery == "/kcsapi/api_start2/getData")
				{
					var utf8 = Encoding.UTF8;
					var src = utf8.GetString(b);
					/*
					src = new Regex("(\"api_map_bgm\"):\\[[0-9]+,[0-9]+\\]")
						.Replace(src, "$1:[67,67]");

					src = new Regex("(\"api_boss_bgm\"):\\[[0-9]+,[0-9]+\\]")
						.Replace(src, "$1:[17,17]");
					*/
					var list = i18n.Current.Table
						.OrderByDescending(x => x.Key.Length);
					foreach (var pair in list)
					{
						var key = pair.Key;
						src = src.Replace(
							$"\"{EncodeUnicode(key)}\"",
							$"\"{EncodeUnicode(pair.Value)}\""
						);
					}

					b = utf8.GetBytes(src);
				}
				/*
				else if (s.Request.PathAndQuery == "/kcsapi/api_port/port")
				{
					var utf8 = Encoding.UTF8;
					var src = utf8.GetString(b);

					src = new Regex("(\"api_p_bgm_id\"):[0-9]+,")
						.Replace(src, "$1:125,");

					b = utf8.GetBytes(src);
				}
				*/

				return b;
			});

			;
		}

		public void ReadyFailed()
		{
			// Nothing to do
		}

		private string EncodeUnicode(string value)
		{
			var sb = new StringBuilder();
			foreach (var c in value)
			{
				if (c > 127)
					sb.Append("\\u" + ((int)c).ToString("x4"));
				else
					sb.Append(c);
			}
			return sb.ToString();
		}
	}
}
