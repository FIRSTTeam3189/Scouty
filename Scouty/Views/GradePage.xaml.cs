using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Scouty
{
	public partial class GradePage : ContentPage
	{
		public Team GradedTeam { get; }
		public Match GradedMatch { get; }
		public ObservableCollection<RobotEvent> Events { get; }
		private List<EventCounter>  Counters { get; }
		private ActionPeriod CurrentPeriod { get; set; }
		private bool HasGear { get; set; }

		public GradePage(Match match, Team team)
		{
			InitializeComponent();
			Title = match.Name + " - " + team.TeamNumber;
			GradedTeam = team;
			GradedMatch = match;
			Events = new ObservableCollection<RobotEvent>();
			Counters = new List<EventCounter>();

			// Add counters
			Counters.Add(new EventCounter(HighGoalShots, "High Goal: {0}/{1}", x => {
				var made = x.Count(y => y.Action == ActionType.MakeHigh);
				return new Tuple<int, int>(made, made + x.Count(y => y.Action == ActionType.MissHigh));
			}));
			Counters.Add(new EventCounter(LowGoalShots, "Low Goal: {0}/{1}", x =>
			{
				var made = x.Count(y => y.Action == ActionType.MakeLow);
				return new Tuple<int, int>(made, made + x.Count(y => y.Action == ActionType.MissLow));
			}));

			Counters.Add(new EventCounter(GearStat, "Gears: {0}/{1}", x =>
			{
				var attempts= x.Count(y => y.Action == ActionType.GearCollected || y.Action == ActionType.GearPickedUp);
				var succ = x.Count(y => y.Action == ActionType.GearHung);
				return new Tuple<int, int>(succ, attempts);
			}));
			Counters.Add(new EventCounter(FoulStat, "Fouls: {0} Tech: {1}", (arg) => {
				var fouls = arg.Count(x => x.Action == ActionType.Foul);
				var tfouls = arg.Count(x => x.Action == ActionType.TechnicalFoul);
				return new Tuple<int, int>(fouls, tfouls);
			}));

			// Check whenever the Events change
			Events.CollectionChanged += (sender, e) => {
				foreach (var counter in Counters)
					counter.Update(Events);
			};
			//adds when clicked ;)
			High1Made.Clicked += (sender, e) => Insert(ActionType.MakeHigh, CurrentPeriod);
			High2Made.Clicked += (sender, e) => Insert(ActionType.MakeHigh, CurrentPeriod, 2);
			High4Made.Clicked += (sender, e) => Insert(ActionType.MakeHigh, CurrentPeriod, 4);
			Low1Made.Clicked += (sender, e) => Insert(ActionType.MakeLow, CurrentPeriod);
			Low2Made.Clicked += (sender, e) => Insert(ActionType.MakeLow, CurrentPeriod, 2);
			Low4Made.Clicked += (sender, e) => Insert(ActionType.MakeLow, CurrentPeriod, 4);
			High1Miss.Clicked += (sender, e) => Insert(ActionType.MissHigh, CurrentPeriod);
			High2Miss.Clicked += (sender, e) => Insert(ActionType.MissHigh, CurrentPeriod, 2);
			High4Miss.Clicked += (sender, e) => Insert(ActionType.MissHigh, CurrentPeriod, 4);
			Low1Miss.Clicked += (sender, e) => Insert(ActionType.MissLow, CurrentPeriod);
			Low2Miss.Clicked += (sender, e) => Insert(ActionType.MissLow, CurrentPeriod, 2);
			Low4Miss.Clicked += (sender, e) => Insert(ActionType.MissLow, CurrentPeriod, 4);
			Foul.Clicked += (sender, e) => Insert(ActionType.Foul, CurrentPeriod);
			TechFoul.Clicked += (sender, e) => Insert(ActionType.TechnicalFoul, CurrentPeriod);
			SpilledBalls.Clicked += (sender, e) => Insert(ActionType.SpilledBalls, CurrentPeriod);
			Mobility.Clicked += (sender, e) => InsertOrIgnore(ActionType.Mobility, ActionPeriod.Auto);
			RobotFail.Clicked += (sender, e) => InsertOrIgnore(ActionType.RobotDisabled, ActionPeriod.Teleop);

			// Special cases
			HangAttemptSuccess.Clicked += (sender, e) => {
				var climbSuccess = Events.FirstOrDefault(x => x.Action == ActionType.ClimbSuccessful);
				if (climbSuccess != null)
				{
					Events.Remove(climbSuccess);
				}
				else if (Events.Any(x => x.Action == ActionType.ClimbAttempted))
				{
					AddClimbEvent(ActionType.ClimbSuccessful);
				}
				else {
					AddClimbEvent(ActionType.ClimbAttempted);
				}
			};
			GearPickupHang.Clicked += (sender, e) => {
				if (HasGear)
					AddGearEvent(ActionType.GearHung, CurrentPeriod);
				else
					AddGearEvent(ActionType.GearPickedUp, CurrentPeriod);
			};
			GearCollectDrop.Clicked += (sender, e) => {
				if (HasGear)
					AddGearEvent(ActionType.GearDropped, CurrentPeriod);
				else
					AddGearEvent(ActionType.GearCollected, CurrentPeriod);
			};
			SwitchPeriod.Clicked += (sender, e) =>
			{
				if (CurrentPeriod == ActionPeriod.Teleop)
				{
					SwitchPeriod.BorderColor = Color.Lime;
					CurrentPeriod = ActionPeriod.Auto;
				}
				else
				{
					SwitchPeriod.BorderColor = Color.Teal;
					CurrentPeriod = ActionPeriod.Teleop;
				}
			};
		}

		void InsertOrIgnore(ActionType action, ActionPeriod period) {
			if (Events.Any(x => x.Action == action && x.Period == period))
				return;
			Events.Add(GenRobotEvent(action, period));
			Update();
		}

		void Insert(ActionType action, ActionPeriod period) {
			Events.Add(GenRobotEvent(action, period));
			Update();
		}

		void Insert(ActionType action, ActionPeriod period, int amount) {
			for (int i = 0; i < amount; i++)
				Events.Add(GenRobotEvent(action, period));
			Update();
		}

		void Update() {
			foreach (var c in Counters)
				c.Update(Events);
		}

		void AddClimbEvent(ActionType action) {
			// Ignore if we have a successful climb
			if (Events.Any(x => x.Action == ActionType.ClimbSuccessful))
				return;

			if (action == ActionType.ClimbAttempted && !Events.Any(x => x.Action == ActionType.ClimbAttempted))
			{
				Insert(ActionType.ClimbSuccessful, ActionPeriod.Teleop);
				HangAttemptSuccess.Text = "Revoke Climb";
			}
			else if (action == ActionType.ClimbAttempted)
			{
				InsertOrIgnore(ActionType.ClimbAttempted, ActionPeriod.Teleop);
				HangAttemptSuccess.Text = "Climb Success";
			}
		}

		void AddGearEvent(ActionType action, ActionPeriod period) {
			// Dont care about non gear events
			if (action != ActionType.GearCollected || action != ActionType.GearDropped || action != ActionType.GearPickedUp || action != ActionType.GearHung)
				return;

			Insert(action, period);
			if (action == ActionType.GearCollected || action == ActionType.GearPickedUp)
			{
				GearPickupHang.Text = "Hung Gear";
				GearCollectDrop.Text = "Dropped Gear";
			}
			else {
				GearPickupHang.Text = "Gear Picked-up";
				GearCollectDrop.Text = "Gear Collected";
			}

			HasGear = !HasGear;
		}

		private RobotEvent GenRobotEvent(ActionType action, ActionPeriod period) {
			return new RobotEvent { 
				Action = action,
				MatchId = GradedMatch.MatchId,
				Period = period,
				TeamId = GradedTeam.TeamNumber
			};
		}
	}

	class EventCounter { 
		private Label LabelToUpdate { get; }
		private string Template { get; }
		private int ValueOne { get; set; }
		private int ValueTwo { get; set; }
		private Func<IEnumerable<RobotEvent>, Tuple<int, int>> GetUpdate { get; }

		public EventCounter(Label labelToUpdate, string template, Func<IEnumerable<RobotEvent>, Tuple<int, int>> onUpdate) {
			LabelToUpdate = labelToUpdate;
			Template = template;
			GetUpdate = onUpdate;
		}

		public void Update(IEnumerable<RobotEvent> events) {
			var vals = GetUpdate(events);
			ValueOne = vals.Item1;
			ValueTwo = vals.Item2;
			LabelToUpdate.Text = string.Format(Template, ValueOne, ValueTwo);
		}
	}
}
