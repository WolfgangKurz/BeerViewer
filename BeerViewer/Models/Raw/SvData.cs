using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization.Json;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Web;
using Nekoxy;
using BeerViewer.Core;

namespace BeerViewer.Models.Raw
{
	/// <summary>
	/// API 공통적 요소를 담은 베이스 클래스
	/// </summary>
	public class SvDataBase
	{
		public int api_result { get; set; }
		public string api_result_msg { get; set; }
	}
	/// <summary>
	/// 베이스 클래스 + 데이터 클래스
	/// </summary>
	public class SvDataBase<T> : SvDataBase
	{
		public T api_data { get; set; }
		public kcsapi_deck[] api_data_deck { get; set; }
	}

	public class SvData : RawDataWrapper<SvDataBase>
	{
		public NameValueCollection Request { get; private set; }

		public bool IsSuccess => this.RawData.api_result == 1;

		public SvData(SvDataBase RawData, string RequestURI) : base(RawData)
		{
			this.Request = HttpUtility.ParseQueryString(RequestURI);
		}

		#region
		public static string JsonParse(string ResponseBody)
		{
			return ResponseBody.StartsWith("svdata=")
				? ResponseBody.Substring(7)
				: null;
		}
		#endregion

		#region 제네릭 파싱 함수
		public static SvData<T> Parse<T>(Session session)
		{
			var json = SvData.JsonParse(session.Response.BodyAsString);
			if (json == null) return null;

			var bytes = Encoding.UTF8.GetBytes(json);
			var serializer = new DataContractJsonSerializer(typeof(SvDataBase<T>));
			using (var stream = new MemoryStream(bytes))
			{
				var rawResult = serializer.ReadObject(stream) as SvDataBase<T>;
				var result = new SvData<T>(rawResult, session.Request.BodyAsString);
				return result;
			}
		}
		public static bool TryParse<T>(Session session, out SvData<T> result)
		{
			try
			{
				result = Parse<T>(session);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				result = null;
			}
			return result != null;
		}
		#endregion

		#region 비제네릭 파싱 함수
		public static SvData Parse(Session session)
		{
			var json = SvData.JsonParse(session.Response.BodyAsString);
			if (json == null) return null;

			var bytes = Encoding.UTF8.GetBytes(json);
			var serializer = new DataContractJsonSerializer(typeof(SvDataBase));
			using (var stream = new MemoryStream(bytes))
			{
				var rawResult = serializer.ReadObject(stream) as SvDataBase;
				var result = new SvData(rawResult, session.Request.BodyAsString);
				return result;
			}
		}

		public static bool TryParse(Session session, out SvData result)
		{
			try
			{
				result = Parse(session);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				result = null;
			}
			return result != null;
		}

		#endregion
	}
	public class SvData<T> : RawDataWrapper<SvDataBase<T>>
	{
		public NameValueCollection Request { get; private set; }

		public bool IsSuccess => this.RawData.api_result == 1;
		public T Data => this.RawData.api_data;
		public kcsapi_deck[] Fleets => this.RawData.api_data_deck;

		public SvData(SvDataBase<T> RawData, string RequestBody) : base(RawData)
		{
			this.Request = HttpUtility.ParseQueryString(RequestBody);
		}
	}
}
