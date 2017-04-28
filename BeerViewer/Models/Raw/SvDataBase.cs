namespace BeerViewer.Models.Raw
{
	public class SvDataBase
	{
		public int api_result { get; set; }
		public string api_result_msg { get; set; }

		public SvDataBase() { }
		public SvDataBase(SvDataBase RawData)
		{
			this.api_result = RawData.api_result;
			this.api_result_msg = RawData.api_result_msg;
		}
	}
	public class SvDataBase<T> : SvDataBase
	{
		public T api_data { get; set; }

		public SvDataBase(T RawData) : base()
		{
			this.api_data = RawData;
		}
		public SvDataBase(SvDataBase<T> RawData) : base(RawData)
		{
			this.api_data = RawData.api_data;
		}
	}
}
