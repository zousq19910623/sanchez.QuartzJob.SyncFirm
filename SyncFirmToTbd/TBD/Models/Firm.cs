using System;
using System.Collections.Generic;
using System.Text;

namespace SyncFirmToTbd.TBD.Models
{
    public class Firm
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public int Location { get; set; }

        public int Industry { get; set; }

        public int Status { get; set; }
    }
}
