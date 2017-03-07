using System;
using System.Collections.Generic;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace Scouty
{
	public class Event
	{
		public string Name { get; set; }
		[PrimaryKey]
		public string EventId { get; set; }
		public string Location { get; set; }

		[ManyToMany(typeof(TeamEvent))]
		public List<Team> Teams { get; set; }
		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public List<Match> Matches { get; set; }

		public int Week { get; set; }
	}
}
