using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SQLiteNetExtensions.Attributes;

namespace Scouty
{
	public class Performance
	{
		private static ActionPeriod a = ActionPeriod.Auto;
		private static ActionPeriod t = ActionPeriod.Teleop;

		[ForeignKey(typeof(Match))]
		public string MatchId { get; set; }
		[ForeignKey(typeof(Team))]
		public int TeamNumber { get; set; }

		[ManyToOne]
		[JsonIgnore]
		public Team Team { get; set; }
		[ManyToOne]
		[JsonIgnore]
		public Match Match { get; set; }

		public AllianceColor Color { get; set; }
	}

	public enum AllianceColor { 
		Red,
		Blue
	}
}