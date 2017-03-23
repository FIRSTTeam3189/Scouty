using System;
using System.Collections.Generic;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace Scouty
{
	public class DataSheet
	{
		[PrimaryKey]
		public string Id { get; set; }
		public string Drivetrain { get; set; }
		public string Autonomous { get; set; }
		public double RobotSpeed { get; set; }
		public double ClimbSpeed { get; set; }
		public ExLevel DriverEx { get; set; }
		public ExLevel CoDriverEx { get; set; }
		public ExLevel CoachEx { get; set; }
		public ExLevel HumanPlayer { get; set; }
		public double ExpectedGears { get; set; }
		public double ExpectedBalls { get; set; }

		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public List<Note> Notes { get; set; }

		[ForeignKey(typeof(Team))]
		public int TeamNumber { get; set; }
		[OneToOne]
		public Team Team { get; set; }
		public int Year { get; set; }
		public bool DirtyBoy { get; set; }
	}

	public enum ExLevel
	{
		NA,
		Little,
		Some,
		OneYear,
		TwoYears,
		ThreeYears,
		FourToSixYears,
		SevenPlusYears
	}
}
