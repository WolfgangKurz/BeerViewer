using BeerViewer.Models.kcsapi;

namespace BeerViewer.Models.Raw
{
	public class SvDataBase
	{
		public int api_result { get; set; }
		public string api_result_msg { get; set; }
	}
	public class SvDataBase<T> : SvDataBase
	{
		public T api_data { get; set; }
		public kcsapi_deck[] api_data_deck { get; set; }
	}
}
