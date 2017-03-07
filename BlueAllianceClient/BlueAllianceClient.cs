using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace BlueAllianceClient
{
	public class BlueAllianceClient
	{
		private readonly HttpClient _client;
		private static readonly Uri BlueAllianceUrl = new Uri("https://www.thebluealliance.com");
		private static readonly string IdHeader = "X-TBA-App-Id";
		private static readonly int Version = 3;
		private static readonly string IfLastModifiedHeader = "If-Modified-Since";
		private static readonly string LastModifiedHeader = "Modified-Since";
		private readonly int TeamNumber;
		public BlueAllianceClient(int teamNumber = 3189, HttpMessageHandler handler = null) {
			if (handler == null)
				_client = new HttpClient();
			else
				_client = new HttpClient(handler);

			TeamNumber = teamNumber;
		}

		/// <summary>
		/// Gets objects async from blue alliance api
		/// </summary>
		/// <returns>The async.</returns>
		/// <param name="api">API.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public async Task<T> GetAsync<T>(string api) {
			var request = GenerateGetRequest(api);

			return await GetObjectFromResponse<T>(await _client.SendAsync(request));
		}

		public async Task<JArray> GetJArrayAsync(string api) {
			var request = GenerateGetRequest(api);

			return await GetJArrayFromResponse(await _client.SendAsync(request));
		}


		public async Task<JToken> GetJTokenAsync(string api)
		{
			var request = GenerateGetRequest(api);

			return await GetJTokenFromResponse(await _client.SendAsync(request));
		}

		/// <summary>
		/// Sends the post async to blue alliance.
		/// </summary>
		/// <returns>The post async.</returns>
		/// <param name="apiPath">API path.</param>
		/// <param name="obj">Object.</param>
		/// <typeparam name="TIn">The 1st type parameter.</typeparam>
		/// <typeparam name="TOut">The 2nd type parameter.</typeparam>
		public async Task<TOut> PostAsync<TIn, TOut>(string apiPath, TIn obj)
		{
			var request = GeneratePostRequest(apiPath, JsonConvert.SerializeObject(obj));

			// Make the request
			return await GetObjectFromResponse<TOut>(await _client.SendAsync(request));
		}

		/// <summary>
		/// Generates the post request.
		/// </summary>
		/// <returns>The post request.</returns>
		/// <param name="api">API to use.</param>
		/// <param name="body">Body of request.</param>
		private HttpRequestMessage GeneratePostRequest(string api, string body)
		{
			// Create the request body
			var content = new StringContent(body,
											System.Text.Encoding.UTF8,
											"application/json");
			var request = new HttpRequestMessage()
			{
				RequestUri = new Uri(BlueAllianceUrl, api),
				Method = HttpMethod.Post,
				Content = content
			};
			request.Headers.Add(IdHeader, $"{TeamNumber}:3189_Scout_System:{Version}");

			return request;
		}

		private HttpRequestMessage GenerateGetRequest(string api)
		{
			// Create request
			var request = new HttpRequestMessage()
			{
				RequestUri = new Uri(BlueAllianceUrl, api),
				Method = HttpMethod.Get
			};

			request.Headers.Add(IdHeader, $"{TeamNumber}:3189_Scout_System:{Version}");
			return request;
		}


		/// <summary>
		/// Gets the object from response.
		/// </summary>
		/// <returns>The object from response.</returns>
		/// <param name="response">Response.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		private static async Task<T> GetObjectFromResponse<T>(HttpResponseMessage response) { 
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
	}
}
