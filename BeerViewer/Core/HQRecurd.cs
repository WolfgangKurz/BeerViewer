using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace BeerViewer.Core
{
	public class HQRecord
	{
		public class HQRecordElement
		{
			/// <summary>
			/// 기록 시간
			/// </summary>
			public DateTime Date;

			/// <summary>
			/// 사령부 Lv
			/// </summary>
			public int HQLevel;

			/// <summary>
			/// 제독 Exp
			/// </summary>
			public int HQExp;

			public HQRecordElement()
			{
				Date = DateTime.Now;
			}

			public HQRecordElement(DateTime time, int level, int exp) : this()
			{
				Date = time;
				HQLevel = level;
				HQExp = exp;
			}

			public HQRecordElement(int level, int exp) : this()
			{
				HQLevel = level;
				HQExp = exp;
			}
		}
		public string RecordHeader => "#일시,사령부 Lv,제독 Exp";
		public string RecordFile
		{
			get
			{
				string dir = "";
				var entry = Assembly.GetEntryAssembly();

				if (entry != null)
					dir = Path.GetDirectoryName(entry.Location);
				else
					dir = "";

				return Path.Combine(dir, "HQRecord.csv");
			}
		}

		private List<HQRecordElement> Record;
		private DateTime _prevTime;
		private bool _initialFlag;

		public HQRecord()
		{
			Record = new List<HQRecordElement>();
			_prevTime = DateTime.Now;
			_initialFlag = true;
		}

		public void Updated()
		{
			if (_initialFlag || IsCrossedHour(_prevTime))
			{
				_prevTime = DateTime.Now;
				_initialFlag = false;

				var admiral = DataStorage.Instance.Homeport?.Admiral;
				if (admiral == null) return;

				Record.Add(
					new HQRecordElement(
						admiral.Level,
						admiral.Experience
					)
				);
				Save();
			}
		}

		public void Save()
		{
			using (StreamWriter sw = new StreamWriter(RecordFile, false, Encoding.UTF8))
			{
				//UTF-8 BOM
				sw.Write(0xEF);
				sw.Write(0xBB);
				sw.Write(0xBF);
				sw.WriteLine(RecordHeader);

				var list = new List<HQRecordElement>(Record);
				list.Sort((e1, e2) => e1.Date.CompareTo(e2.Date));

				foreach (var elem in list)
					sw.WriteLine(
						string.Format(
							"{0},{1},{2}",
							TimeToCSVString(elem.Date),
							elem.HQLevel,
							elem.HQExp
						)
					);
			}
		}
		public void Load()
		{
			if (!File.Exists(RecordFile)) return;

			using (StreamReader sr = new StreamReader(RecordFile, Encoding.UTF8))
			{
				Record.Clear();

				string line;
				int linecount = 1;
				sr.ReadLine();

				while ((line = sr.ReadLine()) != null)
				{
					if (line.Trim().StartsWith("#"))
						continue;

					string[] elem = line.Split(',');
					Record.Add(
						new HQRecordElement(
							CSVStringToTime(elem[0]),
							int.Parse(elem[1]),
							int.Parse(elem[2])
						)
					);
					linecount++;
				}
			}
		}

		public HQRecordElement GetRecord(DateTime target)
		{
			int i;
			for (i = Record.Count - 1; i >= 0; i--)
			{
				if (Record[i].Date < target)
				{
					i++;
					break;
				}
			}
			if (i < 0) i = 0;

			if (0 <= i && i < Record.Count)
				return Record[i];
			else
				return null;
		}
		public HQRecordElement GetRecordPrevious()
		{
			DateTime now = DateTime.Now;
			DateTime target;

			if (now.TimeOfDay.Hours < 2)
				target = new DateTime(now.Year, now.Month, now.Day, 14, 0, 0).Subtract(TimeSpan.FromDays(1));
			else if (now.TimeOfDay.Hours < 14)
				target = new DateTime(now.Year, now.Month, now.Day, 2, 0, 0);
			else
				target = new DateTime(now.Year, now.Month, now.Day, 14, 0, 0);

			return GetRecord(target);
		}
		public HQRecordElement GetRecordDay()
		{
			DateTime now = DateTime.Now;
			DateTime target;
			if (now.TimeOfDay.Hours < 2)
				target = new DateTime(now.Year, now.Month, now.Day, 2, 0, 0).Subtract(TimeSpan.FromDays(1));
			else
				target = new DateTime(now.Year, now.Month, now.Day, 2, 0, 0);

			return GetRecord(target);
		}
		public HQRecordElement GetRecordMonth()
		{
			DateTime now = DateTime.Now;
			return GetRecord(new DateTime(now.Year, now.Month, 1));
		}

		public static bool IsCrossedHour(DateTime prev)
		{
			DateTime nexthour = prev.Date.AddHours(prev.Hour + 1);
			return nexthour <= DateTime.Now;
		}
		public static string TimeToCSVString(DateTime time)
			=> time.ToString("yyyy\\/MM\\/dd HH\\:mm\\:ss", System.Globalization.CultureInfo.InvariantCulture);

		public static DateTime CSVStringToTime(string str)
		{
			string[] elem = str.Split("/ :".ToCharArray());
			return new DateTime(
				elem.Length > 0 ? int.Parse(elem[0]) : 1970,
				elem.Length > 1 ? int.Parse(elem[1]) : 1,
				elem.Length > 2 ? int.Parse(elem[2]) : 1,
				elem.Length > 3 ? int.Parse(elem[3]) : 0,
				elem.Length > 4 ? int.Parse(elem[4]) : 0,
				elem.Length > 5 ? int.Parse(elem[5]) : 0
			);
		}
	}
}
