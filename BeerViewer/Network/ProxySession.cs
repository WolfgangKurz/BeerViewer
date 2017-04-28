using Nekoxy;

using BeerViewer.Models.Raw;

namespace BeerViewer.Network
{
	public static class ProxySession
	{
		public static SvData TryParse(this Session x, bool IsSuccess = true)
		{
			SvData result;
			return SvData.TryParse(x, out result)
				? ((!IsSuccess || result.IsSuccess) ? result : null)
				: null;
		}
		public static SvData<T> TryParse<T>(this Session x, bool IsSuccess = true)
		{
			SvData<T> result;
			return SvData.TryParse(x, out result)
				? ((!IsSuccess || result.IsSuccess) ? result : null)
				: null;
		}
	}
}
