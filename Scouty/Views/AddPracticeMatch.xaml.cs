using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Scouty
{
	public partial class AddPracticeMatch : ContentPage
	{
		private List<RobotEvent> Events { get; }
		private Event CurrentEvent { get; }
		private Note Note { get; }
		private int PracticeNumber { get; set; } = 0;
		public AddPracticeMatch(IEnumerable<RobotEvent> events, Note note, Event ev)
		{
			InitializeComponent();
			Events = events.ToList();
			CurrentEvent = ev;
			Note = note;
			Teams.ItemsSource = ev.Teams;
			PlusButton.Clicked += (sender, e) => {
				PracticeNumber++;
				UpdateLabel();
			};
			MinusButton.Clicked += (sender, e) => {
				if (--PracticeNumber < 1)
					PracticeNumber = 1;
				UpdateLabel();
			};
			ToolbarItems.Add(new ToolbarItem("Save", null, async () => {
				var team = Teams.SelectedItem as Team;
				if (team == null) {
					await DisplayAlert("No Team", "You need to select a team", "OK");
					return;
				}
				if (PracticeNumber < 1) {
					await DisplayAlert("No Match Number", "You need to put in a match number (0 is not a match number)", "OK");
					return;
				}
				var matchId = CurrentEvent.EventId + "_" + $"p{PracticeNumber}";
				var evs = Events.Select(x => new RobotEvent() { 
					Action = x.Action,
					Period = x.Period,
					MatchId = matchId,
					TeamId = team.TeamNumber
				});
				var perf = new Performance()
				{
					Color = IsRed.IsToggled ? AllianceColor.Red : AllianceColor.Blue,
					MatchId = matchId,
					TeamNumber = team.TeamNumber
				};
				var match = new Match { 
					Event = CurrentEvent,
					EventId = CurrentEvent.EventId,
					MatchId = matchId,
					MatchInfo = $"p {PracticeNumber}",
					TimeString = DateTime.Now.Subtract(TimeSpan.FromMinutes(5)).ToString("t")
				};
				var newNote = new Note { 
					Data = Note.Data,
					MatchId = matchId,
					TeamNumber = team.TeamNumber,
					URI = Note.URI,
					NoteId = 0
				};

				var context = DbContext.Instance;
				context.Db.InsertOrReplace(match);
				context.Db.InsertOrReplace(perf);
				context.Db.InsertAll(evs);

				// Add note now too
				var ds = context.GetOrGenerateDataShit(team.TeamNumber);
				newNote.DataSheetId = ds.Id;
				context.Db.Insert(newNote);

				Device.BeginInvokeOnMainThread(async () =>
				{
					// TODO: Figure out why the helll it crashes when popping two back
					await Navigation.PopAsync(false);
				});
			}));
		}

		private void UpdateLabel() {
			PracticeMatchLabel.Text = $"Practice Match: {PracticeNumber}";
		}
	}
}
