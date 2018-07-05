using Baza.ComponentModel;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncFirmToTbd.Repository
{
    public class FirmRepository
    {
        private readonly string m_FirmConnection;
        private readonly ILogger m_Logger;

        public FirmRepository(IConfiguration config, ILogger<FirmRepository> logger)
        {
            m_FirmConnection = config["ConnectionStrings:BazaAts"];
            m_Logger = logger;
        }

        public async Task<IEnumerable<Employer>> GetEmployersBetweenDateAsync(DateTime startDate, DateTime endDate)
        {
            Requires.NotNullOrEmpty(startDate.ToString(CultureInfo.InvariantCulture), nameof(startDate));
            Requires.NotNullOrEmpty(endDate.ToString(CultureInfo.InvariantCulture), nameof(endDate));

            using (var conn = new SqlConnection(m_FirmConnection))
            {
                conn.Open();

                const string sql = "select id,status,tbd_firm_id as tbdFirmId from t_employer where tbd_firm_id is null and start_date between @startDate and @endDate";
                var employers = await conn.QueryAsync<Employer>(sql, new { startDate, endDate });
                m_Logger.LogInformation($"sql：{sql}，开始时间为：{startDate}，结束时间为：{endDate}");
                return employers;
            }
        }

        public async Task<IEnumerable<EmployerInfo>> GetEmployerInfoByIdsAsync(IEnumerable<int> ids)
        {
            Requires.NotNull(ids, nameof(ids));

            using (var conn = new SqlConnection(m_FirmConnection))
            {
                conn.Open();

                const string sql = @"SELECT e.id,e.status,eb.cn_name as name,eb.cn_name_abbr as shortName,eb.company_industry as industry,ec.location FROM t_employer e
                                     INNER JOIN BazaAts.dbo.t_employer_basic eb ON e.id=eb.employer_id
                                     INNER JOIN BazaAts.dbo.t_employer_contact ec ON e.id=ec.employer_id
                                     Where e.id in @ids";
                var employers = await conn.QueryAsync<EmployerInfo>(sql, new { ids });
                var idList = new StringBuilder();
                foreach (var id in ids)
                {
                    idList.Append(id + ",");
                }
                m_Logger.LogInformation($"sql执行语句为：{sql}，id为：{idList}");
                return employers;
            }
        }

        public async Task<bool> UpdateTbdFirmIdAsync(int id, string tbdFirmId)
        {
            using (var conn=new SqlConnection(m_FirmConnection))
            {
                conn.Open();

                const string sql = "update t_employer set tbd_firm_id=@tbdFirmId where id=@id";
                var result = await conn.ExecuteAsync(sql, new {id, tbdFirmId});
                return result > 0;
            }
        }
    }
}