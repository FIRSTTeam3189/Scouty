using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SQLiteNetExtensions.Extensions;
using Xamarin.Forms;

namespace Scouty
{
	public partial class MyGradesPage : ContentPage
	{
		ObservableCollection<Grade> Grades { get; }
		Event CurrentEvent { get; }
		public MyGradesPage(Event ev)
		{
			InitializeComponent();
			Grades = new ObservableCollection<Grade>();
			CurrentEvent = ev;
			Records.ItemsSource = Grades;
			Records.ItemSelected += Records_ItemSelected;
		}


		protected override void OnAppearing()
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				await Task.Run(() =>
				{
					// Get all of the Robot events from the server
					var db = DbContext.Instance.Db;
					var evMatches = CurrentEvent.Matches.Select(x => x.MatchId).ToList();
					var allEvents = db.Table<RobotEvent>().ToList().Where(x => evMatches.Contains(x.MatchId));
					var notes = db.Table<Note>().ToList().Where(x => evMatches.Contains(x.MatchId)).ToDictionary(x => x.MatchId, x => x.Data);

					// Group them into grades now
					var grades = from x in allEvents
								 group x by x.MatchId into g
								 select new Grade
								 {
									 Name = g.Key.Substring(CurrentEvent.EventId.Length + 1).ToUpper(),
									 TeamNumber = g.First().TeamId,
									 Note = notes[g.Key]
								 };

					Device.BeginInvokeOnMainThread(() =>
					{
						foreach (var g in grades)
							Grades.Add(g);
					});
				});
			});
			base.OnAppearing();
		}

		async void Records_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			var grade = e.SelectedItem as Grade;
			if (grade != null) {
				Records.SelectedItem = null;
				await DisplayAlert($"Team {grade.TeamNumber} {grade.Name}'s Note", $"{grade.Note}", "OK");
			}
		}
	}


	class Grade
	{
		public string Name { get; set; }
		public int TeamNumber { get; set; }
		public string Note { get; set; }
	}
}
