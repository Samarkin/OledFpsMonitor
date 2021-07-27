using Samarkin.Oled.FpsMonitor.ViewModels;
using System.Windows;

namespace OledFpsMonitor.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void WindowClosed(object sender, System.EventArgs e)
		{
			(DataContext as MainWindowViewModel)?.HandleWindowClose();
		}
	}
}
