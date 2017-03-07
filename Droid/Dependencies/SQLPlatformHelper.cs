using System;
using System.IO;
using Scouty;
using Scouty.Droid;
using SQLite.Net;
using SQLite.Net.Interop;
using SQLite.Net.Platform.XamarinAndroid;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLPlatformHelper))]
namespace Scouty.Droid
{
	public class SQLPlatformHelper : ISQLPlatformHelper
	{
		public ISQLitePlatform Platform => new SQLitePlatformAndroid();

		public bool DropDatabase(string dbPath)
		{
			var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			File.Delete(Path.Combine(path, dbPath));
		}

		public SQLiteConnectionString GetConnectionString(string dbPath)
		{
			var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			return new SQLiteConnectionString(Path.Combine(path, dbPath), true);
		}
	}
}
