using System;
using System.Linq;
using System.Collections.Generic;
using BlueAllianceClient;

namespace Scouty
{
	public static class BlueAllianceExtensions
	{
		public static Event FromBAEvent(this BAEvent e) {
			return new Event
			{
				EventId = e.Key,
				Location = e.Location,
				Week = e.Week != null ? (int)e.Week + 1: 0,
				Name = e.ShortName,
				Matches = new List<Match>(),
				Teams = new List<Team>()
			};
		}

		public static Match FromBAMatch(this BAMatch match, Event ev) {
			return new Match
			{
				MatchId = match.Key,
				MatchInfo = match.MatchInfo(),
				Event = ev,
				EventId = ev.EventId,
				RobotEvents = new List<RobotEvent>(),
				Teams = new List<Team>()
			};
		}

		public static string MatchInfo(this BAMatch match) {
			return match.Level == "qm"
							? $"{match.Level} {match.MatchNumber}"
							: $"{match.Level} {match.SetNumber} m {match.MatchNumber}";
		}

		public static Team FromBATeam(this BATeam team) {
			return new Team { 
				Name = string.IsNullOrWhiteSpace(team.Nickname) ? team.Name : team.Nickname,
				TeamNumber = team.TeamNumber,
				Events = new List<Event>(),
				Matches = new List<Match>(),
				RobotEvents = new List<RobotEvent>(),
				RookieYear = team.RookieYear ?? 1970
			};
		}

		public static Performance PerformanceFromMatch(this Match match, Team team, AllianceColor color) {
			return new Performance
			{
				Color = color,
				Match = match,
				MatchId = match.MatchId,
				Team = team,
				TeamNumber = team.TeamNumber
			};
		}
	}
}
