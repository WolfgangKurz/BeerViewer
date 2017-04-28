using BeerViewer.Models.Raw;
using BeerViewer.Models.kcsapi;

namespace BeerViewer.Models.Wrapper
{
	public class Mission : SvData<kcsapi_mission>, IIdentifiable
	{
		public int Id => this.api_data.api_id;
		public string Title => this.api_data.api_name;
		public string Detail => this.api_data.api_details;

		internal Mission(kcsapi_mission mission) : base(mission) { }
	}
}
