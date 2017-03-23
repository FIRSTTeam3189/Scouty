using System;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace Scouty
{
	public class Note
	{
		[PrimaryKey, AutoIncrement]
		public int NoteId { get; set; }

		[ForeignKey(typeof(Match))]
		public string MatchId { get; set; }
		[ForeignKey(typeof(Team))]
		public int TeamNumber { get; set; }
		[ForeignKey(typeof(DataSheet))]
		public string DataSheetId { get; set; }

		[MaxLength(512)]
		public string Data { get; set; }
		public string URI { get; set; }
	}
}
