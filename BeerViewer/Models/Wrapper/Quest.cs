﻿using System;

using BeerViewer.Models.Enums;
using BeerViewer.Models.Raw;
using BeerViewer.Models.kcsapi;

namespace BeerViewer.Models.Wrapper
{
	public class Quest : SvData<kcsapi_quest>, IIdentifiable
	{
		public int Id => this.api_data.api_no;

		public QuestCategory Category => (QuestCategory)this.api_data.api_category;
		public QuestType Type => (QuestType)this.api_data.api_type;
		public QuestState State => (QuestState)this.api_data.api_state;
		public QuestProgress Progress => (QuestProgress)this.api_data.api_progress_flag;

		public string Title => this.api_data.api_title;
		public string Detail => this.api_data.api_detail.Replace("<br>", Environment.NewLine);

		public Quest(kcsapi_quest api_data) : base(api_data) { }

		public override string ToString()
			=> $"ID = {this.Id}, Category = {this.Category}, Title = \"{this.Title}\", Type = {this.Type}, State = {this.State}";
	}
}