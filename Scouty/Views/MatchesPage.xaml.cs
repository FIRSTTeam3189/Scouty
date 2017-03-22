using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Threading.Tasks;
using SQLiteNetExtensions.Extensions;
using BlueAllianceClient;
using Scouty.Client;

namespace Scouty
{
	public partial class MatchesPage : TabbedPage
	{
		ObservableCollection<MatchGroup> MatchList { get; set; }
		ObservableCollection<Team> Teams { get; set; }
		BlueAllianceContext _blueContext = new BlueAllianceContext();
		Event MatchEvent { get; }
		public MatchesPage(Event ev)
		{
			InitializeComponent();
			MatchEvent = ev;
			MatchList = new ObservableCollection<MatchGroup>();
			Teams = new ObservableCollection<Team>();
			Matches.ItemsSource = MatchList;
			Matches.ItemSelected += Matches_ItemSelected;
			EventTeams.ItemsSource = Teams;
			EventTeams.ItemSelected += EventTeams_ItemSelected;
			Title = ev.Name;
			CurrentPage = Children[1];
			ViewGradedMatches.Clicked += async (sender, e) => await Navigation.PushAsync(new MyGradesPage(MatchEvent));
			SendData.Clicked += SendData_Clicked;
		}

		protected override void OnAppearing()
		{
			if (MatchList.Count == 0)
			{
				Matches.BeginRefresh();
				EventTeams.BeginRefresh();
				Task.Run(async () =>
				{
					var ev = MatchEvent;
					var db = DbContext.Instance.Db;
					if (db.Table<Match>().Where(x => x.EventId == ev.EventId).Count() == 0)
					{
						// Pull the latest from BlueAlliance
						var trueEvent = await _blueContext.GetEvent(2017, ev.EventId.Substring(4));

						DbContext.Instance.InsertOrUpdateEvent(trueEvent);
					}


					// Now get the matches from that event
					var m = (db.GetAllWithChildren<Match>(x => x.EventId == ev.EventId)).ToList();
					ev.Matches = m ?? new List<Match>();

					var matchGroups = MatchGroup.FromMatches(m);

					// Now get all of the teams
					var attendingTeams = db.Table<TeamEvent>().Where(x => x.EventId == ev.EventId).Select(x => x.TeamNumber).ToList();
					var allTeams = db.Table<Team>().ToList();

					ev.Teams = new List<Team>();
					foreach (var t in allTeams)
						if (attendingTeams.Contains(t.TeamNumber))
							ev.Teams.Add(t);
					
					Device.BeginInvokeOnMainThread(() =>
					{
						MatchList.Clear();
						Teams.Clear();
						foreach (var g in matchGroups)
							MatchList.Add(g);
						foreach (var team in ev.Teams)
							Teams.Add(team);
						Matches.EndRefresh();
						EventTeams.EndRefresh();
					});
				});
			}
			base.OnAppearing();
		}

		async void Matches_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem != null) {
				var match = e.SelectedItem as Match;

				Matches.SelectedItem = null;

				await Navigation.PushAsync(new TeamSelectPage(match));
			}
		}

		async void SendData_Clicked(object sender, EventArgs e)
		{
			SendData.IsEnabled = false;
			try
			{
				// Get all of the Robot events from the server
				var db = DbContext.Instance.Db;
				List<string> evMatches;
				if (MatchEvent.Matches != null && MatchEvent.Matches.Count != 0)
					evMatches = MatchEvent.Matches.Select(x => x.MatchId).ToList();
				else
					evMatches = db.Table<Match>().Where(x => x.EventId == MatchEvent.EventId).Select(x => x.MatchId).ToList();
				var allEvents = db.Table<RobotEvent>().ToList().Where(x => evMatches.Contains(x.MatchId));

				var success = await ServerClient.Instance.PostAsync("api/RobotEvent/Post", allEvents);

				if (success)
				{
					await DisplayAlert("Success", "All events uploaded to the server. Deleting local robot events.", "OK");
					db.DeleteAll(allEvents);
				}
				else {
					await DisplayAlert("Failure", "Events not posted, try again later", "OK");
				}

				SendData.IsEnabled = true;
			}
			catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine(ex.ToString());
			}

			await DisplayAlert("Failure", "Events not posted, try again later", "OK");
			SendData.IsEnabled = true;


		}

		async void EventTeams_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem != null)
			{
				var team = e.SelectedItem as Team;

				EventTeams.SelectedItem = null;

				// See what the user wants to do
				var action = await DisplayActionSheet("What would you like to do?", "Nothing", null, "Pit Scout", "View Stats");
				if (action == "Nothing")
					return;
				if (action == "Pit Scout")
				{
					await Navigation.PushAsync(new PitScoutPage(team));
					return;
				}
					
				// Disable lists
				EventTeams.IsEnabled = false;
				Matches.IsEnabled = false;

				// Grab the stats for the event
				var stats = await ServerClient.Instance.GetTeamStats(MatchEvent.EventId);

				EventTeams.IsEnabled = true;
				Matches.IsEnabled = true;

				if (stats == null)
				{
					await DisplayAlert("Failure", "Failed to load stats", "OK");
					return;
				}

				await Navigation.PushAsync(new TeamStatPage(stats, team.TeamNumber));
			}
		}
	}

	class MatchGroup : ObservableCollection<Match> {
		private static int GetLevelNum(string info) {
			if (info.StartsWith("qf", StringComparison.CurrentCultureIgnoreCase))
				return 2;
			if (info.StartsWith("qm", StringComparison.CurrentCultureIgnoreCase))
				return 1;
			if (info.StartsWith("ef", StringComparison.CurrentCultureIgnoreCase)) 
				return 3;
			if (info.StartsWith("sf", StringComparison.CurrentCultureIgnoreCase))
				return 4;
			if (info.StartsWith("f", StringComparison.CurrentCultureIgnoreCase))
				return 5;

			return 0;
		}

		private static readonly Dictionary<int, string> LevelDescription = new Dictionary<int, string> {
			{ 1, "Qualifications" },
			{ 2, "Quarter Finals" },
			{ 3, "Eigth Finals" },
			{ 4, "Semi Finals" },
			{ 5, "Finals" }
		};

		private static int GetMatchNum(string info) {
			var split = info.Split(null as char[]);
			return int.Parse(split[split.Length - 1]);
		}

		public static IEnumerable<MatchGroup> FromMatches(IEnumerable<Match> matches) {
			var ma =(from m in matches
					group m by GetLevelNum(m.MatchInfo) into g
					orderby g.Key
			        select new MatchGroup(g.Select(x => x).OrderBy(x => GetMatchNum(x.MatchInfo)), g.Key));
			var grps = new ObservableCollection<MatchGroup>();
			foreach (var g in ma)
				grps.Add(g);

			return grps;
		}

		public string Level { get; set; }
		public string ShortLevel { get; set; }

		public MatchGroup(IEnumerable<Match> matches, int compLevel) {
			foreach (var m in matches)
				Add(m);

			Level = LevelDescription[compLevel];
			var split = Level.Split(null as char[]);
			ShortLevel = "";
			foreach (var s in split)
				ShortLevel += s[0];
		}
	}


}
