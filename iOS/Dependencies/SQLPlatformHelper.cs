using System;
using System.IO;
using Scouty;
using Scouty.iOS;
using SQLite.Net;
using SQLite.Net.Interop;
using SQLite.Net.Platform.XamarinIOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLPlatformHelper))]
namespace Scouty.iOS
{
	public class SQLPlatformHelper : ISQLPlatformHelper
	{
		public ISQLitePlatform Platform => new SQLitePlatformIOS();

		public bool DropDatabase(string dbPath)
		{
			var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			var libFolder = Path.Combine(docFolder, "..", "Library", "Databases");

			if (!Directory.Exists(libFolder))
				Directory.CreateDirectory(libFolder);

			dbPath = Path.Combine(libFolder, dbPath);
			File.Delete(dbPath);

			return true;
		}

		public string GetConnectionString(string dbPath)
		{
			var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			var libFolder = Path.Combine(docFolder, "..", "Library", "Databases");

			if (!Directory.Exists(libFolder))
				Directory.CreateDirectory(libFolder);

			dbPath = Path.Combine(libFolder, dbPath);
			return dbPath;
		}
	}
}
