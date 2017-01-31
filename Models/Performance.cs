using System;
using System.Collections.Generic;
using System.Text;

namespace Scouty.Models
{
    class Performance
    {
        public Team Team { get; set; }
        public Match Match { get; set; }
        public int AutoHighShotsFired { get; set; }
        public int AutoLowShotsFired { get; set; }
        public int AutoHighShotsMade { get; set; }
        public int AutoLowShotsMade { get; set; }
        public int AutoGearsAttempted { get; set; }
        public int AutoGearsHung { get; set; }
        public bool TeleopIsHung { get; set; }
        public int TeleopHighShotsFired { get; set; }
        public int TeleopLowShotsFired { get; set; }
        public int TeleopHighShotsMade { get; set; }
        public int TeleopLowShotsMade { get; set; }
        public int TeleopGearsAttempted { get; set; }
        public int TeleopGearsHung { get; set; }
        public string Comment { get; set; }
    }
}
