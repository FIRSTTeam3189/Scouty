using System;
using SQLite.Net;
using SQLite.Net.Interop;

namespace Scouty
{
	public interface ISQLPlatformHelper
	{
		ISQLitePlatform Platform { get; }
		string GetConnectionString(string dbPath);
		bool DropDatabase(string dbPath);
	}
}
