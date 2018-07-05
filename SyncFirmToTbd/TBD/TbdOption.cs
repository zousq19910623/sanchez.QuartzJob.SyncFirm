using Baza.TBD.OpenAPI.Client;

namespace SyncFirmToTbd.TBD
{
    public class TbdOption
    {
        public TBDServiceOption TbdServiceOption { get; set; }

        public TbdOpenApi TbdOpenApi { get; set; }
    }

    public class TbdOpenApi
    {
        public string Url { get; set; }

        public string SourceChannel { get; set; }
    }
}
