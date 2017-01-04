using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models.Raw
{
	/// <summary>
	/// 칸코레 API Response의 api_data 데이터 랩퍼
	/// </summary>
	/// <typeparam name="T">랩핑할 api_data 형식</typeparam>
	public abstract class RawDataWrapper<T> : Notifier
	{
		/// <summary>
		/// api_data 데이터
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public T RawData { get; private set; }

		/// <summary>
		/// 랩핑할 api_data를 사용해서 <see cref="RawDataWrapper{T}"/> 클래스의 새 인스턴스를 초기화
		/// </summary>
		/// <param name="RawData">랩핑할 api_data 데이터</param>
		protected RawDataWrapper(T RawData)
		{
			this.UpdateRawData(RawData);
		}

		/// <summary>
		/// 랩핑할 api_data를 사용해서 <see cref="RawDataWrapper{T}"/> 클래스 인스턴스를 갱신
		/// </summary>
		/// <param name="RawData">랩핑할 api_data 데이터</param>
		protected void UpdateRawData(T RawData)
		{
			this.RawData = RawData;
		}
	}
}
