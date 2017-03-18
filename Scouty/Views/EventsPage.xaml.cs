using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using BlueAllianceClient;
using Scouty.Client.Models;
using Scouty.Client;

namespace Scouty
{
	public partial class EventsPage : CarouselPage
	{
		private BlueAllianceContext _baContext = new BlueAllianceContext();
		public ObservableCollection<EventGroup> EventsList { get; set; }
		public EventsPage()
		{
			InitializeComponent();

			EventsList = new ObservableCollection<EventGroup>();
			Events.ItemsSource = EventsList;

			this.refreshEventsButton.Clicked += RefreshEventsButton_Clicked;
			this.deleteEventsButton.Clicked += DeleteEventsButton_Clicked;
			this.TestServer.Clicked += TestServer_Clicked;
			Events.ItemSelected += Events_ItemSelected;

			Title = "Events";
		}

		protected override void OnAppearing()
		{
			// Get all of the events from the database if they are there
			Task.Run(() =>
			{
				var c = DbContext.Instance.Db;
				var events = c.Table<Event>().ToList();
				var eventGroups = EventGroup.FromEvents(events);
				Device.BeginInvokeOnMainThread(() => {
					foreach (var e in eventGroups)
						EventsList.Add(e);
				});
			});
			base.OnAppearing();
		}

		async void RefreshEventsButton_Clicked(object sender, EventArgs e)
		{
			refreshEventsButton.IsEnabled = false;

			EventsList.Clear();

			// Pull down the latest shizz
			var events = await _baContext.GetEvents(2017);

			// Put them in the shizz
			DbContext.Instance.InsertOrUpdateEvents(events);

			// Get the shizz back
			var dbEvents = DbContext.Instance.Db
								  .Table<Event>()
								  .ToList();

			// Get the groups
			var groups = EventGroup.FromEvents(dbEvents);

			foreach (var grp in groups)
			{
				EventsList.Add(grp);
			}

			refreshEventsButton.IsEnabled = true;
		}

		async void DeleteEventsButton_Clicked(object sender, EventArgs e)
		{
			deleteEventsButton.IsEnabled = false;

			var confirm = await DisplayAlert("Are you sure?", "This will delete everything you cared about remembered in your life?", "Yes", "No");

			if (confirm)
			{
				EventsList.Clear();

				// Delete all of the shit in the events table
				DbContext.Instance.Db.DeleteAll<Event>();
				DbContext.Instance.Db.DeleteAll<Team>();
				DbContext.Instance.Db.DeleteAll<Match>();
				DbContext.Instance.Db.DeleteAll<Performance>();
				DbContext.Instance.Db.DeleteAll<RobotEvent>();
				DbContext.Instance.Db.DeleteAll<TeamEvent>();
			}

			deleteEventsButton.IsEnabled = true;
		}

		void Events_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem != null)
				Navigation.PushAsync(new MatchesPage((Event)e.SelectedItem));

			Events.SelectedItem = null;
		}

		async void TestServer_Clicked(object sender, EventArgs e)
		{
			var evRequest = new RefreshEventRequest { 
				Year = 2017
			};

			try
			{
				var ev = ServerClient.Instance.PostAsync<RefreshEventRequest, List<Scouty.Event>>("api/events/Refresh", evRequest);
			}
			catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine($"{ex.Message} \n{ex}");
			}
		}
	}

	public class EventGroup : ObservableCollection<Event> {
		public static ObservableCollection<EventGroup> FromEvents(IEnumerable<Event> events) {
			return new ObservableCollection<EventGroup>((from e in events
														 group e by e.Week into g
														 orderby g.Key
			                                             select new EventGroup(g.Select(x => x).OrderBy(x => x.Name), g.Key)));
		}

		public EventGroup(IEnumerable<Event> events, int week) {
			Week = $"Week {week}";
			WeekShort = $"{week}";
			foreach (var e in events)
				Add(e);
		}

		public string Week { get; set; } = "";
		public string WeekShort { get; set; } = "";
	}
}
