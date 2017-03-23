using System;
using System.Collections.Generic;
using System.Linq;

namespace Scouty
{
	public class TeamStat
	{
		public string EventId { get; set; }
		public int TeamNumber { get; set; }
		public List<TeamMatchStat> MatchStats { get; set; }
		public TeamStat()
		{
		}
	}

	public class TeamMatchStat
	{
		public string MatchId { get; set; }
		public List<int> Teams { get; set; }
		public List<RobotEvent> Events { get; set; }
		public TeamMatchStat() { }
		public TeamMatchStat(string matchId, IEnumerable<int> teams, IEnumerable<RobotEvent> robotEvents)
		{
			MatchId = matchId;
			Teams = teams.ToList();
			Events = robotEvents.ToList();
		}
	}
}
