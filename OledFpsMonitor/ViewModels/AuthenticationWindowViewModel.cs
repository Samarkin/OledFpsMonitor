namespace Samarkin.Oled.FpsMonitor.ViewModels
{
	public class AuthenticationWindowViewModel : BaseViewModel
	{
		public AuthenticationWindowViewModel()
		{
			var settings = Settings.Default;
			ServerAddress = settings.ServerAddress;
			Login = settings.Login;
			Password = settings.Password;
		}

		private string _login = "";
		public string Login
		{
			get => _login;
			set => SetValue(ref _login, value);
		}

		private string _password = "";
		public string Password
		{
			get => _password;
			set => SetValue(ref _password, value);
		}

		private string _serverAddress = "";
		public string ServerAddress
		{
			get => _serverAddress;
			set => SetValue(ref _serverAddress, value);
		}

		public void Save()
		{
			var settings = Settings.Default;
			settings.Login = Login;
			settings.Password = Password;
			settings.ServerAddress = ServerAddress;
			settings.Save();
		}
	}
}
