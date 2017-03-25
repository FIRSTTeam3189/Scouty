using System;
using Newtonsoft.Json;

namespace Scouty.Stats
{
	public class TeamStatReport
	{
		public StatType StatType { get; set; }
		public double Value { get; set; }
		public double ZScore { get; set; }
		public int TeamNumber { get; set; }
		public int Rank { get; set; }

		public TeamStatReport() { }

		public TeamStatReport(StatType statType, double value, int teamNumber, double zScore, int rank) {
			StatType = statType;
			Value = value;
			TeamNumber = teamNumber;
			ZScore = zScore;
			Rank = rank;
		}
	}
}
