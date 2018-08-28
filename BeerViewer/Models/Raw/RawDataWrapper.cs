using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models.Raw
{
	public class RawDataWrapper<T> : Notifier
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public T RawData { get; private set; }

		protected RawDataWrapper(T RawData)
		{
			this.UpdateData(RawData);
		}

		protected void UpdateData(T RawData)
		{
			this.RawData = RawData;
		}
	}
}
