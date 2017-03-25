using System;
using System.Linq;
using System.Collections.Generic;

namespace Scouty.Stats
{
	public class StatReport
	{
		public IReadOnlyList<TeamStatReport> Rankings { get; set; }
		public StatType Type { get; set; }
		public double EventAverage { get; set; }
		public double EventStdDeviation { get; set; }

		public StatReport() { }

		public StatReport(List<TeamStat> stats, StatType statType) {
			EventAverage = stats.GetAverage(statType);
			EventStdDeviation = stats.GetStdDeviation(statType);
			Type = statType;

			// Grab all of the team averages for the stat
			var avgs = stats
				.Select(x => new { TeamNumber = x.TeamNumber, Avg = x.GetTeamAverage(statType) }).ToList();
			var zScores = stats
				.Select(x => new { TeamNumber = x.TeamNumber, ZScore = x.GetZScore(Type, EventAverage, EventStdDeviation) })
				.ToDictionary(x => x.TeamNumber, x => x.ZScore);

			var ranks = avgs
				.OrderByDescending(x => x.Avg)
				.Select((x, i) => new TeamStatReport(statType, x.Avg, x.TeamNumber, zScores[x.TeamNumber], i + 1))
				.ToList();
			Rankings = ranks;
		}
	}
}
