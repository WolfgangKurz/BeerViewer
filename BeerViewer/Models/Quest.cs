using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Core;
using BeerViewer.Models.Raw;

namespace BeerViewer.Models
{
	public class Quest : RawDataWrapper<kcsapi_quest>, IIdentifiable
	{
		public int Id => this.RawData.api_no;
		public string QuestId => QuestNameTable.IdNameTable.ContainsKey(this.Id)
			? QuestNameTable.IdNameTable[this.Id]
			: this.Id.ToString();

		public QuestCategory Category => (QuestCategory)this.RawData.api_category;
		public QuestType Type => (QuestType)this.RawData.api_type;
		public QuestState State => (QuestState)this.RawData.api_state;
		public QuestProgress Progress => (QuestProgress)this.RawData.api_progress_flag;

		public string Title
		{
			get
			{
				if (Translator.QuestNameTable.ContainsKey(this.RawData.api_title))
					return Translator.QuestNameTable[this.RawData.api_title];

				if (Translator.QuestIdTable.ContainsKey(this.Id.ToString()))
					return Translator.QuestIdTable[this.Id.ToString()].Name;

				if (Translator.QuestIdTable.ContainsKey(this.QuestId))
					return Translator.QuestIdTable[this.QuestId].Name;

				return this.RawData.api_title;
			}
		}
		public string Detail
		{
			get
			{
				if (Translator.QuestDetailTable.ContainsKey(this.RawData.api_title))
					return Translator.QuestDetailTable[this.RawData.api_title];

				if (Translator.QuestIdTable.ContainsKey(this.Id.ToString()))
					return Translator.QuestIdTable[this.Id.ToString()].Detail;

				if (Translator.QuestIdTable.ContainsKey(this.QuestId))
					return Translator.QuestIdTable[this.QuestId].Detail;

				return this.RawData.api_detail.Replace("<br>", Environment.NewLine);
			}
		}

		public Quest(kcsapi_quest rawData) : base(rawData) { }

		public override string ToString()
		{
			return $"ID = {this.Id}, Category = {this.Category}, Title = \"{this.Title}\", Type = {this.Type}, State = {this.State}";
		}
	}
}
