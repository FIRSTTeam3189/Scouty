using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlueAllianceClient;
using SQLite.Net;
using SQLite.Net.Async;
using SQLiteNetExtensionsAsync;
using SQLiteNetExtensionsAsync.Extensions;
using Xamarin.Forms;

namespace Scouty
{
	public class DbContext
	{
		public static readonly string DefaultDatabase = "db.sqlite";
		private static readonly DbContext _instance = new DbContext();
		public static DbContext Instance => _instance;
		public SQLiteAsyncConnection Db { get; private set; }

		private DbContext() { }

		public async Task InitalizeDb(string dbPath)
		{
			Db = new SQLiteAsyncConnection(
				() => new SQLiteConnectionWithLock(DependencyService.Get<ISQLPlatformHelper>().Platform,
												   DependencyService.Get<ISQLPlatformHelper>().GetConnectionString(dbPath)));
			await Db.CreateTablesAsync(typeof(Event), typeof(Team), typeof(TeamEvent), typeof(Match), typeof(Performance), typeof(Performance));
		}

		public async Task<T> TryGetWithChildrenAsync<T>(object pk, bool recursive= false) where T : class
		{
			try
			{
				return await Db.GetWithChildrenAsync<T>(pk, recursive);
			}
			catch (InvalidOperationException e) {
				System.Diagnostics.Debug.WriteLine(e.ToString());
				return default(T);
			}
		}

		/// <summary>
		/// Inserts/Updates the list of events
		/// </summary>
		/// <param name="events">Events to add/update in database.</param>
		public async Task InsertOrUpdateEvents(IEnumerable<BAEvent> events)
		{
			foreach (var e in events)
				await InsertOrUpdateEvent(e);
		}

		/// <summary>
		/// Inserts/update event from Blue Alliance.
		/// </summary>
		/// <param name="e">Event.</param>
		public async Task InsertOrUpdateEvent(BAEvent e)
		{
			// Figure out if all teams are in the Database
			var existingTeams = await Db.Table<Team>().ToListAsync();
			var newTeams = e.Teams
							.Where(x => !existingTeams.Select(y => y.TeamNumber).Contains(x.TeamNumber))
							.Select(x => x.FromBATeam());

			// Put new teams in Database
			await Db.InsertAllAsync(newTeams);

			// Figure out if event already exists in Database
			var ev = await TryGetWithChildrenAsync<Event>(e.Key);
			if (ev == null)
			{
				// Add BAEvent to Database
				ev = e.FromBAEvent();
				await Db.InsertAsync(ev);
				ev = await Db.Table<Event>().Where(x => x.EventId == e.Key).FirstAsync();
			}

			// Get all of the teams existing now
			existingTeams = await Db.Table<Team>().ToListAsync();

			if (ev.Teams == null)
				ev.Teams = new List<Team>();

			// Add all of the teams that are going to the event
			var evTeams = existingTeams
				.Where(x => e.Teams.Select(y => y.TeamNumber).Contains(x.TeamNumber)
				       && !ev.Teams.Select(y => y.TeamNumber).Contains(x.TeamNumber));
			foreach (var team in evTeams)
			{
				ev.Teams.Add(team);
			}

			await Db.UpdateWithChildrenAsync(ev);

			// Find what the differences are
			var existingMatches = await Db.Table<Match>()
										  .Where(x => x.EventId == ev.EventId)
										  .ToListAsync();
			var newMatches = e.Matches
							  .Where(x => !existingMatches
									 .Select(y => y.MatchInfo)
									 .Contains(x.MatchInfo()))
							  .Select(x => new { DbMatch = x.FromBAMatch(ev), Match = x })
							  .ToList();

			// Now add matches
			await Db.InsertAllAsync(newMatches.Select(x => x.DbMatch));

			// Get all of the new ids of the matches now
			var matches = await Db.Table<Match>().Where(x => x.EventId == ev.EventId).ToListAsync();
			newMatches = (from m in matches
						  join o in newMatches on m.MatchInfo equals o.DbMatch.MatchInfo
						  select new { DbMatch = m, o.Match }).ToList();
			
			var newPerformances = new List<Performance>();

			// Now Generate all of the performances
			foreach (var match in newMatches)
			{
				var redTeams = (from t in match.Match.Red
								join dbT in existingTeams on t.TeamNumber equals dbT.TeamNumber
								select dbT);
				var blueTeams = (from t in match.Match.Blue
								 join dbT in existingTeams on t.TeamNumber equals dbT.TeamNumber
								 select dbT);
				
				match.DbMatch.Event = ev;

				newPerformances.AddRange(blueTeams.Select(x => match.DbMatch.PerformanceFromMatch(x, AllianceColor.Blue))
				                         .Union(redTeams.Select(x => match.DbMatch.PerformanceFromMatch(x, AllianceColor.Red))));
			}

			await Db.InsertAllAsync(newPerformances);

		}
	}
}
