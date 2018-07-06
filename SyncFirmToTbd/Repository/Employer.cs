using System.ComponentModel.DataAnnotations.Schema;

namespace SyncFirmToTbd.Repository
{
    public class Employer
    {
        public int Id { get; set; }

        public int Status { get; set; }

        [Column("tbd_firm_id")]
        public string TbdFirmId { get; set; }
    }

    public class EmployerInfo
    {
        public int Id { get; set; }

        public int Status { get; set; }
        
        public int Location { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public int Industry { get; set; }
    }
}
