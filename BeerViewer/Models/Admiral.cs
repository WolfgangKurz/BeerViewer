using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Models.Raw;

namespace BeerViewer.Models
{
	/// <summary>
	/// 제독 데이터
	/// </summary>
	public class Admiral : RawDataWrapper<kcsapi_basic>
	{
		public string MemberId => this.RawData.api_member_id;
		public string Nickname => this.RawData.api_nickname;

		#region Comment 프로퍼티
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

		/// <summary>
		/// 제독 경험치
		/// </summary>
		public int Experience => this.RawData.api_experience;

		/// <summary>
		/// 다음 레벨까지 남은 경험치
		/// </summary>
		public int ExperienceForNexeLevel => Models.Experience.GetAdmiralExpForNextLevel(this.RawData.api_level, this.RawData.api_experience);

		/// <summary>
		/// 사령부 레벨
		/// </summary>
		public int Level => this.RawData.api_level;

		/// <summary>
		/// 제독 랭크 (원사, 대위, 중위 ...)
		/// </summary>
		public string Rank => Models.Rank.GetName(this.RawData.api_rank);

		/// <summary>
		/// 출격 승리 수
		/// </summary>
		public int SortieWins => this.RawData.api_st_win;

		/// <summary>
		/// 출격 패배 수
		/// </summary>
		public int SortieLoses => this.RawData.api_st_lose;

		/// <summary>
		/// 출격 승률
		/// </summary>
		public double SortieWinningRate
		{
			get
			{
				var battleCount = this.RawData.api_st_win + this.RawData.api_st_lose;
				return battleCount == 0 ? 0
					: this.RawData.api_st_win / (double)battleCount;
			}
		}

		/// <summary>
		/// 소지 가능 칸무스 수
		/// </summary>
		public int MaxShipCount => this.RawData.api_max_chara;

		/// <summary>
		/// 소지 가능 장비 수
		/// </summary>
		public int MaxSlotItemCount => this.RawData.api_max_slotitem + 3;

		internal Admiral(kcsapi_basic RawData) : base(RawData)
		{
			this.Comment = this.RawData.api_comment;
		}

		public override string ToString()
		{
			return $"ID = {this.MemberId}, Nickname = \"{this.Nickname}\", Level = {this.Level}, Rank = \"{this.Rank}\"";
		}
	}
}
