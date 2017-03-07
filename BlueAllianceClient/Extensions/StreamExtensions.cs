using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueAllianceClient
{
	public static class StreamExtensions
	{
		public static T FromStream<T>(this Stream stream, Encoding encoding) {
			var serializer = new JsonSerializer();

			using (var streamReader = new StreamReader(stream, encoding))
			using (var reader = new JsonTextReader(streamReader))
				return serializer.Deserialize<T>(reader);
		}

		public static T FromStream<T>(this Stream stream) {
			return FromStream<T>(stream, Encoding.UTF8);
		}

		public static JToken JTokenFromStream(this Stream stream) {
			using (var streamReader = new StreamReader(stream, Encoding.UTF8))
			using (var reader = new JsonTextReader(streamReader))
				return JToken.Load(reader);
		}

		public static JArray JArrayFromStream(this Stream stream)
		{
			using (var streamReader = new StreamReader(stream, Encoding.UTF8))
			using (var reader = new JsonTextReader(streamReader))
				return JArray.Load(reader);
		}
	}
}
