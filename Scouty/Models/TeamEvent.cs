using System;
using SQLiteNetExtensions.Attributes;

namespace Scouty
{
	public class TeamEvent
	{
		[ForeignKey(typeof(Event))]
		public string EventId { get; set; }
		[ForeignKey(typeof(Team))]
		public int TeamNumber { get; set; }
	}
}
