using BeerViewer.Network;
using BeerViewer.Models;
using BeerViewer.Models.Enums;
using BeerViewer.Models.Raw;
using BeerViewer.Models.kcsapi;

namespace BeerViewer.Models.Wrapper
{
	public class Admiral : RawDataWrapper<kcsapi_basic>
	{
		public string MemberId => this.RawData.api_member_id;
		public string Nickname => this.RawData.api_nickname;

		#region Comment Property
		private string _Comment;
		public string Comment
		{
			get { return this._Comment; }
			set
			{
				if (this._Comment != value)
				{
					this._Comment = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		public int Experience => this.RawData.api_experience;
		public int ExperienceForNexeLevel => Models.Experience.GetAdmiralExpForNextLevel(this.RawData.api_level, this.RawData.api_experience);

		public int Level => this.RawData.api_level;
		public AdmiralRank Rank => (AdmiralRank)this.RawData.api_rank;

		public int SortieWins => this.RawData.api_st_win;
		public int SortieLoses => this.RawData.api_st_lose;

		public int ResourceLimit => (this.Level + 3) * 250;

		public double SortieWinningRate
		{
			get
			{
				var battleCount = this.RawData.api_st_win + this.RawData.api_st_lose;
				return battleCount == 0 ? 0
					: this.RawData.api_st_win / (double)battleCount;
			}
		}

		public int MaxShipCount => this.RawData.api_max_chara;
		public int MaxSlotItemCount => this.RawData.api_max_slotitem + 3;

		internal Admiral(kcsapi_basic api_data) : base(api_data)
		{
			this.Comment = this.RawData.api_comment;

			Proxy.Instance.Register(Proxy.api_req_member_updatecomment, e =>
			{
				var x = e.TryParse();
				if (x == null) return;

				this.Comment = x.Request["api_cmt"];
			});
		}

		public override string ToString()
			=> $"ID = {this.MemberId}, Nickname = \"{this.Nickname}\", Level = {this.Level}, Rank = \"{this.Rank}\"";
	}
}
