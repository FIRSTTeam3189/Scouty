using System;
namespace Scouty
{
	public class Token
	{
		public string Username { get; set; }
		public string AccessToken { get; set; }
		public DateTime ExpiresOn { get; set; }
		public string TokenType { get; set; }
		public Token()
		{
		}
	}
}
