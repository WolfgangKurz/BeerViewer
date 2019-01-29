using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace BeerViewer.Modules
{
	public class i18n
	{
		public static i18n i { get; } = new i18n();

		public static i18nLang Current => Settings.UseTranslation
			? i18n.i[Settings.LanguageCode]
			: i18nLang.Empty;

		protected Dictionary<string, i18nLang> Languages { get; set; }
		public string[] LanguageList => this.Languages.Keys.Where(x => x != "g").ToArray();

		public i18nLang this[string lang]
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

			Languages = new Dictionary<string, i18nLang>();
			LoadRecursive(@".\i18n", "*.txt");
			LoadRecursive(@".\WindowFrame");
		}

		private void LoadLanguage(IDictionary<string, string> list)
		{
			foreach (var x in list)
			{
				var y = x.Key;
				if (y.StartsWith("i18n.")) y = y.Substring(5);
				if (y.Contains("_"))
				{
					var sublang = y.Contains("_"); // Is this "LANG_SUB.txt" ?
					var origin = sublang ? y.Substring(0, y.IndexOf("_")) : y; // Without SUB

					if (Languages.ContainsKey(origin)) // Already registered
						Languages[origin].Update(x.Value);

					else if (!sublang) // Base language
						Languages.Add(origin, new i18nLang(x.Value));

					else if (!list.Any(m =>
							 m.Key.StartsWith(origin + ".")
							 || m.Key.StartsWith(origin + "_")
							 || m.Key.StartsWith("i18n." + origin + ".")
							 || m.Key.StartsWith("i18n." + origin + "_")
						))
						/* Sub language without base language
						 * ex)
						 * i18n.ko.txt (for module directory)
						 * i18n.ko_equip.txt
						 * ko.txt (for i18n directory)
						 * ko_equip.txt
						 */
						Languages.Add(origin, new i18nLang(x.Value));
				}

				else if (Languages.ContainsKey(y)) // Already registered
					Languages[y].Update(x.Value);

				else
					Languages.Add(y, new i18nLang(x.Value));
			}
		}
		private void LoadRecursive(string dir, string pattern = "i18n.*.txt")
		{
			this.LoadLanguage(
				Directory.GetFiles(dir, pattern)
					.ToDictionary(x => Path.GetFileNameWithoutExtension(x), x => x)
			);

			var dirs = Directory.GetDirectories(dir);
			foreach (var _dir in dirs) this.LoadRecursive(_dir);
		}
	}
	public class i18nLang
	{
		private static Regex regex { get; } = new Regex(@"^([^\t]+)\t+(.+)$", RegexOptions.Compiled);

		public static i18nLang Empty { get; } = new i18nLang();
		public IReadOnlyDictionary<string, string> Table { get; private set; }

		public i18nLang()
		{
			this.Table = new Dictionary<string, string>(); // Just empty table
		}
		public i18nLang(string fullPath)
		{
			var Language = Path.GetFileNameWithoutExtension(fullPath);
			var filename = $"{Language}.txt";
			var path = Path.GetDirectoryName(fullPath);

			if (File.Exists(fullPath))
			{
				this.Table = File.ReadAllLines(fullPath)
					.Select(x => x.Trim())
					.Where(x => !string.IsNullOrWhiteSpace(x) && x.Contains('\t'))
					.Select(x => regex.Match(x).Groups)
					.ToDictionary(
						x => x[1].Value,
						x => x[2].Value
					);
			}
		}

		public string this[string text]
		{
			get
			{
				string value;
				if (Table.TryGetValue(text, out value))
					return value;
				else
					return text;
			}
		}

		internal void Update(string fullPath)
		{
			var Language = Path.GetFileNameWithoutExtension(fullPath);
			var filename = $"{Language}.txt";
			var path = Path.GetDirectoryName(fullPath);

			if (File.Exists(fullPath))
			{
				var _ = File.ReadAllLines(fullPath)
					.Select(x => x.Trim())
					.Where(x => !string.IsNullOrWhiteSpace(x) && x.Contains('\t'))
					.Select(x => regex.Match(x).Groups)
					.ToDictionary(
						x => x[1].Value,
						x => x[2].Value
					);

				var target = this.Table as IDictionary<string, string>;
				foreach (var item in _)
				{
					if (target.ContainsKey(item.Key))
						target[item.Key] = item.Value;
					else
						target.Add(item);
				}
			}
		}
	}
}
