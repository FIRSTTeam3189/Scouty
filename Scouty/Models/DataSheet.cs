using System;
using System.Collections.Generic;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace Scouty
{
	public class DataSheet
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public string Drivetrain { get; set; }
		public string Autonomous { get; set; }
		public string RobotSpeed { get; set; }
		public string ClimbSpeed { get; set; }
		public ExLevel DriverEx { get; set; }
		public ExLevel CoDriverEx { get; set; }
		public ExLevel CoachEx { get; set; }
		public ExLevel HumanPlayer { get; set; }
		public int ExpectedGears { get; set; }
		public int ExpectedBalls { get; set; }

		[ForeignKey(typeof(Team))]
		public int TeamNumber { get; set; }
		[OneToOne]
		public Team Team { get; set; }
		public int Year { get; set; }
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
