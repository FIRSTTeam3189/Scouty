using System;

using Xamarin.Forms;

namespace Scouty
{
	public static class ModelExtensions
	{
		public static string GetActionString(this RobotEvent ev) {
			var str = "";
			switch (ev.Action) { 
				case ActionType.ClimbAttempted:
					str = "Climb attempted";
					break;
				case ActionType.ClimbSuccessful:
					str = "Climb Successful";
					break;
				case ActionType.Foul:
					str = "Committed a Foul";
					break;
				case ActionType.GearCollected:
					str = "Gear Collect: Feeder";
					break;
				case ActionType.GearDropped:
					str = "Dropped a gear";
					break;
				case ActionType.GearHung:
					str = "Hung gear";
					break;
				case ActionType.GearPickedUp:
					str = "Gear Collect: Ground";
					break;
				case ActionType.MakeHigh:
					str = "Make high shot";
					break;
				case ActionType.MakeLow:
					str = "Make low shot";
					break;
				case ActionType.MissHigh:
					str = "MISSED a high shot";
					break;
				case ActionType.MissLow:
					str = "MISSED a low shot";
					break;
				case ActionType.Mobility:
					str = "Moved during autonomous";
					break;
				case ActionType.RobotDisabled:
					str = "Malfunctioned, did nothing, or was disabled";
					break;
				case ActionType.SpilledBalls:
					str = "Splilled balls";
					break;
				case ActionType.TechnicalFoul:
					str = "TECHNICAL foul";
					break;
			}

			if (ev.Period == ActionPeriod.Auto)
				return str + " during Autonomous";
			return str + " during Teleop";
		}
		/// <summary>
		/// Gets the event time string.
		/// </summary>
		/// <returns>The event time string.</returns>
		/// <param name="time">Time.</param>
		public static string GetEventTimeString(this ActionPeriod time)
		{
			if (time == ActionPeriod.Auto)
				return "Autonomous Period";
			else
				return "Teleop Period";
		}

		/// <summary>
		/// Gets the event time short string.
		/// </summary>
		/// <returns>The event time short string.</returns>
		/// <param name="time">Time.</param>
		public static string GetEventTimeShortString(this ActionPeriod time)
		{
			if (time == ActionPeriod.Auto)
				return "A";
			else
				return "T";
		}
	}
}

