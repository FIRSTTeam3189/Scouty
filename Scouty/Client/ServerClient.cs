using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using BlueAllianceClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Scouty.Client
{
	public class ServerClient
	{
		private static readonly Uri ServerUrl = new Uri("https://robot.servebeer.com/");
		private static readonly ServerClient _instance = new ServerClient();
		public static ServerClient Instance => _instance;

		private HttpClient _client = new HttpClient();
		private ServerClient() { }

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

		public async Task<bool> Login(string user, string password) {
			var login = new LoginRequest { 
				Username = user,
				Password = password
			};

			var r = GeneratePostRequest("api/Account/Login", login);
			var response = await _client.SendAsync(r);
			JToken t;
			try
			{
				t = await GetJTokenFromResponse(response);
			}
			catch {
				return false;
			}

			return true;
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

			return request;
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
}
