using System;
using Newtonsoft.Json;

namespace BlueAllianceClient
{
	public class BATeam
	{
		[JsonProperty("website")]
		public string Website { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("locality")]
		public string Locality { get; set; }

		[JsonProperty("region")]
		public string Region { get; set; }

		[JsonProperty("country_name")]
		public string Country { get; set; }

		[JsonProperty("location")]
		public string Location { get; set; }

		[JsonProperty("team_number")]
		public int TeamNumber { get; set; }

		[JsonProperty("key")]
		public string Key { get; set; }

		[JsonProperty("nickname")]
		public string Nickname { get; set; }

		[JsonProperty("rookie_year")]
		public int? RookieYear { get; set; }

		[JsonProperty("motto")]
		public string Motto { get; set; }

		public override string ToString()
		{
			return string.Format("[Team: Website={0}, Name={1}, Locality={2}, Region={3}, Country={4}, Location={5}, TeamNumber={6}, Key={7}, Nickname={8}, RookieYear={9}, Motto={10}]", Website, Name, Locality, Region, Country, Location, TeamNumber, Key, Nickname, RookieYear, Motto);
		}
	}
}
