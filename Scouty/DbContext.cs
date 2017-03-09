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
			await Db.CreateTablesAsync(typeof(Event), typeof(Team), typeof(TeamEvent), typeof(Match), typeof(Performance), typeof(RobotEvent));
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
			await Db.InsertOrIgnoreAllAsync(events.Select(x => x.FromBAEvent()));
		}

		/// <summary>
		/// Inserts/update event from Blue Alliance.
		/// </summary>
		/// <param name="e">Event.</param>
		public async Task InsertOrUpdateEvent(BAEvent e)
		{
			// Put new teams in Database
			await Db.InsertOrIgnoreAllAsync(e.Teams);

			// Figure out if event already exists in Database
			var ev = await TryGetWithChildrenAsync<Event>(e.Key);
			if (ev == null)
			{
				// Add BAEvent to Database
				ev = e.FromBAEvent();
				await Db.InsertWithChildrenAsync(ev);
				ev = await Db.GetWithChildrenAsync<Event>(ev.EventId);
			}

			await Db.UpdateWithChildrenAsync(ev);

			// Find what the differences are
			var newMatches = e.Matches
							  .Select(x => new { DbMatch = x.FromBAMatch(ev), Match = x })
							  .ToList();

			// Now add matches
			await Db.InsertOrIgnoreAllAsync(newMatches.Select(x => x.DbMatch));
			
			var newPerformances = new List<Performance>();

			// Now Generate all of the performances
			foreach (var match in newMatches)
			{
				var redTeams = (from t in match.Match.Red
				                join dbT in ev.Teams on t.TeamNumber equals dbT.TeamNumber
								select dbT);
				var blueTeams = (from t in match.Match.Blue
				                 join dbT in ev.Teams on t.TeamNumber equals dbT.TeamNumber
								 select dbT);
				
				match.DbMatch.Event = ev;

				newPerformances.AddRange(blueTeams.Select(x => match.DbMatch.PerformanceFromMatch(x, AllianceColor.Blue))
				                         .Union(redTeams.Select(x => match.DbMatch.PerformanceFromMatch(x, AllianceColor.Red))));
			}

			await Db.InsertOrIgnoreAllAsync(newPerformances);

		}
	}
}
