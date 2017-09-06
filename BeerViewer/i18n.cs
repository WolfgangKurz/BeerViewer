using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace BeerViewer
{
	public class i18n
	{
		public static i18n i { get; } = new i18n();

		public static dynamic/*i18nLang*/ Current => Settings.UseTranslation.Value
			? i18n.i[Settings.LanguageCode.Value]
			: i18nLang.Empty;

		protected Dictionary<string, i18nLang> Languages { get; set; }
		public dynamic/*i18nLang*/ this[string lang]
		{
			get
			{
				var key = lang.ToLower();

				if (Languages.ContainsKey(key))
					return Languages[key];

				return Languages.FirstOrDefault().Value;
			}
		}

		public i18n()
		{
			var path = Path.Combine(
				Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
				"i18n"
			);

			var list = Directory.GetFiles(path, "*.txt")
				.Select(x => Path.GetFileNameWithoutExtension(x));

			Languages = new Dictionary<string, i18nLang>();

			if (list.Contains("en")) // Default langauge
			{
				Languages.Add("en", new i18nLang("en"));
				list = list.Where(x => x != "en");
			}
			foreach (var x in list)
				Languages.Add(x, new i18nLang(x));
		}
	}
	public class i18nLang : DynamicObject
	{
		private static Regex regex { get; } = new Regex(@"^([^\t]+)\t+(.+)$", RegexOptions.Compiled);

		public static i18nLang Empty { get; } = new i18nLang();
		private Dictionary<string, string> Table { get; set; }

		public i18nLang()
		{
			this.Table = new Dictionary<string, string>(); // Just empty table
		}
		public i18nLang(string Language)
		{
			var path = Path.Combine(
				Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
				"i18n"
			);
			var filename = $"{Language}.txt";
			var fullPath = Path.Combine(path, filename);

			if (File.Exists(fullPath))
			{
				this.Table =
					Directory.GetFiles(path, "*.txt")
						.Select(x => Path.GetFileName(x))
						.Where(x => x == filename || x.StartsWith(Language + "_"))
						.Select(x => Path.Combine(path, x))
						.SelectMany(y =>
							File.ReadAllLines(y)
								.Select(x => x.Trim())
								.Where(x => !string.IsNullOrWhiteSpace(x) && x.Contains('\t'))
								.Select(x => regex.Match(x).Groups)
								.ToDictionary(
									x => x[1].Value,
									x => x[2].Value
								)
						)
						.ToDictionary(x => x.Key, x => x.Value);
			}
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			string sRet;
			var bRet = Table.TryGetValue(binder.Name, out sRet);
			if (!bRet)
			{
				result = binder.Name; // default(object);
				return true;
			}
			result = sRet;
			return true;
		}

		public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
		{
			if (indexes.Length != 1) throw new NotImplementedException();

			string sRet;
			var bRet = Table.TryGetValue(indexes[0] as string, out sRet);
			if (!bRet)
			{
				result = indexes[0] as string; // default(object);
				return true;
			}
			result = sRet;
			return true;
		}

		public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
			=> throw new NotImplementedException();

		public override bool TrySetMember(SetMemberBinder binder, object value)
			=> throw new NotImplementedException();

		public override bool TryDeleteMember(DeleteMemberBinder binder)
			=> throw new NotImplementedException();

		public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes)
			=> throw new NotImplementedException();
	}
}
