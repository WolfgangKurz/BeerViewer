using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Threading;

namespace BeerViewer.Core
{
	public class Translator
	{
		private enum TranslateType
		{
			ShipTypes,
			ShipNames,
			SlotItemTypes,
			SlotItems,
			Quests,
			Operations,
		}
		public class QuestNameDetail
		{
			public string Name { get; set; }
			public string Detail { get; set; }
		}

		public static ConcurrentDictionary<int, string> ShipTypeTable { get; }
		public static ConcurrentDictionary<string, string> ShipNameTable { get; }
		public static ConcurrentDictionary<string, string> SlotItemTypeTable { get; }
		public static ConcurrentDictionary<string, string> SlotItemTable { get; }

		public static ConcurrentDictionary<string, string> QuestNameTable { get; }
		public static ConcurrentDictionary<string, string> QuestDetailTable { get; }
		public static ConcurrentDictionary<string, QuestNameDetail> QuestIdTable { get; }

		public static ConcurrentDictionary<string, string> OperationTable { get; }

		static Translator()
		{
			Translator.ShipTypeTable = new ConcurrentDictionary<int, string>();
			Translator.ShipNameTable = new ConcurrentDictionary<string, string>();
			Translator.SlotItemTypeTable = new ConcurrentDictionary<string, string>();
			Translator.SlotItemTable = new ConcurrentDictionary<string, string>();

			Translator.QuestNameTable = new ConcurrentDictionary<string, string>();
			Translator.QuestDetailTable = new ConcurrentDictionary<string, string>();
			Translator.QuestIdTable = new ConcurrentDictionary<string, QuestNameDetail>();

			Translator.OperationTable = new ConcurrentDictionary<string, string>();
		}

		internal static void Initialize()
		{
			new Thread(() =>
			{
				Translator.LoadTranslate(TranslateType.ShipTypes);
				Translator.LoadTranslate(TranslateType.ShipNames);
				Translator.LoadTranslate(TranslateType.SlotItemTypes);
				Translator.LoadTranslate(TranslateType.SlotItems);
				Translator.LoadTranslate(TranslateType.Quests);
				Translator.LoadTranslate(TranslateType.Operations);
			}).Start();
		}

		private static void LoadTranslate(TranslateType TranslateType)
		{
			var Filename = Path.Combine(
				Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
				"Translations",
				TranslateType.ToString() + ".xml"
			);
			if (!File.Exists(Filename)) return;

			XmlDocument xml = new XmlDocument();
			XmlNodeList nodes;

			try { xml.Load(Filename); }
			catch { return; }

			switch (TranslateType)
			{
				case TranslateType.ShipTypes:
					Translator.ShipTypeTable.Clear();
					nodes = xml.SelectNodes("/ShipTypes/Type");
					foreach (XmlNode node in nodes) {
						try
						{
							int id;
							if (!int.TryParse(node["ID"].InnerText, out id)) continue;
							Translator.ShipTypeTable.AddOrUpdate(id, node["Value"].InnerText, (a, b) => b);
						}
						catch { }
					}
					break;

				case TranslateType.ShipNames:
					Translator.ShipNameTable.Clear();
					nodes = xml.SelectNodes("/Ships/Ship");
					foreach (XmlNode node in nodes)
					{
						try { Translator.ShipNameTable.AddOrUpdate(node["Source"].InnerText, node["Value"].InnerText, (a, b) => b); }
						catch { }
					}
					break;

				case TranslateType.SlotItemTypes:
					Translator.SlotItemTypeTable.Clear();
					nodes = xml.SelectNodes("/SlotItemTypes/SlotItemType");
					foreach (XmlNode node in nodes)
					{
						try { Translator.SlotItemTypeTable.AddOrUpdate(node["Source"].InnerText, node["Value"].InnerText, (a, b) => b); }
						catch { }
					}
					break;

				case TranslateType.SlotItems:
					Translator.SlotItemTable.Clear();
					nodes = xml.SelectNodes("/SlotItems/SlotItem");
					foreach (XmlNode node in nodes)
					{
						try { Translator.SlotItemTable.AddOrUpdate(node["Source"].InnerText, node["Value"].InnerText, (a, b) => b); }
						catch { }
					}
					break;

				case TranslateType.Quests:
					Translator.QuestNameTable.Clear();
					Translator.QuestDetailTable.Clear();
					Translator.QuestIdTable.Clear();
					nodes = xml.SelectNodes("/Quests/Quest");
					foreach (XmlNode node in nodes)
					{
						try { Translator.QuestNameTable.AddOrUpdate(node["NameSource"].InnerText, node["NameValue"].InnerText, (a, b) => b); }
						catch { }

						try { Translator.QuestDetailTable.AddOrUpdate(node["DetailSource"].InnerText, node["DetailValue"].InnerText, (a, b) => b); }
						catch { }

						try {
							Translator.QuestIdTable.AddOrUpdate(
								node["ID"].InnerText,
								new QuestNameDetail
								{
									Name = node["NameValue"].InnerText,
									Detail = node["DetailValue"].InnerText
								},
								(a, b) => b
							);
						} catch { }
					}
					break;

				case TranslateType.Operations:
					Translator.OperationTable.Clear();
					nodes = xml.SelectNodes("/Operations/Map");
					foreach (XmlNode node in nodes)
					{
						try { Translator.OperationTable.AddOrUpdate(node["Source"].InnerText, node["Value"].InnerText, (a, b) => b); }
						catch { }
					}
					break;
			}
		}
	}
}
