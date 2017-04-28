using BeerViewer.Network;
using BeerViewer.Models;
using BeerViewer.Models.Enums;
using BeerViewer.Models.Raw;
using BeerViewer.Models.kcsapi;

namespace BeerViewer.Models.Wrapper
{
	public class Admiral : SvData<kcsapi_basic>
	{
		public string MemberId => this.api_data.api_member_id;
		public string Nickname => this.api_data.api_nickname;

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

		public int Experience => this.api_data.api_experience;
		public int ExperienceForNexeLevel => Models.Experience.GetAdmiralExpForNextLevel(this.api_data.api_level, this.api_data.api_experience);

		public int Level => this.api_data.api_level;
		public AdmiralRank Rank => (AdmiralRank)this.api_data.api_rank;

		public int SortieWins => this.api_data.api_st_win;
		public int SortieLoses => this.api_data.api_st_lose;

		public double SortieWinningRate
		{
			get
			{
				var battleCount = this.api_data.api_st_win + this.api_data.api_st_lose;
				return battleCount == 0 ? 0
					: this.api_data.api_st_win / (double)battleCount;
			}
		}

		public int MaxShipCount => this.api_data.api_max_chara;
		public int MaxSlotItemCount => this.api_data.api_max_slotitem + 3;

		internal Admiral(kcsapi_basic api_data) : base(api_data)
		{
			this.Comment = this.api_data.api_comment;

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
