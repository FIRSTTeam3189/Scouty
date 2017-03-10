using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Threading.Tasks;
using SQLiteNetExtensionsAsync.Extensions;
using BlueAllianceClient;

namespace Scouty
{
	public partial class MatchesPage : ContentPage
	{
		ObservableCollection<MatchGroup> MatchList { get; set; }
		BlueAllianceContext _blueContext = new BlueAllianceContext();
		Event MatchEvent { get; }
		public MatchesPage(Event ev)
		{
			InitializeComponent();
			MatchEvent = ev;
			MatchList = new ObservableCollection<MatchGroup>();
			Matches.ItemsSource = MatchList;
		}

		protected override void OnAppearing()
		{
			if (MatchList.Count == 0)
				Task.Run(async () =>
				{
					var ev = MatchEvent;
					var db = DbContext.Instance.Db;
					if (await db.Table<Match>().Where(x => x.EventId == ev.EventId).CountAsync() == 0)
					{
						// Pull the latest from BlueAlliance
						var trueEvent = await _blueContext.GetEvent(2017, ev.EventId.Substring(4));

						await DbContext.Instance.InsertOrUpdateEvent(trueEvent);
					}


					// Now get the matches from that event
					var m = (await db.GetAllWithChildrenAsync<Match>(x => x.EventId == ev.EventId)).ToList();

					var matchGroups = MatchGroup.FromMatches(m);
					Device.BeginInvokeOnMainThread(() =>
					{
						foreach (var g in matchGroups)
							MatchList.Add(g);
					});
				});
			base.OnAppearing();
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

		public static ObservableCollection<MatchGroup> FromMatches(IEnumerable<Match> matches) {
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
