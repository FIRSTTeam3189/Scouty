using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace BlueAllianceClient
{
	public class BAEvent
	{
		[JsonProperty("key")]
		public string Key { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("short_name")]
		public string ShortName { get; set; }

		[JsonProperty("event_code")]
		public string EventCode { get; set; }

		[JsonProperty("event_type_string")]
		public string EventType { get; set; }

		[JsonProperty("event_type")]
		public int EventTypeCode { get; set; }

		[JsonProperty("event_district_string")]
		public string EventDistrict { get; set; }

		[JsonProperty("event_district")]
		public int EventDistrictCode { get; set; }

		[JsonProperty("year")]
		public int Year { get; set; }

		[JsonProperty("week")]
		public int? Week { get; set; }

		[JsonProperty("location")]
		public string Location { get; set; }

		[JsonProperty("venue_address")]
		public string VenueAddress { get; set; }

		[JsonProperty("timezone")]
		public string TimeZone { get; set; }

		[JsonProperty("website")]
		public string Website { get; set; }

		[JsonProperty("official")]
		public bool? Official { get; set; }

		[JsonProperty("teams")]
		public List<BATeam> Teams { get; set; }

		[JsonIgnore]
		public List<BAMatch> Matches { get; set; }
		public override string ToString()
		{
			var str = new StringBuilder();
			if (Teams != null)
			{
				str.Append("\nTeams {\n");
				foreach (var team in Teams)
				{
					str.Append($"\t{team.ToString()}\n");
				}
				str.Append("}\n");
			}
			if (Matches != null)
			{
				str.Append("Matches {\n");
				foreach (var match in Matches)
				{
					str.Append($"\t{{[{match.Key}, {match.Level}, {match.MatchNumber}, {match.SetNumber}]\n");
					str.Append("\tRed: {\n");
					foreach (var team in match.Red)
					{
						str.Append($"\t\t{team.ToString()}\n");
					}
					str.Append("\tBlue: {\n");
					foreach (var team in match.Blue)
					{
						str.Append($"\t\t{team.ToString()}\n");
					}
					str.Append("\t}\n}");
				}
			}
			return string.Format("[Event: Key={0}, Name={1}, ShortName={2}, EventCode={3}, EventType={4}, EventTypeCode={5}, EventDistrict={6}, EventDistrictCode={7}, Year={8}, Week={9}, Location={10}, VenueAddress={11}, TimeZone={12}, Website={13}, Official={14}] {15}", Key, Name, ShortName, EventCode, EventType, EventTypeCode, EventDistrict, EventDistrictCode, Year, Week, Location, VenueAddress, TimeZone, Website, Official, str);
		}
	}
}
