using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BeerViewer.Models.Raw
{
	/// <summary>
	/// 프로퍼티 변경통지를 도움
	/// </summary>
	public class Notifier : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public void PropertyEvent(string PropertyName, Action Handler, bool RaiseRegistered = false)
		{
			this.PropertyChanged += (_, e) =>
			{
				if (PropertyName == e.PropertyName)
					Handler?.Invoke();
			};

			if (RaiseRegistered)
				Handler?.Invoke();
		}
	}
}
