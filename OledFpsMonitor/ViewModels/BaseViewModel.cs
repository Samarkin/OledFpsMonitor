using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Samarkin.Oled.FpsMonitor.ViewModels
{
	public abstract class BaseViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		protected void SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
		{
			if (EqualityComparer<T>.Default.Equals(field, value))
			{
				return;
			}
			field = value;
			NotifyPropertyChanged(propertyName);
		}

		protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
