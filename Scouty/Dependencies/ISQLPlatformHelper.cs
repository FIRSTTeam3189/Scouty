using System;
using SQLite.Net;
using SQLite.Net.Interop;

namespace Scouty
{
	public interface ISQLPlatformHelper
	{
		ISQLitePlatform Platform { get; }
		SQLiteConnectionString GetConnectionString(string dbPath);
		bool DropDatabase(string dbPath);
	}
}
