using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace Scouty
{
	public class RobotEvent
	{
		[ForeignKey(typeof(Team))]
		public int TeamId { get; set; }
		[ForeignKey(typeof(Match))]
		public int MatchId { get; set; }

		public ActionType Action { get; set; }
		public ActionPeriod Period { get; set; }

		[PrimaryKey]
		public int Time { get; set; } 
	}

	public enum ActionType
	{
		MakeHigh,
		MakeLow,
		MissHigh,
		MissLow,
		GearCollected,
		GearDropped,
		GearHung,
		ClimbAttempted,
		ClimbSuccessful,
		RobotDisabled,
		SpilledBalls
	}

	public enum ActionPeriod { 
		Auto,
		Teleop
	}
}