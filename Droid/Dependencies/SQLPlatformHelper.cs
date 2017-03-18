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
		public ISQLitePlatform Platform {
			get {
				return new SQLitePlatformAndroid();
			}
		}

		public bool DropDatabase(string dbPath)
		{
			var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			File.Delete(Path.Combine(path, dbPath));
			return true;
		}

		public string GetConnectionString(string dbPath)
		{
			var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			return Path.Combine(path, dbPath);
		}
	}
}
