using BeerViewer.Models.Raw;
using BeerViewer.Models.kcsapi;

namespace BeerViewer.Models.Wrapper
{
	public class Mission : RawDataWrapper<kcsapi_mission>, IIdentifiable
	{
		public int Id => this.RawData.api_id;
		public string Title => this.RawData.api_name;
		public string Detail => this.RawData.api_details;

		internal Mission(kcsapi_mission mission) : base(mission) { }
	}
}
