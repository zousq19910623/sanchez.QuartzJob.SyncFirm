using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SyncFirmToTbd.Repository
{
    public class BaseRepository
    {
        protected string m_Connection;
        protected ILogger m_Logger;

        public BaseRepository(IConfiguration config, ILogger<FirmRepository> logger)
        {
            m_Logger = logger;
            m_Connection = config["ConnectionStrings:BazaAts"];
        }
    }
}
