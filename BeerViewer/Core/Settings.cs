using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codeplex.Data;

using BeerViewer.Models.Raw;

namespace BeerViewer.Core
{
	internal class Settings
	{
		public static SettingValue<double> BrowserZoom { get; set; } = new SettingValue<double>(GetKey(), 1.0);
		public static SettingValue<bool> VerticalMode { get; set; } = new SettingValue<bool>(GetKey(), false);

		public static SettingValue<int> ResourceSelected1 { get; set; } = new SettingValue<int>(GetKey(), 0);
		public static SettingValue<int> ResourceSelected2 { get; set; } = new SettingValue<int>(GetKey(), 1);

		public static SettingValue<bool> IsViewRangeCalcIncludeFirstFleet { get; set; } = new SettingValue<bool>(GetKey(), true);
		public static SettingValue<bool> IsViewRangeCalcIncludeSecondFleet { get; set; } = new SettingValue<bool>(GetKey(), false);

		public static SettingValue<string> ViewRangeCalcType { get; set; } = new SettingValue<string>(GetKey(), "ViewRange.Type4");

		public static SettingValue<bool> Notify_ExpeditionComplete { get; set; } = new SettingValue<bool>(GetKey(), true);
		public static SettingValue<bool> Notify_BuildComplete { get; set; } = new SettingValue<bool>(GetKey(), false);
		public static SettingValue<bool> Notify_RepairComplete { get; set; } = new SettingValue<bool>(GetKey(), true);
		public static SettingValue<bool> Notify_ConditionComplete { get; set; } = new SettingValue<bool>(GetKey(), true);

		public static SettingValue<bool> BattleInfo_AutoSelectTab { get; set; } = new SettingValue<bool>(GetKey(), true);
		public static SettingValue<bool> BattleInfo_DetailKouku { get; set; } = new SettingValue<bool>(GetKey(), true);

		public static SettingValue<bool> BattleInfo_FirstIsCritical { get; set; } = new SettingValue<bool>(GetKey(), false);
		public static SettingValue<bool> BattleInfo_SecondIsCritical { get; set; } = new SettingValue<bool>(GetKey(), false);
		public static SettingValue<bool> BattleInfo_EnableColorChange { get; set; } = new SettingValue<bool>(GetKey(), true);

		public static SettingValue<bool> BattleInfo_CriticalEnabled { get; set; } = new SettingValue<bool>(GetKey(), true);
		public static SettingValue<bool> BattleInfo_IsEnabledBattleEndNotify { get; set; } = new SettingValue<bool>(GetKey(), true);


		internal class SettingValue<T> : Notifier
		{
			public string Key { get; }

			private T _Value { get; set; } = default(T);
			public T Value
			{
				get { return this._Value; }
				set
				{
					if (!this._Value.Equals(value))
					{
						this._Value = value;
						this.RaisePropertyChanged();
						Settings.Save();
					}
				}
			}

			public SettingValue(string Key) : this(Key, default(T))
			{
			}
			public SettingValue(string Key, T defaultValue)
			{
				this.Key = Key;

				this._Value = Settings.Load<T>(Key, defaultValue);

				if (Settings.settingValueList == null)
					Settings.settingValueList = new List<object>();
				Settings.settingValueList.Add(this);
			}

			public override string ToString()
			{
				var type = Value.GetType();
				string value;

				if (type == typeof(bool))
					value = Value.ToString().ToLower();
				else if (type == typeof(string))
					value = $"\"{Value.ToString()}\"";
				else
					value = Value.ToString();

				return $"{Key}={value}";
			}
		}
		private static string GetKey([CallerMemberName] string propertyName = "")
			=> nameof(Settings) + "." + propertyName;

		private static string SettingsPath =>
			Path.Combine(
				Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
				"Settings.config"
			);

		public static List<object> settingValueList { get; private set; }
		private static void Save()
		{
			if (settingValueList == null) return;

			StringBuilder sb = new StringBuilder();
			foreach (var item in settingValueList)
				sb.AppendLine(item.ToString());

			try
			{
				File.WriteAllText(SettingsPath, sb.ToString());
			}
			catch { }
		}
		private static string Load(string Key)
		{
			if (!File.Exists(SettingsPath)) return null;

			var lines = File.ReadAllLines(SettingsPath)
				.Select(x => x.Trim())
				.Where(x => x.Length > 0);

			foreach(var line in lines)
			{
				if (!line.Contains("=")) continue;

				var part = line.Split(new char[] { '=' }, 2);
				if (part[0] == Key) return part[1];
			}
			return null;
		}
		private static T Load<T>(string Key, T defaultValue)
		{
			var value = Settings.Load(Key);
			if (value == null) return defaultValue;

			try
			{
				return (T)DynamicJson.Parse(value);
			}
			catch { }
			return defaultValue;
		}
	}
}
