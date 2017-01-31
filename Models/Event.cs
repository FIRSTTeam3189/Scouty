using System;
using System.Collections.Generic;
using System.Text;

namespace Scouty.Models
{
    class Event
    {
        public List<Team> Teams { get; set; }
        public string EventName { get; set; }
        public Location Location { get; set; }
        public List<Match> Matches { get; set; }
    }

    public struct Location {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
