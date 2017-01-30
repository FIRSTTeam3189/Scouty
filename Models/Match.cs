using System;
using System.Collections.Generic;
using System.Text;

namespace Scouty.Models
{
    class Match
    {
        public int Number
        {
            get;
            set;
        }
        public MatchType Type
        {
            get;
            set;
        }
        public List<Team> RedTeams
        {
            get;
            set;
        }
        public List<Team> BlueTeams
        {
            get;
            set;
        }
    }
    public enum MatchType
    {
        Practice, 
        Qualifiction,
        Decafinals,
        Quarterfinals,
        Semifinals,
        Finals
    }
        
}
