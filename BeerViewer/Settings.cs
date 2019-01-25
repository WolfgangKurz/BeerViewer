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


		/// <summary>
		/// Use translation? (use Japanese?)
		/// </summary>
		public static SettableSettingValue<bool> UseTranslation { get; } = new SettableSettingValue<bool>(
			getKey(), "Setting", true,
			"Use Translation"
		);

		/// <summary>
		/// Use hardware acceleration for CEF browser?
		/// </summary>
		public static SettableSettingValue<bool> HardwareAccelerationEnabled { get; } = new SettableSettingValue<bool>(
			getKey(), "Setting", true,
			"Enable Hardware Acceleration",
			"Use hardware acceleration for CEF browser.",
			"Restart viewer required for apply this setting."
		);

		/// <summary>
		/// Current Language Code
		/// </summary>
		public static SettableSettingValue<string> LanguageCode { get; } = new SettableSettingValue<string>(
			getKey(), "Setting", "ko",
			"Display Language", "", "",
			Modules.i18n.i.LanguageList
		);

		/// <summary>
		/// Includes LoS calculator first fleet's LoS?
		/// </summary>
		public static SettableSettingValue<bool> IsLoSIncludeFirstFleet { get; } = new SettableSettingValue<bool>(
			getKey(), "Setting", true,
			"LoS Includes 1st Fleet?"
		);

		/// <summary>
		/// Includes LoS calculator seconds fleet's LoS?
		/// </summary>
		public static SettableSettingValue<bool> IsLoSIncludeSecondFleet { get; } = new SettableSettingValue<bool>(
			getKey(), "Setting", false,
			"LoS Includes 2nd Fleet?"
		);

		/// <summary>
		/// LOS calculator's formula
		/// </summary>
		public static SettableSettingValue<string> LoSCalculator { get; } = new SettableSettingValue<string>(
			getKey(), "Setting", "Cn1",
			"LoS Calculator"
		);

		/// <summary>
		/// Notification adjust time (for expedition, construction, etc.)
		/// </summary>
		public static SettableSettingValue<int> NotifationConditionValue { get; } = new SettableSettingValue<int>(
			getKey(), "Setting", 49,
			"Notification Condition Value"
		);

		/// <summary>
		/// Notification adjust time (for expedition, construction, etc.)
		/// </summary>
		public static SettableSettingValue<int> NotificationTime { get; } = new SettableSettingValue<int>(
			getKey(), "Setting", 60,
			"Notification Time"
		);

		/// <summary>
		/// Notification adjust time (for expedition, construction, etc.)
		/// </summary>
		public static SettingValue<WindowInfo> WindowInformation { get; } = new SettingValue<WindowInfo>(
			getKey(), "Setting", new WindowInfo(null, null, 1440, 840)
		);
	}
}
