using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models.BattleInfo
{
	public static class NotificationType
	{
		private static readonly string baseName = typeof(NotificationType).Assembly.GetName().Name;

		public static string BattleEnd = $"{baseName}.{nameof(BattleEnd)}";
		public static string ConfirmPursuit = $"{baseName}.{nameof(ConfirmPursuit)}";
		public static string CriticalState = $"{baseName}.{nameof(CriticalState)}";
	}
}
