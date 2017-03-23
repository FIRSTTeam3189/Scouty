using System;
using System.Collections.Generic;
using SQLiteNetExtensions.Extensions;
using Xamarin.Forms;

namespace Scouty
{
	public partial class PitScoutPage : ContentPage
	{
		private static readonly Dictionary<string, ExLevel> ExperienceDict = new Dictionary<string, ExLevel>() {
			{ "None", ExLevel.NA },
			{ "Little", ExLevel.Little },
			{ "Some", ExLevel.Some },
			{ "One Year", ExLevel.OneYear },
			{ "Two Years", ExLevel.TwoYears },
			{ "Three Years", ExLevel.ThreeYears },
			{ "Four to Six Years", ExLevel.FourToSixYears },
			{ "Seven+ Years", ExLevel.SevenPlusYears }
		};
		private static readonly List<string> ExpKeys = new List<string>() { 
			"None", "Little", "Some", "One Year", "Two Years", "Three Years", "Four to Six Years", "Seven+ Years"
		};
		public PitScoutPage(Team team)
		{
			InitializeComponent();
			foreach (var key in ExperienceDict.Keys)
			{
				DriverEx.Items.Add(key);
				CoDriverEx.Items.Add(key);
				CoachEx.Items.Add(key);
				HumanPlayerEx.Items.Add(key);
			}

			Title = $"#{team.TeamNumber} - {team.Name}";

			ToolbarItems.Add(new ToolbarItem("Done", null, async () => {
				double o;
				if (DriverEx.SelectedIndex < 0 || CoDriverEx.SelectedIndex < 0 || CoachEx.SelectedIndex < 0 || HumanPlayerEx.SelectedIndex < 0)
				{
					await DisplayAlert("Missing Fields", "You need to fill out all experience fields", "OK");
					return;
				}
				else if (!double.TryParse(RobotSpeed.Text, out o) || !double.TryParse(ClimbSpeed.Text, out o) || !double.TryParse(ExpectedGears.Text, out o) || !double.TryParse(ExpectedHighGoals.Text, out o))
				{
					await DisplayAlert("Missing Fields", "Please enter in valid Climb, Robot speed and Expected Gears/Goals.", "OK");
					return;
				}
				else if (string.IsNullOrWhiteSpace(Drivetrain.Text) || string.IsNullOrWhiteSpace(Autonomous.Text)) {
					await DisplayAlert("Missing Fields", "Please enter in a value for Drivetrain and Autonomous Strategy.", "OK");
					return;
				}

				var driverEx = ExperienceDict[ExpKeys[DriverEx.SelectedIndex]];
				var coDriverEx = ExperienceDict[ExpKeys[CoDriverEx.SelectedIndex]];
				var coachEx = ExperienceDict[ExpKeys[CoachEx.SelectedIndex]];
				var humanPlayerEx = ExperienceDict[ExpKeys[HumanPlayerEx.SelectedIndex]];

				var robotSpeed = double.Parse(RobotSpeed.Text);
				var climbSpeed = double.Parse(ClimbSpeed.Text);
				var expectedGears = double.Parse(ExpectedGears.Text);
				var expectedHighGoals = double.Parse(ExpectedHighGoals.Text);

				var autonomous = Autonomous.Text;
				var drivetrain = Drivetrain.Text;

				// Grab or Generate existing scout sheet
				var ds = DbContext.Instance.GetOrGenerateDataShit(team.TeamNumber);

				var datasheet = new DataSheet() {
					DirtyBoy = true,
					Id = ds.Id,
					Notes = ds.Notes,
					Autonomous = autonomous,
					ClimbSpeed = climbSpeed,
					CoachEx = coachEx,
					CoDriverEx = coDriverEx,
					DriverEx = driverEx,
					Drivetrain = drivetrain,
					ExpectedBalls = expectedHighGoals,
					ExpectedGears = expectedGears,
					HumanPlayer = humanPlayerEx,
					RobotSpeed = robotSpeed,
					Team = team,
					TeamNumber = team.TeamNumber,
					Year = DateTime.Now.Year
				};
				DbContext.Instance.Db.UpdateWithChildren(datasheet);
				await Navigation.PopAsync();
			}));
		}
	}
}
