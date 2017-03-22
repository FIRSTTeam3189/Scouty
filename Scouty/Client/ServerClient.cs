using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using BlueAllianceClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace Scouty.Client
{
	public class ServerClient
	{
		private static readonly Uri ServerUrl = new Uri("https://robot.servebeer.com/");
		private static readonly ServerClient _instance = new ServerClient();
		private static readonly string TokenName = "hentai.jwt";
		public static ServerClient Instance => _instance;
		private bool _init = false;

		public Token AccessToken { get; private set; }
		private HttpClient _client = new HttpClient();
		private Dictionary<string, List<TeamStat>> _cachedStats = new Dictionary<string, List<TeamStat>>();

		private ServerClient() {
			
		}

		/// <summary>
		/// Initializes the ServerClient Instance, loads in the access token and sees if it is still valid
		/// </summary>
		public void Initialize() {
			if (_init)
				return;
			_init = true;


			var fileHelper = DependencyService.Get<IFileHelper>();
			var tokenPath = fileHelper.GetLocalFilePath(TokenName);
			if (fileHelper.FileExists(tokenPath))
			{
				try
				{
					var rawToken = fileHelper.ReadFile(tokenPath);
					AccessToken = JsonConvert.DeserializeObject<Token>(rawToken);
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine($"Failed to load token {e.ToString()}");
					AccessToken = null;
				}
			}
		}

		/// <summary>
		/// Registers a new user with the Scouty Server
		/// </summary>
		/// <returns>If registration is successful.</returns>
		/// <param name="user">Username to register under.</param>
		/// <param name="password">Password to use.</param>
		/// <param name="realName">Real name.</param>
		public async Task<bool> Register(string user, string password, string realName) {
			var r = new CustomRegisterRequest()
			{
				Username = user,
				Password = password,
				RealName = realName
			};

			var request = GeneratePostRequest("api/Account/CustomRegister", r);

			var response = await _client.SendAsync(request);
			if (response.IsSuccessStatusCode)
				return true;
			else {
				System.Diagnostics.Debug.WriteLine($"Error: {response.StatusCode}, Reason: {response.ReasonPhrase}");
				return false;
			}
		}

		/// <summary>
		/// Logs out of the scouty server
		/// </summary>
		/// <returns>The logout.</returns>
		public async Task Logout() {
			// Don't logout when AccessToken is null
			if (AccessToken == null)
				return;
			var r = GeneratePostRequest("api/Account/Logout", new LoginRequest());
			var resp = await _client.SendAsync(r);
			if (!resp.IsSuccessStatusCode)
				System.Diagnostics.Debug.WriteLine("Failed to log out");

			AccessToken = null;
			DependencyService.Get<IFileHelper>().DeleteFile(DependencyService.Get<IFileHelper>().GetLocalFilePath(TokenName));
		}

		/// <summary>
		/// Logs into the scouty server with the user and password, then sets up access token
		/// </summary>
		/// <returns>If login was successful.</returns>
		/// <param name="user">User to login under.</param>
		/// <param name="password">Password to use.</param>
		public async Task<bool> Login(string user, string password) {
			var login = new LoginRequest { 
				Username = user,
				Password = password
			};

			var r = GeneratePostRequest("api/Account/CustomLogin", login);
			var response = await _client.SendAsync(r);
			JToken token;
			try
			{
				token = await GetJTokenFromResponse(response);
				if (token["access_token"] == null || token["expires_in"] == null || token["token_type"] == null)
					throw new HttpRequestException("Dumb retarded monkey baby");
			}
			catch {
				return false;
			}

			// Now lets Get the token
			var expiryDate = DateTime.Now.AddSeconds(token["expires_in"].Value<double>());
			var token_girl = token["access_token"].Value<string>();
			var tokenType = token["token_type"].Value<string>();
			var tokenBoychild = new Token { 
				Username = user,
				AccessToken = token_girl,
				ExpiresOn = expiryDate,
				TokenType = tokenType
			};
			AccessToken = tokenBoychild;

			// Now save onto disk
			var tokenPath = DependencyService.Get<IFileHelper>().GetLocalFilePath(TokenName);
			if (DependencyService.Get<IFileHelper>().FileExists(tokenPath))
				DependencyService.Get<IFileHelper>().DeleteFile(tokenPath);
			DependencyService.Get<IFileHelper>().WriteFile(tokenPath, JsonConvert.SerializeObject(AccessToken));

			return true;
		}

		public async Task<List<TeamStat>> GetTeamStats(string eventId, bool forceRefresh = false) {
			if (!_cachedStats.ContainsKey(eventId) || forceRefresh)
			{
				try
				{
					_cachedStats[eventId] = await PostAsync<StatRequest, List<TeamStat>>("api/Stat/Stats", new StatRequest { EventId = eventId });
				}
				catch (Exception e) {
					System.Diagnostics.Debug.WriteLine("Failed to get stats for " + eventId);
					return null;
				}
			}

			return _cachedStats[eventId];
		}

		/// <summary>
		/// Generates the post request.
		/// </summary>
		/// <returns>The post request.</returns>
		/// <param name="api">API to use.</param>
		/// <param name="body">Body of request.</param>
		private HttpRequestMessage GeneratePostRequest<T>(string api, T obj) where T : class
		{
			//Create the request body
			var content = new StringContent(JsonConvert.SerializeObject(obj),
											System.Text.Encoding.UTF8,
											"application/json");
			var request = new HttpRequestMessage()
			{
				RequestUri = new Uri(ServerUrl, api),
				Method = HttpMethod.Post,
				Content = content
			};
			if (AccessToken != null && DateTime.Now < AccessToken.ExpiresOn)
				request.Headers.Add("Authorization", $"Bearer " + AccessToken.AccessToken);

			return request;
		}

		/// <summary>
		/// Posts an API to the server asyncronously 
		/// </summary>
		/// <returns>The async.</returns>
		/// <param name="api">API.</param>
		/// <param name="obj">Object.</param>
		/// <typeparam name="TIn">The 1st type parameter.</typeparam>
		/// <typeparam name="TOut">The 2nd type parameter.</typeparam>
		public async Task<TOut> PostAsync<TIn, TOut>(string api, TIn obj) where TIn : class
																	where TOut : class
		{
			var request = GeneratePostRequest(api, obj);
			var resp = await _client.SendAsync(request);

			return await GetObjectFromResponse<TOut>(resp);
		}

		public async Task<bool> PostAsync<TIn>(string api, TIn obj) where TIn : class {
			var request = GeneratePostRequest(api, obj);
			try
			{
				var resp = await _client.SendAsync(request);
				if (!resp.IsSuccessStatusCode)
					System.Diagnostics.Debug.WriteLine($"Code: {resp.StatusCode} Reason: {resp.ReasonPhrase}");

				return resp.IsSuccessStatusCode;
			}
			catch (HttpRequestException e) {
				System.Diagnostics.Debug.WriteLine($"Error: {e.Message} \n\n{e.ToString()}");
				return false;
			}
		}

		/// <summary>
		/// Gets the object from response.
		/// </summary>
		/// <returns>The object from response.</returns>
		/// <param name="response">Response.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		private static async Task<JToken> GetJTokenFromResponse(HttpResponseMessage response)
		{
			if (response.IsSuccessStatusCode)
			{
				Stream stream;
				try
				{
					stream = await response.Content.ReadAsStreamAsync();
				}
				catch (Exception e)
				{
					throw new HttpRequestException("Failed to read response stream", e);
				}

				try
				{
					return stream.JTokenFromStream();
				}
				catch (Exception e)
				{
					throw new HttpRequestException("Failed to parse object", e);
				}
			}
			else
			{
				throw new HttpRequestException($"{response.StatusCode} " +
											   $": {response.ReasonPhrase} " +
											   $"\n {response.ToString()}");
			}
		}

		/// <summary>
		/// Gets the object from response.
		/// </summary>
		/// <returns>The object from response.</returns>
		/// <param name="response">Response.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		private static async Task<JArray> GetJArrayFromResponse(HttpResponseMessage response)
		{
			if (response.IsSuccessStatusCode)
			{
				Stream stream;
				try
				{
					stream = await response.Content.ReadAsStreamAsync();
				}
				catch (Exception e)
				{
					throw new HttpRequestException("Failed to read response stream", e);
				}

				try
				{
					return stream.JArrayFromStream();
				}
				catch (Exception e)
				{
					throw new HttpRequestException("Failed to parse object", e);
				}
			}
			else
			{
				throw new HttpRequestException($"{response.StatusCode} " +
											   $": {response.ReasonPhrase} " +
											   $"\n {response.ToString()}");
			}
		}

		/// <summary>
		/// Gets the object from response.
		/// </summary>
		/// <returns>The object from response.</returns>
		/// <param name="response">Response.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		private static async Task<T> GetObjectFromResponse<T>(HttpResponseMessage response)
		{
			if (response.IsSuccessStatusCode)
			{
				Stream stream;
				try
				{
					stream = await response.Content.ReadAsStreamAsync();
				}
				catch (Exception e)
				{
					throw new HttpRequestException("Failed to read response stream", e);
				}

				try
				{
					return stream.FromStream<T>();
				}
				catch (Exception e)
				{
					throw new HttpRequestException("Failed to parse object", e);
				}
			}
			else
			{
				throw new HttpRequestException($"{response.StatusCode} " +
											   $": {response.ReasonPhrase} " +
											   $"\n {response.ToString()}");
			}
		}

	}

	class CustomRegisterRequest { 
		public string Username { get; set; }
		public string Password { get; set; }
		public string RealName { get; set; }
	}

	class LoginRequest { 
		public string Username { get; set; }
		public string Password { get; set; }
	}

	class StatRequest { 
		public string EventId { get; set; }
	}
}
