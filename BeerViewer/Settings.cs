using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Drawing;
using BeerViewer.Framework;

namespace BeerViewer
{
	public partial class Settings
	{
		private static string getKey([CallerMemberName] string propertyName = "")
			=> propertyName;

		#region Language group
		/// <summary>
		/// Use translation? (or use Japanese?)
		/// </summary>
		public static SettableSettingValue<bool> UseTranslation { get; } = new SettableSettingValue<bool>(
			getKey(), "Language", true,
			"Use Translation"
		);

		/// <summary>
		/// Current Language Code
		/// </summary>
		public static SettableSettingValue<string> LanguageCode { get; } = new SettableSettingValue<string>(
			getKey(), "Language", "ko",
			"Display Language", "", "",
			Modules.i18n.i.LanguageList
		);
		#endregion

		#region System group
		/// <summary>
		/// Use hardware acceleration for CEF browser?
		/// </summary>
		public static SettableSettingValue<bool> HardwareAccelerationEnabled { get; } = new SettableSettingValue<bool>(
			getKey(), "System", true,
			"Enable Hardware Acceleration",
			"Use hardware acceleration for CEF browser.",
			"Restart viewer required for apply this setting."
		);
		#endregion

		#region LoS group
		/// <summary>
		/// LOS calculator's formula
		/// </summary>
		public static SettableSettingValue<string> LoSCalculator { get; } = new SettableSettingValue<string>(
			getKey(), "LoS", "Cn1",
			"LoS Calculator"
		);

		/// <summary>
		/// Includes LoS calculator first fleet's LoS?
		/// </summary>
		public static SettableSettingValue<bool> IsLoSIncludeFirstFleet { get; } = new SettableSettingValue<bool>(
			getKey(), "LoS", true,
			"LoS Includes 1st Fleet?"
		);

		/// <summary>
		/// Includes LoS calculator seconds fleet's LoS?
		/// </summary>
		public static SettableSettingValue<bool> IsLoSIncludeSecondFleet { get; } = new SettableSettingValue<bool>(
			getKey(), "LoS", false,
			"LoS Includes 2nd Fleet?"
		);
		#endregion

		#region Notification group
		/// <summary>
		/// Notification adjust time (for expedition, construction, etc.)
		/// </summary>
		public static SettableSettingValue<int> NotifationConditionValue { get; } = new SettableSettingValue<int>(
			getKey(), "Notification", 49,
			"Notification Condition Value"
		);

		/// <summary>
		/// Notification adjust time (for expedition, construction, etc.)
		/// </summary>
		public static SettableSettingValue<int> NotificationTime { get; } = new SettableSettingValue<int>(
			getKey(), "Notification", 60,
			"Notification Time"
		);
		#endregion

		#region MainFrame group
		/// <summary>
		/// MainFrame's zoom factor
		/// </summary>
		public static SettableSettingValue<double> ZoomFactor { get; } = new SettableSettingValue<double>(
			getKey(), "MainFrame", 1.0,
			"Game screen zoom factor"
		);
		#endregion

		/// <summary>
		/// Notification adjust time (for expedition, construction, etc.)
		/// </summary>
		public static SettingValue<WindowInfo> WindowInformation { get; } = new SettingValue<WindowInfo>(
			getKey(), "Setting", new WindowInfo(null, null, 1440, 840)
		);
	}
}
