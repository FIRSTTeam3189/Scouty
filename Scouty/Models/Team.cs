using System.Collections.Generic;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace Scouty
{
	public class Team
	{
		[PrimaryKey]
		public int TeamNumber { get; set; }
		public string Name { get; set; }
		public int RookieYear { get; set; }

		[ManyToMany(typeof(TeamEvent))]
		public List<Event> Events { get; set; }
		[ManyToMany(typeof(Performance))]
		public List<Match> Matches { get; set; }
		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public List<RobotEvent> RobotEvents { get; set; }
		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public List<Note> Notes { get; set; }
		[OneToOne]
		public DataSheet DataSheet { get; set; }
	}
}