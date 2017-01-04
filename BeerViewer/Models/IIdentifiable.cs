using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models
{
	/// <summary>
	/// 요소를 고유 식별할 수 있도록
	/// </summary>
	public interface IIdentifiable
	{
		int Id { get; }
	}
}
