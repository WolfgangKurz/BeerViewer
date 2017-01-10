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
				if (admiral.Level == 0) return;
				if (admiral.Experience == 0) return;

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

				var list = Record
					.Where(x => x.HQLevel > 0 || x.HQExp > 0)
					.OrderBy(x => x.Date);

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
					DateTime? time;
					int level, exp;

					time = CSVStringToTime(elem[0]);
					if (!time.HasValue) continue;
					if (!int.TryParse(elem[1], out level)) continue;
					if (!int.TryParse(elem[2], out exp)) continue;
					if (level <= 0 || exp <= 0) continue;

					Record.Add(new HQRecordElement(time.Value, level, exp));
					linecount++;
				}
			}
		}

		public HQRecordElement GetRecord(DateTime target)
			=> Record.OrderBy(x => x.Date).FirstOrDefault(x => x.Date >= target)
			?? new HQRecordElement(DateTime.MinValue, 0, 0);

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

		public static DateTime? CSVStringToTime(string str)
		{
			DateTime time;
			if (!DateTime.TryParse(str, out time)) return null;
			return time;
		}
	}
}
