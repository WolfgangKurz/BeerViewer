using System;

using BeerViewer.Models.Enums;
using BeerViewer.Models.Raw;
using BeerViewer.Models.kcsapi;

namespace BeerViewer.Models.Wrapper
{
	public class Quest : RawDataWrapper<kcsapi_quest>, IIdentifiable
	{
		public int Id => this.RawData.api_no;

		public QuestCategory Category => (QuestCategory)this.RawData.api_category;
		public QuestType Type => (QuestType)this.RawData.api_type;
		public QuestState State => (QuestState)this.RawData.api_state;
		public QuestProgress Progress => (QuestProgress)this.RawData.api_progress_flag;

		public string Title => this.RawData.api_title;
		public string Detail => this.RawData.api_detail.Replace("<br>", Environment.NewLine);

		public Quest(kcsapi_quest api_data) : base(api_data) { }

		public override string ToString()
			=> $"ID = {this.Id}, Category = {this.Category}, Title = \"{this.Title}\", Type = {this.Type}, State = {this.State}";
	}
}
