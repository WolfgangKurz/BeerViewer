using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Models.Raw;

namespace BeerViewer.Models
{
	public class Mission : RawDataWrapper<kcsapi_mission>, IIdentifiable
	{
		public int Id { get; }

		public string Title { get; }

		public string Detail { get; }

		public Mission(kcsapi_mission mission) : base(mission)
		{
			this.Id = mission.api_id;
			this.Title = mission.api_name;
			this.Detail = mission.api_details;
		}
	}
}
