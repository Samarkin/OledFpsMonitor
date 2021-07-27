using System.Windows;

namespace Samarkin.Oled.FpsMonitor.Views
{
	/// <summary>
	/// Interaction logic for AuthenticationWindow.xaml
	/// </summary>
	public partial class AuthenticationWindow : Window
	{
		public AuthenticationWindow()
		{
			InitializeComponent();
		}

		private void OkClicked(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}

		private void CancelClicked(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
