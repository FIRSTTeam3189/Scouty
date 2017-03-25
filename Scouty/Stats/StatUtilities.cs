using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Scouty
{
	public static class StatUtilities
	{
		static StatUtilities()
		{
			StatUIStrings = ((StatType[])Enum.GetValues(typeof(StatType))).ToDictionary(k => k, k => k.GetEnumString());
		}

		public static readonly IReadOnlyDictionary<StatType, string> StatUIStrings;
		public static readonly IReadOnlyDictionary<StatType, Func<TeamMatchStat, double>> StatFunctions =
			new Dictionary<StatType, Func<TeamMatchStat, double>>() {
			{ StatType.AutonomousGearDrops, ms => ms.Events.Count(x => x.Action == ActionType.GearDropped && x.Period == ActionPeriod.Auto) },
			{ StatType.AutonomousGearEfficiency,
				ms => {
					if (ms.Events.Count(x => x.Action == ActionType.GearPickedUp || x.Action == ActionType.GearCollected && x.Period == ActionPeriod.Auto) == 0)
						return 0;
					return ms.Events.Count(x => x.Action == ActionType.GearHung && x.Period == ActionPeriod.Auto) / (double)(ms.Events.Count(x => x.Action == ActionType.GearCollected || x.Action == ActionType.GearPickedUp && x.Period == ActionPeriod.Auto));
				}
			},
			{ StatType.AutonomousGearHung,
				ms => ms.Events.Count(x => x.Action == ActionType.GearHung && x.Period == ActionPeriod.Auto) },
			{ StatType.AutonomousShootHigh,
				ms => ms.Events.Count(x => x.Action == ActionType.MakeHigh || x.Action == ActionType.MissHigh && x.Period == ActionPeriod.Auto)},
			{ StatType.AutonomousShootHighAccuracy,
				ms => {
					if (ms.Events.Count(x => x.Action == ActionType.MakeHigh || x.Action == ActionType.MissHigh && x.Period == ActionPeriod.Auto) == 0){
						return 0;
					}
					return ms.Events.Count(x => x.Action == ActionType.MakeHigh && x.Period == ActionPeriod.Auto) / (double)ms.Events.Count(x => x.Action == ActionType.MakeHigh || x.Action == ActionType.MissHigh && x.Period == ActionPeriod.Auto);
				}
			},
			{ StatType.AutonomousShootHighMiss,
				ms => ms.Events.Count(x => x.Action == ActionType.MissHigh && x.Period == ActionPeriod.Auto) },
			{ StatType.AutonomousShootLow,
				ms => ms.Events.Count(x => x.Action == ActionType.MakeLow && x.Period == ActionPeriod.Auto) },
			{ StatType.AutonomousShootLowAccuracy,
				ms => {
					if (ms.Events.Count(x => x.Action == ActionType.MakeLow || x.Action == ActionType.MissLow && x.Period == ActionPeriod.Auto) == 0){
						return 0;
					}
					return ms.Events.Count(x => x.Action == ActionType.MakeLow && x.Period == ActionPeriod.Auto) / (double)ms.Events.Count(x => x.Action == ActionType.MakeLow || x.Action == ActionType.MissLow && x.Period == ActionPeriod.Auto);
				}
			},
			{ StatType.AutonomousShootLowMiss,
				ms => ms.Events.Count(x => x.Action == ActionType.MissLow && x.Period == ActionPeriod.Auto) },
			{ StatType.ClimbAttempts,
				ms => ms.Events.Count(x => x.Action == ActionType.ClimbAttempted) },
			{ StatType.ClimbNonAttempts,
				ms => ms.Events.Any(x => x.Action == ActionType.ClimbAttempted) ? 0 : 1 },
			{ StatType.ClimbNonSuccess,
				ms => ms.Events.Any(x => x.Action == ActionType.ClimbSuccessful) ? 0 : 1 },
			{ StatType.ClimbSuccess,
				ms => ms.Events.Count(x => x.Action == ActionType.ClimbSuccessful) },
			{ StatType.Foul,
				ms => ms.Events.Count(x => x.Action == ActionType.Foul) },
			{ StatType.FoulPoints,
				ms => ms.Events.Count(x => x.Action == ActionType.Foul)*5 + ms.Events.Count(x => x.Action == ActionType.TechnicalFoul)*25 },
			{ StatType.GearAquireDifferencial,
				ms => ms.Events.Count(x => x.Action == ActionType.GearCollected) - ms.Events.Count(x => x.Action == ActionType.GearPickedUp) },
			{ StatType.GearCollect,
				ms => ms.Events.Count(x => x.Action == ActionType.GearCollected) },
			{ StatType.GearDrops,
				ms => ms.Events.Count(x => x.Action == ActionType.GearDropped) },
			{ StatType.GearEfficiency,
				ms => {
					if (ms.Events.Count(x => x.Action == ActionType.GearPickedUp || x.Action == ActionType.GearCollected) == 0)
						return 0;
					return ms.Events.Count(x => x.Action == ActionType.GearHung) / (double)ms.Events.Count(x => x.Action == ActionType.GearCollected || x.Action == ActionType.GearPickedUp);
				}
			},
			{ StatType.GearHung,
				ms => ms.Events.Count(x => x.Action == ActionType.GearHung) },
			{ StatType.GearPickup,
				ms => ms.Events.Count(x => x.Action == ActionType.GearPickedUp) },
			{ StatType.Mobility,
				ms => ms.Events.Count(x => x.Action == ActionType.Mobility) },
			{ StatType.RobotFailure,
				ms => ms.Events.Count(x => x.Action == ActionType.RobotDisabled) },
			{ StatType.RotorsTurned,
				ms => {
					var gearsHung = ms.Events.Count(x => x.Action == ActionType.GearHung);
					var start = 1;

					if (gearsHung >= 13)
						return 4;
					if (gearsHung >= 7)
						return 3;
					if (gearsHung >= 3)
						return 2;
					if (gearsHung >= 1)
						return 1;
					return 0;
				}
			},
			{ StatType.Score,
				ms => {
					var autoGearHung = ms.Events.Any(x => x.Action == ActionType.GearHung && x.Period == ActionPeriod.Auto);
					var teleopGears = ms.Events.Count(x => x.Action == ActionType.GearHung);
					var autoHighMade = ms.Events.Count(x => x.Action == ActionType.MakeHigh && x.Period == ActionPeriod.Auto);
					var teleopHighMade = ms.Events.Count(x => x.Action == ActionType.MakeHigh && x.Period == ActionPeriod.Teleop);
					var autoLowMade = ms.Events.Count(x => x.Action == ActionType.MakeLow && x.Period == ActionPeriod.Teleop);
					var teleopLowMade = ms.Events.Count(x => x.Action == ActionType.MakeLow && x.Period == ActionPeriod.Teleop);
					var climbPoints = ms.Events.Any(x => x.Action == ActionType.ClimbSuccessful) ? 50 : 0;
					var foulPoints = ms.Events.Count(x => x.Action == ActionType.Foul)*5 + ms.Events.Count(x => x.Action == ActionType.TechnicalFoul)*25;
					var teleopRotors = 0;
					int pressurePoints = (autoHighMade * 9 + autoLowMade * 3 + teleopHighMade * 3 + teleopLowMade)/9;

					// calculate points from events
					int points  = pressurePoints + climbPoints - foulPoints;
					if (teleopGears >= 13)
						teleopRotors++;
					if (teleopGears >= 7)
						teleopRotors++;
					if (teleopGears >= 3)
						teleopRotors++;
					if (teleopGears >= 1)
						teleopRotors++;
					if (autoGearHung)
						teleopRotors--;

					points += teleopRotors * 40;
					points += autoGearHung ? 60 : 0;
					return points;
				}
			},
			{ StatType.ShootHigh,
				ms => ms.Events.Count(x => x.Action == ActionType.MakeHigh || x.Action == ActionType.MissHigh) },
			{ StatType.ShootHighAccuracy,
				ms => {
					if (ms.Events.Count(x => x.Action == ActionType.MakeHigh || x.Action == ActionType.MissHigh) == 0){
						return 0;
					}
					return ms.Events.Count(x => x.Action == ActionType.MakeHigh) / (double)ms.Events.Count(x => x.Action == ActionType.MakeHigh || x.Action == ActionType.MissHigh);
				}
			},
			{ StatType.ShootHighMiss,
				ms => ms.Events.Count(x => x.Action == ActionType.MissHigh) },
			{ StatType.ShootLow,
				ms => ms.Events.Count(x => x.Action == ActionType.MakeLow || x.Action == ActionType.MissLow) },
			{ StatType.ShootLowAccuracy,
				ms => {
					if (ms.Events.Count(x => x.Action == ActionType.MakeLow || x.Action == ActionType.MissLow) == 0){
						return 0;
					}
					return ms.Events.Count(x => x.Action == ActionType.MakeLow) / (double)ms.Events.Count(x => x.Action == ActionType.MakeLow || x.Action == ActionType.MissLow);
				}
			},
			{ StatType.ShootLowMiss,
				ms => ms.Events.Count(x => x.Action == ActionType.MissLow) },
			{ StatType.SpilledBalls,
				ms => ms.Events.Count(x => x.Action == ActionType.SpilledBalls) },
			{ StatType.TechnicalFoul,
				ms => ms.Events.Count(x => x.Action == ActionType.TechnicalFoul) }
		};

		/// <summary>
		/// Splits string by camal case and puts whitespace between words
		/// </summary>
		/// <returns>The string after putting space between words.</returns>
		/// <param name="str">String to split.</param>
		public static string SplitByCamalCase(this string str)
		{
			return System.Text.RegularExpressions.Regex.Replace(str,
																"(?<=[a-z])([A-Z])",
																" $1",
																System.Text.RegularExpressions.RegexOptions.CultureInvariant).Trim();
		}

		/// <summary>
		/// Gets the string describing stat action
		/// </summary>
		/// <returns>The enum string.</returns>
		/// <param name="value">Value.</param>
		public static string GetEnumString(this System.Enum value)
		{
			var field = value.GetType().GetRuntimeField(value.ToString());
			var attribs = field.GetCustomAttributes(typeof(Description), false);
			if (attribs != null)
			{
				foreach (var attrib in attribs)
				{
					if (attrib is Description)
						return (attrib as Description).DescriptionStr;
				}
			}

			return value.ToString().SplitByCamalCase();
		}

		/// <summary>
		/// Gets the team match stat
		/// </summary>
		/// <returns>The team average.</returns>
		/// <param name="teamStat">Team stat.</param>
		/// <param name="type">Type.</param>
		public static double GetTeamAverage(this TeamStat teamStat, StatType type)
		{
			return teamStat.MatchStats.Average(x => StatFunctions[type](x));
		}

		public static double GetTeamVariance(this TeamStat teamStat, StatType type)
		{
			var avg = teamStat.GetTeamAverage(type);
			return Math.Pow(teamStat.MatchStats.Select(x => StatFunctions[type](x)).Sum(), 2);
		}

		public static double GetTeamStdDeviation(this TeamStat teamStat, StatType type)
		{
			return Math.Sqrt(teamStat.GetTeamVariance(type));
		}

		public static double GetAverage(this List<TeamStat> teamStats, StatType type)
		{
			return teamStats.Sum(x => x.GetTeamAverage(type)) / (teamStats.Count - 1);
		}

		public static double GetVariance(this List<TeamStat> teamStats, StatType type)
		{
			var avg = teamStats.GetAverage(type);
			return Math.Pow(teamStats.Select(x => x.GetTeamAverage(type)).Sum(), 2);
		}

		public static double GetStdDeviation(this List<TeamStat> teamStats, StatType type)
		{
			return Math.Sqrt(teamStats.GetVariance(type));
		}

		public static double GetZScore(this TeamStat teamStats, StatType type, double avg, double stdDeviation)
		{
			return (teamStats.GetTeamAverage(type) - avg) / stdDeviation;
		}
	}

	public enum StatType
	{
		[Description("Crossing Baseline")]
		Mobility,
		AutonomousGearHung,
		AutonomousGearEfficiency,
		AutonomousGearDrops,
		GearHung,
		GearEfficiency,
		GearDrops,
		GearAquireDifferencial,
		GearPickup,
		GearCollect,

		RotorsTurned,
		Score,

		AutonomousShootHigh,
		AutonomousShootHighMiss,
		AutonomousShootHighAccuracy,
		AutonomousShootLow,
		AutonomousShootLowAccuracy,
		AutonomousShootLowMiss,
		ShootHigh,
		ShootHighAccuracy,
		ShootHighMiss,
		ShootLow,
		ShootLowAccuracy,
		ShootLowMiss,

		ClimbAttempts,
		ClimbNonAttempts,
		ClimbSuccess,
		ClimbNonSuccess,

		SpilledBalls,
		Foul,
		TechnicalFoul,
		FoulPoints,
		RobotFailure
	}

	[AttributeUsage(AttributeTargets.All)]
	public class Description : Attribute
	{
		public string DescriptionStr { get; set; }

		public Description(string description)
		{
			if (string.IsNullOrWhiteSpace(description))
				throw new ArgumentNullException(nameof(description));

			DescriptionStr = description;
		}
	}
}
