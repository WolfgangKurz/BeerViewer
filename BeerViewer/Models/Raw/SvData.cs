using System;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Web;
using Nekoxy;

namespace BeerViewer.Models.Raw
{
	public class SvData : SvDataBase
	{
		public NameValueCollection Request { get; private set; }
		public bool IsSuccess => this.api_result == 1;

		private static string JsonParse(string ResponseBody)
			=> ResponseBody.StartsWith("svdata=")
				? ResponseBody.Substring(7)
				: null;

		public SvData(SvDataBase RawData) : base(RawData)
		{
			this.Request = null;
		}
		public SvData(SvDataBase RawData, string RequestURI) : base(RawData)
		{
			this.Request = HttpUtility.ParseQueryString(RequestURI);
		}

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
	}
	public class SvData<T> : SvDataBase<T>
	{
		public NameValueCollection Request { get; private set; }
		public bool IsSuccess => this.api_result == 1;

		public SvData(T RawData) : base(RawData)
		{
			this.Request = null;
		}
		public SvData(SvDataBase<T> RawData) : base(RawData)
		{
			this.Request = null;
		}
		public SvData(SvDataBase<T> RawData, string RequestBody) : base(RawData)
		{
			this.Request = HttpUtility.ParseQueryString(RequestBody);
		}
	}
}
