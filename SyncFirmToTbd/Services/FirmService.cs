using Baza.ComponentModel;
using Microsoft.Extensions.Logging;
using SyncFirmToTbd.Repository;
using SyncFirmToTbd.TBD.Models;
using SyncFirmToTbd.TBD.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyncFirmToTbd.Services
{


    public class FirmService
    {
        private FirmRepository m_FirmRepository;
        private TbdFirmService m_TbdFirmService;
        private readonly ILogger m_Logger;

        public FirmService(FirmRepository firmRepository, TbdFirmService tbdFirmService, ILogger<FirmService> logger)
        {
            m_FirmRepository = firmRepository;
            m_TbdFirmService = tbdFirmService;
            m_Logger = logger;
        }

        /// <summary>
        /// 获取一个月内新增的雇主
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Employer>> GetNewEmployersInMouthAsync()
        {
            var nowDate = DateTime.Now;
            var oneMonthAgeDate = nowDate.AddMonths(-1);
            var employers = await m_FirmRepository.GetEmployersBetweenDateAsync(oneMonthAgeDate, nowDate);
            m_Logger.LogInformation($"获取一个月内新增的雇主，开始时间为：{oneMonthAgeDate}，结束时间为：{nowDate}");
            return employers;
        }

        public async Task<InvokedResult> PutNewEmployerToTbdAsync()
        {
            var employers = await GetNewEmployersInMouthAsync();
            var ids = employers.Select(e => e.Id).ToList();
            if (ids.Count <= 0)
            {
                m_Logger.LogInformation("没有需要同步的公司");
                return InvokedResult.SucceededResult;
            }
            var emploperInfos = await GetEmployersInfoAsync(ids);
            foreach (var employerInfo in emploperInfos)
            {
                var firm = new Firm
                {
                    Id = employerInfo.Id.ToString(),
                    Industry = employerInfo.Industry,
                    Location = employerInfo.Location,
                    Name = employerInfo.Name,
                    ShortName = string.IsNullOrEmpty(employerInfo.ShortName) ? employerInfo.Name : employerInfo.ShortName,
                    Status = employerInfo.Status
                };
                try
                {
                    var result = await m_TbdFirmService.CreateFirmAsync(firm);
                    var updateResult = await m_FirmRepository.UpdateTbdFirmIdAsync(employerInfo.Id, result.Data);
                    if (updateResult)
                    {
                        m_Logger.LogInformation($"同步公司成功,id为{result.Data}");
                    }
                }
                catch (Exception ex)
                {
                    m_Logger.LogInformation(ex.Message);
                }
            }
            return InvokedResult.SucceededResult;
        }

        private async Task<IEnumerable<EmployerInfo>> GetEmployersInfoAsync(IEnumerable<int> ids)
        {
            return await m_FirmRepository.GetEmployersInfoByIdsAsync(ids);
        }
    }
}