using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace Scouty
{
	public partial class GradePage : TabbedPage
	{
		public Team GradedTeam { get; }
		public Match GradedMatch { get; }
		public ObservableCollection<GroupedRobotEvent> Groups { get; set; }
		private bool HasAppeared { get; set; }
		private List<EventCounter>  Counters { get; }
		private ActionPeriod CurrentPeriod { get; set; }
		private bool HasGear { get; set; }
		private int EventNum { get; set; }

		public GradePage(Match match, Team team)
		{
			InitializeComponent();
			Title = match.MatchInfo.ToUpper() + " - " + team.TeamNumber;
			GradedTeam = team;
			GradedMatch = match;
			Groups = new ObservableCollection<GroupedRobotEvent>() { 
				new GroupedRobotEvent(ActionPeriod.Auto),
				new GroupedRobotEvent(ActionPeriod.Teleop)
			};
			EventList.ItemsSource = Groups;
			EventList.ItemSelected += SelectedEvent;
			Counters = new List<EventCounter>();

			ToolbarItems.Add(new ToolbarItem("Done", null, async () => {
				if (Groups.SelectMany(x => x).Count() == 0)
					Groups.First(x => x.Time == ActionPeriod.Teleop).Add(GenRobotEvent(ActionType.RobotDisabled, ActionPeriod.Teleop));
				
				if (await DisplayAlert("Are you sure?", "You will not be able to edit this saved data, are you sure that it is correct?", "Yes", "No"))
				{
					// Lets save to database now
					await Task.Run(() =>
					{
						var evs = Groups.SelectMany(x => x).Select(x => new RobotEvent()
						{
							Action = x.Event.Action,
							MatchId = x.Event.MatchId,
							TeamId = x.Event.TeamId
						});

						var db = DbContext.Instance.Db;
						db.InsertAll(evs);

						// Pop 2 times
						Device.BeginInvokeOnMainThread(() =>
						{
							Navigation.PopAsync(false);
							Navigation.PopAsync(false);
						});
					});
				}
			}));

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

			foreach (var grp in Groups)
				grp.CollectionChanged += (sender, e) => Update();

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
				var climbSuccess = Groups.First(x => x.Time == ActionPeriod.Teleop).FirstOrDefault(x => x.Event.Action == ActionType.ClimbSuccessful);
				if (climbSuccess != null)
				{
					Groups.First(x => x.Time == ActionPeriod.Teleop).Remove(climbSuccess);
					HangAttemptSuccess.Text = "Climb Success";
				}
				else if (Groups.First(x => x.Time == ActionPeriod.Teleop).Any(x => x.Event.Action == ActionType.ClimbAttempted))
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
					SwitchPeriod.Text = "Auto";
				}
				else
				{
					SwitchPeriod.BorderColor = Color.Teal;
					CurrentPeriod = ActionPeriod.Teleop;
					SwitchPeriod.Text = "Teleop";
				}
			};
		}

		void InsertOrIgnore(ActionType action, ActionPeriod period) {
			if (Groups.SelectMany(x => x).Select(x => x.Event).Any(x => x.Action == action && x.Period == period))
				return;
			
			Groups.First(x => x.Time == period).Add(GenRobotEvent(action, period));
			Update();
		}

		void Insert(ActionType action, ActionPeriod period) {
			Groups.First(x => x.Time == period).Add(GenRobotEvent(action, period));
			Update();
		}

		void Insert(ActionType action, ActionPeriod period, int amount) {
			for (int i = 0; i < amount; i++)
				Groups.First(x => x.Time == period).Add(GenRobotEvent(action, period));
			Update();
		}

		void Update() {
			foreach (var c in Counters)
				c.Update(Groups.SelectMany(x => x).Select(x => x.Event));
		}

		void AddClimbEvent(ActionType action) {
			// Ignore if we have a successful climb
			if (Groups.SelectMany(x => x).Select(x => x.Event).Any(x => x.Action == ActionType.ClimbSuccessful))
				return;

			if (action == ActionType.ClimbSuccessful && Groups.SelectMany(x => x).Select(x => x.Event).Any(x => x.Action == ActionType.ClimbAttempted))
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
			if (action != ActionType.GearCollected && action != ActionType.GearDropped && action != ActionType.GearPickedUp && action != ActionType.GearHung)
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

		private RobotEventUI GenRobotEvent(ActionType action, ActionPeriod period) {
			return new RobotEventUI(new RobotEvent { 
				Action = action,
				MatchId = GradedMatch.MatchId,
				Period = period,
				TeamId = GradedTeam.TeamNumber,
				Time = EventNum++
			});
		}

		public async void SelectedEvent(object sender, SelectedItemChangedEventArgs e)
		{
			var selectedEvent = e.SelectedItem as RobotEventUI;

			EventList.SelectedItem = null;

			if (selectedEvent != null)
			{
				var eventTimesFull = new List<EventTimeUI>() { new EventTimeUI(ActionPeriod.Auto), new EventTimeUI(ActionPeriod.Teleop) }
					.Where(x => x.Time != selectedEvent.Event.Period)
					.ToArray();

				var eventTimes = eventTimesFull.Select(x => x.Name).ToArray();

				// Lets display an action sheet for this
				var title = $"What would you like to do about {selectedEvent.Type} during {selectedEvent.Event.Period.ToString()}";
				var selectedAction = await DisplayActionSheet(title, "Nothing", "Destroy", eventTimes);

				// See what the user selected
				if (selectedAction == "Nothing")
				{
					return;
				}
				else if (selectedAction == "Destroy")
				{
					// Remove the event
					Groups.First(x => x.Time == selectedEvent.Event.Period).Remove(selectedEvent);
				}
				else
				{
					var selectedTime = eventTimesFull.FirstOrDefault(x => x.Name == selectedAction);
					if (selectedTime != null)
					{
						// Move the group over to its new time group
						Groups.First(x => x.Time == selectedEvent.Event.Period).Remove(selectedEvent);
						selectedEvent.Event.Period = selectedTime.Time;
						var newGrp = Groups.FirstOrDefault(x => x.Time == selectedTime.Time);

						if (newGrp == null)
						{
							newGrp = new GroupedRobotEvent(selectedTime.Time, new List<RobotEvent>() { selectedEvent.Event });
							Groups.Add(newGrp);
						}
						else
						{
							newGrp.Add(selectedEvent);
						}
					}
				}
			}
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

	public class EventTimeUI
	{
		public const string AUTONOMOUS_NAME = "Change To Autonomous";
		public const string TELEOP_NAME = "Change To Teleop";
		public ActionPeriod Time { get; set; }
		public string Name { get; set; }

		public EventTimeUI(ActionPeriod time)
		{
			Time = time;

			if (time == ActionPeriod.Auto)
			{
				Name = AUTONOMOUS_NAME;
			}
			else
				Name = TELEOP_NAME;
		}
	}

	public class GroupedRobotEvent : ObservableCollection<RobotEventUI>
	{
		public string ShortName { get; set; }
		public string LongName { get; set; }
		public ActionPeriod Time { get; set; }

		public GroupedRobotEvent(ActionPeriod time, IEnumerable<RobotEvent> events = null)
		{
			if (events != null)
			{
				foreach (var ev in events)
				{
					Add(new RobotEventUI(ev));
					if (time != ev.Period)
						throw new ArgumentException("Mismatch of event time!");
				}
			}

			Time = time;

			ShortName = time.GetEventTimeShortString();
			LongName = time.GetEventTimeString();
		}
	}

	public class RobotEventUI
	{
		public RobotEvent Event { get; set; }
		public string Type { get; set; }

		public RobotEventUI(RobotEvent associatedEvent)
		{
			Event = associatedEvent;

			// Get the string assoiciated with the event
			Type = associatedEvent.GetActionString();
		}
	}
}
