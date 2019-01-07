/*
 * Based on MetroTrilithon/source/MetroTrilithon.Desktop/Desktop/SystemEnvironment.cs
 *      and MetroTrilithon/source/MetroTrilithon.Desktop/Desktop/DotNetVersion.cs
 * Project page: https://github.com/Grabacr07/MetroTrilithon
 * Author: Grabacr07
 * License: MIT (https://github.com/Grabacr07/MetroTrilithon/blob/master/LICENSE.txt)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using Microsoft.Win32;

namespace MetroTrilithon.Desktop
{
	public class SystemEnvironment
	{
		public string OS { get; }
		public string OSVersion { get; }
		public string DesktopArchitecture { get; }
		public string AppArchitecture { get; }

		public string CPU { get; }
		public string TotalPhysicalMemorySize { get; }
		public string FreePhysicalMemorySize { get; }

		public string DotNetVersion { get; }

		public string ErrorMessage { get; }

		public SystemEnvironment()
		{
			try
			{
				this.AppArchitecture = Environment.Is64BitProcess ? "x64" : "x86";
				this.DesktopArchitecture = Environment.Is64BitOperatingSystem ? "x64" : "x86";

				using (var managementClass = new ManagementClass("Win32_OperatingSystem"))
				{
					using (var managementObject = managementClass.GetInstances().OfType<ManagementObject>().FirstOrDefault())
					{
						if (managementObject == null) return;

						this.OS = managementObject["Caption"].ToString();
						this.OSVersion = managementObject["Version"].ToString();

						this.TotalPhysicalMemorySize = $"{managementObject["TotalVisibleMemorySize"]:N0} KBs";
						this.FreePhysicalMemorySize = $"{managementObject["FreePhysicalMemory"]:N0} KBs";
					}
				}

				using (var managementClass = new ManagementClass("Win32_Processor"))
				{
					using (var managementObject = managementClass.GetInstances().OfType<ManagementObject>().FirstOrDefault())
					{
						if (managementObject == null) return;

						this.CPU = managementObject["Name"].ToString();
					}
				}

				this.DotNetVersion = Desktop.DotNetVersion.GetVersion();
			}
			catch (Exception ex)
			{
				this.ErrorMessage = ex.Message;
			}
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(this.ErrorMessage))
				return $@"## System Environment
({this.ErrorMessage})";

			return $@"## System Environment
- OS:           {this.OS} ({this.OSVersion})
- Architecture: {this.AppArchitecture} app on {this.DesktopArchitecture} desktop
- Runtime:      {this.DotNetVersion}

- CPU:          {this.CPU}
- RAM:          {this.FreePhysicalMemorySize} free / {this.TotalPhysicalMemorySize} total";
		}
	}

	public class DotNetVersion
	{
		public static string GetVersion()
		{
			const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
			var version = "";

			using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
				if (ndpKey?.GetValue("Release") is int value)
					version = GetVersionCore(value);

			if (string.IsNullOrEmpty(version))
				version = Environment.Version.ToString();

			return $".NET Framework {version}";
		}

		private static string GetVersionCore(int releaseKey)
		{
			if (releaseKey >= 461808) return "4.7.2 or later";
			if (releaseKey >= 461308) return "4.7.1";
			if (releaseKey >= 460798) return "4.7";
			if (releaseKey >= 394802) return "4.6.2";
			if (releaseKey >= 394254) return "4.6.1";
			if (releaseKey >= 393295) return "4.6";
			if (releaseKey >= 379893) return "4.5.2";
			if (releaseKey >= 378675) return "4.5.1";
			if (releaseKey >= 378389) return "4.5";
			return "";
		}
	}
}
