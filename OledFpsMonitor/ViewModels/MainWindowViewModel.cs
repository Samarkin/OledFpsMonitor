using Samarkin.Oled.FpsMonitor.Views;
using Samarkin.SystemMonitor;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Samarkin.Oled.FpsMonitor.ViewModels
{
	public class MainWindowViewModel : BaseViewModel
	{
		public ICommand MainButtonCommand { get; }

		private OledClient? _oledClient;
		private FpsCounter _fpsCounter;
		private Timer _timer;
		public string? Login => _oledClient?.UserName;

		private string _text;
		public string Text
		{
			get => _text;
			set => SetValue(ref _text, value);
		}

		public MainWindowViewModel()
		{
			MainButtonCommand = new RelayCommand(ExecuteDoStuff);
			_text = "Hello, world!";
			_fpsCounter = new FpsCounter();
			_fpsCounter.Start();
			_timer = new Timer(UpdateOled);
			_ = _timer.Change(0, 100); // 10 times per second
		}

		private async void UpdateOled(object? state)
		{
			if (_oledClient == null)
			{
				// Not connected, can't display anything
				return;
			}
			int processId = ProcessUtils.GetForegroundProcessId();
			int? fps = _fpsCounter.GetFpsForProcess(processId);
			string processName = _fpsCounter.GetProcessName(processId);
			try
			{
				await _oledClient.DisplayMessage(0, processName, 1);
				if (fps.HasValue)
				{
					await _oledClient.DisplayMessage(2, $"{fps} FPS", 1);
				}
				else
				{
					await _oledClient.DisplayMessage(2, "");
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		private async Task Connect()
		{
			// TODO: Do it the MVVM way
			AuthenticationWindow? window = new();
			if (window.ShowDialog() == true && window.DataContext is AuthenticationWindowViewModel authVM)
			{
				OledClient oledClient = new(authVM.ServerAddress);
				await oledClient.Login(authVM.Login, authVM.Password);
				_oledClient = oledClient;
				NotifyPropertyChanged(nameof(Login));
				authVM.Save();
			}
		}

		private async void ExecuteDoStuff()
		{
			if (_oledClient == null)
			{
				await Connect();
				if (_oledClient == null)
				{
					return;
				}
			}
			await _oledClient.DisplayMessage(3, Text);
		}

		public void HandleWindowClose()
		{
			_timer.Dispose();
			_fpsCounter.Dispose();
			_oledClient?.Dispose();
		}
	}
}
