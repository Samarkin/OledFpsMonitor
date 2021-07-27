using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Samarkin.Oled
{
	public sealed class OledClient : IDisposable
	{
		private const string SessionTokenHeaderName = "X-Session-Token";
		private readonly HttpClient _httpClient = new();
		private readonly Uri _rootUri;

		public OledClient(string address)
		{
			if (address == null)
			{
				throw new ArgumentNullException(nameof(address));
			}
			if (!address.EndsWith('/'))
			{
				address += '/';
			}
			_rootUri = new Uri(new UriBuilder(address).Uri, "api/");
		}

		public async Task Login(string username, string password)
		{
			var body = new
			{
				login = username,
				password = password,
			};
			_httpClient.DefaultRequestHeaders.Remove(SessionTokenHeaderName);
			UserName = null;
			var response = await _httpClient.PostAsJsonAsync(new Uri(_rootUri, "login"), body);
			if (response.IsSuccessStatusCode && response.Headers.TryGetValues(SessionTokenHeaderName, out var tokens))
			{
				UserName = username;
				_httpClient.DefaultRequestHeaders.Add(SessionTokenHeaderName, tokens.FirstOrDefault());
			}
			else if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				throw new NotAuthenticatedException();
			}
			else
			{
				throw new OledException($"OLED call failed with status code {response.StatusCode}");
			}
		}

		public async Task DisplayMessage(int line, string message, int? duration = null)
		{
			var body = new
			{
				text = message,
				duration = duration,
			};
			var response = await _httpClient.PutAsJsonAsync(new Uri(_rootUri, $"messages/{line}"), body);
			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				throw new NotAuthenticatedException();
			}
			else if (!response.IsSuccessStatusCode)
			{
				throw new OledException($"OLED call failed with status code {response.StatusCode}");
			}
		}

		public void Dispose()
		{
			UserName = null;
			_httpClient.Dispose();
		}

		public Uri RootUri { get { return _rootUri; } }
		public string? UserName { get; private set; }
	}
}
