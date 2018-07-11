using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using SyncFirmToTbd.Models;

namespace SyncFirmToTbd.Repository
{
    public class UserRepository : BaseRepository
    {
        public UserRepository(IConfiguration config, ILogger<FirmRepository> logger) : base(config, logger)
        {
        }

        public async Task<IEnumerable<User>> GetUsersInDateByEmployerIdAsync(int employerId, DateTime startDate, DateTime endDate)
        {
            using (var conn = new SqlConnection(m_Connection))
            {
                conn.Open();

                const string sql =
                    "select id as Id,open_id as OpenId from t_usyuser where employer_id=@employerId and created_date between @startDate and @endDate";
                var users = await conn.QueryAsync<User>(sql, new { employerId, startDate, endDate });
                m_Logger.LogInformation($"sql执行语句为：{sql}，id为：{employerId}，开始时间为：{startDate}，结束时间为：{endDate}");
                return users;
            }
        }
    }
}
