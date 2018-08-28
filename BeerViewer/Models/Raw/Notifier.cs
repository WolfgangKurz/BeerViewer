using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BeerViewer.Models.Raw
{
	public partial class Notifier : INotifyPropertyChanged
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
