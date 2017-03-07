using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueAllianceClient
{
	public class BlueAllianceContext
	{
		public BlueAllianceClient Connection { get; }
		private List<BAEvent> _cachedEvents;

		public BlueAllianceContext(int teamNumber = 3189, HttpMessageHandler handler = null)
		{
			Connection = new BlueAllianceClient(teamNumber, handler);
		}

		public async Task<List<BAEvent>> GetEvents(int year) {
			if (_cachedEvents == null)
				_cachedEvents = await Connection.GetAsync<List<BAEvent>>($"/api/v2/events/{year}");

			return _cachedEvents;
		}

		public async Task<BAEvent> GetEvent(int year, string eventCode) {
			var ev = await Connection.GetAsync<BAEvent>($"/api/v2/event/{year}{eventCode}");

			// Now pull down all of the matches as JToken, and Teams too
			var rawMatches = await Connection.GetJArrayAsync($"/api/v2/event/{year}{eventCode}/matches");
			var teams = await Connection.GetAsync<List<BATeam>>($"/api/v2/event/{year}{eventCode}/teams");

			// Select all that we want from Matches
			var matches = rawMatches.Select(x => {
				var redTeam = x["alliances"]["red"]["teams"]
					.Select(t => teams
					        .First(tm => tm.Key.Equals((string)t, StringComparison.CurrentCultureIgnoreCase)));
				var blueTeam = x["alliances"]["blue"]["teams"]
					.Select(t => teams
							.First(tm => tm.Key.Equals((string)t, StringComparison.CurrentCultureIgnoreCase)));
				
				return new BAMatch {
					Blue = blueTeam.ToList(),
					Red = redTeam.ToList(),
					Key = (string)x["key"],
					Event = ev,
					Level = (string)x["comp_level"],
					MatchNumber = (int)x["match_number"],
					SetNumber = x["set_number"]?.Value<int?>()
				};
			});

			ev.Teams = teams;
			ev.Matches = matches.ToList();

			return ev;
		}
	}
}
