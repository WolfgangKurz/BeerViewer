using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Threading;
using System.Reflection;

using BeerViewer.Models.Raw;

namespace BeerViewer.Core
{
	internal enum CheckState
	{
		NetError = -1,
		DataError = -2,

		Required = 0,
		Pending = 1,
		Latest = 10,
		Updatable = 20
	}

	internal class VersionChecker : Notifier
	{
		#region State 프로퍼티
		private CheckState _State { get; set; } = CheckState.Required;
		public CheckState State
		{
			get { return this._State; }
			set
			{
				if (this._State != value)
				{
					this._State = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region Version 프로퍼티
		private string _Version { get; set; } = "*Unknown";
		public string Version
		{
			get { return this._Version; }
			set
			{
				if(this._Version != value)
				{
					this._Version = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region UpdateURL 프로퍼티
		private string _UpdateURL { get; set; } = "#";
		public string UpdateURL
		{
			get { return this._UpdateURL; }
			set
			{
				if (this._UpdateURL != value)
				{
					this._UpdateURL = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		public void RequestCheck()
		{
			if (State == CheckState.Pending) return;

			State = CheckState.Pending;
			new Thread(() =>
			{
				string content = string.Empty;
				try
				{
					WebRequest request = WebRequest.Create(Const.UpdateURL);
					WebResponse response = request.GetResponse();
					Stream data = response.GetResponseStream();
					using (StreamReader sr = new StreamReader(data))
						content = sr.ReadToEnd();
				}
				catch { }

				if (string.IsNullOrEmpty(content))
				{
					State = CheckState.NetError;
					return;
				}

				string[] lines = content.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
				if (lines.Length != 2)
				{
					State = CheckState.DataError;
					return;
				}

				var isValid = new Regex(@"[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+").IsMatch(lines[0]);
				if (!isValid)
				{
					State = CheckState.DataError;
					return;
				}

				Assembly assembly = Assembly.GetExecutingAssembly();
				Version Version = assembly.GetName().Version;

				int[] part = lines[0].Split('.').Select(x => int.Parse(x)).ToArray();
				if (part[0] > Version.Major)
					State = CheckState.Updatable;
				else if (part[0] == Version.Major && part[1] > Version.Minor)
					State = CheckState.Updatable;
				else if (part[0] == Version.Major && part[1] == Version.Minor && part[2] > Version.Revision)
					State = CheckState.Updatable;
				else if (part[0] == Version.Major && part[1] == Version.Minor && part[2] == Version.Revision && part[3] > Version.Build)
					State = CheckState.Updatable;
				else
					State = CheckState.Latest;
			}).Start();
		}
	}
}
