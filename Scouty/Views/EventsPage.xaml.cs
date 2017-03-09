using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using BlueAllianceClient;

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
			Events.ItemSelected += Events_ItemSelected;
		}

		protected override void OnAppearing()
		{
			// Get all of the events from the database if they are there
			Task.Run(async () =>
			{
				var c = DbContext.Instance.Db;
				var events = await c.Table<Event>().ToListAsync();
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
			await DbContext.Instance.InsertOrUpdateEvents(events);

			// Get the shizz back
			var dbEvents = await DbContext.Instance.Db
								  .Table<Event>()
								  .ToListAsync();

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
				await DbContext.Instance.Db.DeleteAllAsync<Event>();
				await DbContext.Instance.Db.DeleteAllAsync<Team>();
				await DbContext.Instance.Db.DeleteAllAsync<Match>();
				await DbContext.Instance.Db.DeleteAllAsync<Performance>();
				await DbContext.Instance.Db.DeleteAllAsync<RobotEvent>();
				await DbContext.Instance.Db.DeleteAllAsync<TeamEvent>();
			}

			deleteEventsButton.IsEnabled = true;
		}

		void Events_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem != null)
				Navigation.PushAsync(new MatchesPage((Event)e.SelectedItem));

			Events.SelectedItem = null;
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
