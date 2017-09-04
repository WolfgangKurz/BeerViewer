using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace BeerViewer
{
	public class i18n
	{
		public static i18n i { get; } = new i18n();

		public static dynamic/*i18nLang*/ Current
			=> i18n.i[Settings.LanguageCode.Value];

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
			foreach (var x in list)
				Languages.Add(x, new i18nLang(x));
		}
	}
	public class i18nLang : DynamicObject
	{
		private Dictionary<string, string> Table { get; set; }

		public i18nLang(string Language)
		{
			var path = Path.Combine(
				Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
				"i18n",
				$"{Language}.txt"
			);

			if (File.Exists(path))
			{
				this.Table =
					File.ReadAllLines(path)
						.Select(x => x.Trim())
						.Where(x => !string.IsNullOrWhiteSpace(x) && x.Contains('\t'))
						.ToDictionary(
							x => x.Substring(0, x.IndexOf('\t')),
							x => x.Substring(x.IndexOf('\t') + 1)
						);
			}
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			string sRet;
			var bRet = Table.TryGetValue(binder.Name, out sRet);
			if (!bRet)
			{
				result = default(object);
				return false;
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
				result = default(object);
				return false;
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
