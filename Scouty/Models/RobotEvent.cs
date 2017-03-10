using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace Scouty
{
	public class RobotEvent
	{
		[ForeignKey(typeof(Team))]
		public int TeamId { get; set; }
		[ForeignKey(typeof(Match))]
		public string MatchId { get; set; }

		public ActionType Action { get; set; }
		public ActionPeriod Period { get; set; }

		[PrimaryKey, AutoIncrement]
		public int Time { get; set; } 
	}

	public enum ActionType
	{
		Mobility,
		MakeHigh,
		MakeLow,
		MissHigh,
		MissLow,
		GearCollected,
		GearPickedUp,
		GearDropped,
		GearHung,
		ClimbAttempted,
		ClimbSuccessful,
		RobotDisabled,
		SpilledBalls,
		Foul,
		TechnicalFoul
	}

	public enum ActionPeriod { 
		Auto,
		Teleop
	}
}