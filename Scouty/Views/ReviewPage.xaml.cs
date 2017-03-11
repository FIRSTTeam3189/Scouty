using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Scouty.Views
{
	public partial class ReviewPage : ContentPage
	{
		public ObservableCollection<GroupedRobotEvent> Groups { get; set; }

		public ReviewPage(IEnumerable<RobotEvent> events)
		{
			InitializeComponent();
			var groups = events.GroupBy(x => x.Period, (k, ev) => new GroupedRobotEvent(k, ev));

			Title = "Confirm Events";

			Groups = new ObservableCollection<GroupedRobotEvent>();
			foreach (var grp in groups)
				Groups.Add(grp);

			EventList.ItemsSource = Groups;
			EventList.ItemSelected += SelectedEvent;

			ToolbarItems.Add(new ToolbarItem("Save", null, async () => {
				if (await DisplayAlert("Are you sure?", "You will not be able to edit this saved data, are you sure that it is correct?", "Yes", "No")){
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

						// Pop 3 times
						Device.BeginInvokeOnMainThread(() =>
						{
							Navigation.PopAsync(false);
							Navigation.PopAsync(false);
							Navigation.PopAsync(false);
						});
					});
				}

			}));
		}

		void Confirm(object sender, EventArgs e)
		{
			var events = Groups.Select(x => x);
			var allThings = new List<RobotEvent>();
			foreach (var ev in events)
			{
				foreach (var t in ev)
				{
					allThings.Add(t.Event);
				}
			}
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
			} else
				Name = TELEOP_NAME;
		}
	}

	public class GroupedRobotEvent : ObservableCollection<RobotEventUI>
	{
		public string ShortName { get; set; }
		public string LongName { get; set; }
		public ActionPeriod Time { get; set; }

		public GroupedRobotEvent(ActionPeriod time, IEnumerable<RobotEvent> events)
		{
			foreach (var ev in events)
			{
				Add(new RobotEventUI(ev));
				if (time != ev.Period)
					throw new ArgumentException("Mismatch of event time!");
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