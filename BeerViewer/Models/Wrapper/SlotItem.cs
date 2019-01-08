using BeerViewer.Models;
using BeerViewer.Models.Raw;
using BeerViewer.Models.kcsapi;


namespace BeerViewer.Models.Wrapper
{
	public class SlotItem : RawDataWrapper<kcsapi_slotitem>, IIdentifiable
	{
		public int Id => this.RawData.api_id;
		public SlotItemInfo Info { get; private set; }

		public int Level => this.RawData.api_level;
		public string LevelText => this.Level >= 10 ? "★max" : this.Level >= 1 ? ("★+" + this.Level) : "";
		public string NameWithLevel => $"{this.Info.Name}{(this.Level >= 1 ? (" " + this.LevelText) : "")}{(this.Proficiency >= 1 ? (" " + this.ProficiencyText) : "")}";

		public int Proficiency => this.RawData.api_alv;
		public string ProficiencyText => this.Proficiency >= 1 ? (" (Proficiency " + this.Proficiency + ")") : "";

		internal SlotItem(kcsapi_slotitem Data) : base(Data)
		{
			if (Master.Instance.SlotItems?.ContainsKey(this.RawData.api_slotitem_id) ?? false)
				this.Info = Master.Instance.SlotItems[this.RawData.api_slotitem_id] ?? SlotItemInfo.Empty;
			else
				this.Info = SlotItemInfo.Empty;
		}

		public void Remodel(int level, int masterId)
		{
			this.RawData.api_level = level;
			this.Info = Master.Instance.SlotItems[masterId] ?? SlotItemInfo.Empty;

			this.RaisePropertyChanged(nameof(this.Info));
			this.RaisePropertyChanged(nameof(this.Level));
		}

		public override string ToString()
			=> $"ID = {this.Id}, Name = \"{this.Info.Name}\", Level = {this.Level}, Proficiency = {this.Proficiency}";

		public static SlotItem Empty { get; } = new SlotItem(new kcsapi_slotitem
		{
			api_slotitem_id = -1
		});
	}
}
