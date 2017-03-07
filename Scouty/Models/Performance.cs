using System.Collections.Generic;
using System.Linq;
using SQLiteNetExtensions.Attributes;

namespace Scouty
{
	public class Performance
	{
		private static ActionPeriod a = ActionPeriod.Auto;
		private static ActionPeriod t = ActionPeriod.Teleop;

		[ForeignKey(typeof(Match))]
		public string MatchId { get; set; }
		[ForeignKey(typeof(Team))]
		public int TeamNumber { get; set; }

		[ManyToOne]
		public Team Team { get; set; }
		[ManyToOne]
		public Match Match { get; set; }

		public AllianceColor Color { get; set; }

		public IEnumerable<RobotEvent> Actions => Match.RobotEvents.Where(x => x.TeamId == TeamNumber && x.MatchId == MatchId);
		public int this[ActionPeriod period, ActionType type] => Actions.Count(x => x.Period == period && x.Action == type);

		public int AutoSpilledBalls => this[a, ActionType.SpilledBalls];

		public double AutoHighAccuracy => AutoHighShotMade / (double)(AutoHighShotMissed + AutoHighShotMade);
		public int AutoHighShotMade => this[a, ActionType.MakeHigh];
		public int AutoHighShotMissed => this[a, ActionType.MissHigh];

		public double AutoLowAccuracy => AutoLowShotMade / (double)(AutoLowShotMissed + AutoLowShotMade);
		public int AutoLowShotMade => this[a, ActionType.MakeLow];
		public int AutoLowShotMissed => this[a, ActionType.MissLow];

		public int TeleopBallsSpilled => this[t, ActionType.SpilledBalls];

		public double TeleopHighAccuracy => TeleopHighShotMade / (double)(TeleopHighShotMissed + TeleopHighShotMade);
		public int TeleopHighShotMade => this[t, ActionType.MakeHigh];
		public int TeleopHighShotMissed => this[t, ActionType.MissHigh];

		public double TeleopLowAccuracy => TeleopLowShotMade / (double)(TeleopLowShotMissed + TeleopLowShotMade);
		public int TeleopLowShotMade => this[t, ActionType.MakeLow];
		public int TeleopLowShotMissed => this[t, ActionType.MissLow];

		public int AutoGearCollected => this[a, ActionType.GearCollected];
		public int AutoGearHung => this[a, ActionType.GearHung];
		public int AutoGearDropped => this[a, ActionType.GearDropped];

		public int TeleopGearCollected => this[t, ActionType.GearCollected];
		public int TeleopGearHung => this[t, ActionType.GearHung];
		public int TeleopGearDropped => this[t, ActionType.GearDropped];

		public bool RobotDisabled => Actions.Any(x => x.Action == ActionType.RobotDisabled);
		public bool ClimbAttempted => Actions.Any(x => x.Action == ActionType.ClimbAttempted);
		public bool ClimbSuccessful => Actions.Any(x => x.Action == ActionType.ClimbSuccessful);
		public bool ClimbFailed => ClimbAttempted ^ ClimbSuccessful;
	}

	public enum AllianceColor { 
		Red,
		Blue
	}
}