using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlueAllianceClient
{
	public class BAMatch
	{
		public string Key { get; set; }

		public string Level { get; set; }
		public int? SetNumber { get; set; }
		public int MatchNumber { get; set; }
		public string TimeString { get; set; }

		public List<BATeam> Red { get; set; }
		public List<BATeam> Blue { get; set; }
		public BAEvent Event { get; set; }
	}
}