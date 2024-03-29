﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlueAllianceClient;
using SQLite.Net;
using SQLiteNetExtensions.Extensions;
using Xamarin.Forms;

namespace Scouty
{
	public class DbContext
	{
		public static readonly string DefaultDatabase = "db.sqlite";
		private static readonly DbContext _instance = new DbContext();
		public static DbContext Instance => _instance;
		public SQLiteConnection Db { get; private set; }

		private DbContext() { }

		public void InitalizeDb(string dbPath)
		{
			Db = new SQLiteConnection(DependencyService.Get<ISQLPlatformHelper>().Platform, DependencyService.Get<ISQLPlatformHelper>().GetConnectionString(dbPath));
			Db.CreateTable(typeof(Team));
			Db.CreateTable(typeof(Event));
			Db.CreateTable(typeof(TeamEvent));
			Db.CreateTable(typeof(Match));
			Db.CreateTable(typeof(Performance));
			Db.CreateTable(typeof(RobotEvent));
			Db.CreateTable(typeof(DataSheet));
			Db.CreateTable(typeof(Note));

		}

		public T TryGetWithChildren<T>(object pk, bool recursive= false) where T : class
		{
			try
			{
				return Db.GetWithChildren<T>(pk, recursive);
			}
			catch (InvalidOperationException e) {
				System.Diagnostics.Debug.WriteLine(e.ToString());
				return default(T);
			}
		}

		public DataSheet GetOrGenerateDataShit(int teamNumber) {
			DataSheet ds;
			if (!Db.Table<DataSheet>().Any(a => a.TeamNumber == teamNumber))
			{
				ds = new DataSheet()
				{
					TeamNumber = teamNumber,
					Year = DateTime.Now.Year,
					Id = Guid.NewGuid().ToString(),
					Notes = new List<Note>(),
					DirtyBoy = false
				};
				Db.Insert(ds);
			}
			else {
				ds = Db.GetAllWithChildren<DataSheet>(x => x.TeamNumber == teamNumber).First();
				if (ds.Notes == null)
					ds.Notes = new List<Note>();
			}

			return ds;
		}

		/// <summary>
		/// Inserts/Updates the list of events
		/// </summary>
		/// <param name="events">Events to add/update in database.</param>
		public void InsertOrUpdateEvents(IEnumerable<BAEvent> events)
		{
			Db.InsertOrIgnoreAll(events.Select(x => x.FromBAEvent()));
		}

		/// <summary>
		/// Inserts/update event from Blue Alliance.
		/// </summary>
		/// <param name="e">Event.</param>
		public void InsertOrUpdateEvent(BAEvent e)
		{
			// Put new teams in Database
			var eventTeams = e.Teams.Select(x => x.FromBATeam()).ToList();

			Db.InsertOrIgnoreAll(eventTeams);

			var ev = e.FromBAEvent();
			ev.Teams.AddRange(eventTeams);

			Db.InsertOrReplaceWithChildren(ev);

			// Find what the differences are
			var newMatches = e.Matches
							  .Select(x => new { DbMatch = x.FromBAMatch(ev), Match = x })
							  .ToList();

			// Now add matches
			var eventMatches = e.Matches.Select(x => x.FromBAMatch(ev)).ToList();
			Db.InsertOrIgnoreAll(eventMatches);
			
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

			Db.InsertOrIgnoreAll(newPerformances);
		}
	}
}
