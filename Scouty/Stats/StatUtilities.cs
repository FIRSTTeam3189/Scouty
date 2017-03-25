using System;
using System.Collections.Generic;

namespace Scouty
{
	public static class StatUtilities
	{
		public static readonly IReadOnlyDictionary<SelectorType, string> SelectorUIStrings = new Dictionary<SelectorType, string>() {
			
		};
	}

	public enum SelectorType
	{
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
		ClimbAttemptDifferential,
		ClimbSuccess,
		ClimbNonSuccess,

		SpilledBalls,
		Foul,
		TechnicalFoul,
		FoulPoints,
		RobotFailure
	}
}
