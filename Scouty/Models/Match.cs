using System.Collections.Generic;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace Scouty
{
	public class Match
	{
		[PrimaryKey]
		public string MatchId { get; set; }
		public string MatchInfo { get; set; }

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