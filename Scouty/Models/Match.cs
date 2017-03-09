using System.Collections.Generic;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System.Linq;

namespace Scouty
{
	public class Match
	{
		[PrimaryKey]
		public string MatchId { get; set; }
		public string MatchInfo { get; set; }

		[Ignore]
		public string Name => MatchInfo.ToUpper();
		[Ignore]
		public string TeamDetail => Teams.Aggregate("", (x, y) => $"{x} {y.TeamNumber}");

		[ManyToMany(typeof(Performance))]
		public List<Team> Teams { get; set; }
		[ManyToOne]
		public Event Event { get; set; }
		[ForeignKey(typeof(Event))]
		public string EventId { get; set; }
		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public List<RobotEvent> RobotEvents { get; set; }
	}
}