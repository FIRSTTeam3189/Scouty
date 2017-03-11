using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SQLiteNetExtensions.Extensions;
using Xamarin.Forms;

namespace Scouty
{
	public partial class TeamSelectPage : ContentPage
	{
		public ObservableCollection<Alliance> Alliances { get; set; }
		public Match SelectedMatch { get; }
		public TeamSelectPage(Match match)
		{
			InitializeComponent();
			SelectedMatch = match;
			Alliances = new ObservableCollection<Alliance>();

			Teams.ItemsSource = Alliances;
			Teams.ItemSelected += Teams_ItemSelected;
		}

		void Teams_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			var item = e.SelectedItem;
			if (item != null) {
				var team = item as Team;

				Navigation.PushAsync(new GradePage(SelectedMatch, team));
			}
		}

		protected override void OnAppearing()
		{
			Task.Run(() => {
				var db = DbContext.Instance.Db;

				var performances = db.GetAllWithChildren<Performance>(x => x.MatchId == SelectedMatch.MatchId);
				var alliances = Alliance.FromPerformances(performances);

				Device.BeginInvokeOnMainThread(() => {
					foreach (var a in alliances)
						Alliances.Add(a);
				});
			});
			base.OnAppearing();
		}

		public class Alliance : ObservableCollection<Team> 
		{
			public static IEnumerable<Alliance> FromPerformances(IEnumerable<Performance> performances) {
				
				return (from p in performances
						group p by p.Color into g
				        orderby g.Key
				        select new Alliance(g.Select(x => x.Team), g.Key));
			}
			public AllianceColor AllianceColor { get; }
			public string Name => AllianceColor == Scouty.AllianceColor.Blue ? "Blue" : "Red";
			public Color HeaderColor => AllianceColor == Scouty.AllianceColor.Blue ? Color.FromHex("#9494ff") : Color.FromHex("#ff9494");

			public Alliance(IEnumerable<Team> teams, AllianceColor color) {
				foreach (var t in teams)
					Add(t);

				AllianceColor = color;
			}
		}
	}
}
