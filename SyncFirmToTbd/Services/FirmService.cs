﻿using Baza.ComponentModel;
using Microsoft.Extensions.Logging;
using SyncFirmToTbd.Repository;
using SyncFirmToTbd.TBD.Models;
using SyncFirmToTbd.TBD.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SyncFirmToTbd.Config;
using SyncFirmToTbd.Models;

namespace SyncFirmToTbd.Services
{


    public class FirmService
    {
        private FirmRepository m_FirmRepository;
        private UserRepository m_UserRepository;
        private TbdFirmService m_TbdFirmService;
        private PassportService m_PassportService;
        private TimingOption m_TimingOption;
        private readonly ILogger m_Logger;

        public FirmService(FirmRepository firmRepository, UserRepository userRepository, TbdFirmService tbdFirmService,
            PassportService passportService, ILogger<FirmService> logger, IConfiguration config)
        {
            m_FirmRepository = firmRepository;
            m_UserRepository = userRepository;
            m_TbdFirmService = tbdFirmService;
            m_PassportService = passportService;
            m_Logger = logger;
            m_TimingOption = new TimingOption();
            var section = config.GetSection("TimingOption");
            section.Bind(m_TimingOption);
        }

        /// <summary>
        /// 获取指定时间内新增的雇主,默认查询时间为1天
        /// </summary>
        /// <param name="days">天数，默认为1，表示1天内</param>
        /// <param name="mouths">月数，默认为0</param>
        /// <returns></returns>
        public async Task<IEnumerable<Employer>> GetNewEmployersAsync(int days = 1, int mouths = 0)
        {
            var nowDate = DateTime.Now;
            var fromDate = nowDate.AddDays(-days).AddMonths(-mouths);
            var employers = await m_FirmRepository.GetEmployersBetweenDateAsync(fromDate, nowDate);
            m_Logger.LogInformation($"获取{nowDate.Subtract(fromDate).Days}天内新增的雇主，开始时间为：{fromDate}，结束时间为：{nowDate}");
            return employers;
        }

        public async Task<InvokedResult> PutNewEmployerToTbdAsync()
        {
            SetTimingOption(m_TimingOption);
            var employers = await GetNewEmployersAsync(m_TimingOption.Days, m_TimingOption.Mouths);
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

                        var nowDate = DateTime.Now;
                        var fromDate = nowDate.AddDays(-m_TimingOption.Days).AddMonths(-m_TimingOption.Mouths);
                        var users = await m_UserRepository.GetUsersInDateByEmployerIdAsync(employerInfo.Id, fromDate, nowDate);
                        foreach (var user in users)
                        {
                            var updateFirmIdResult = await m_PassportService.UpdateUserFirmId(user.OpenId, result.Data);
                            if (updateFirmIdResult.Succeeded)
                            {
                                m_Logger.LogInformation($"更新用户公司id成功,user id为{user.Id}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    m_Logger.LogInformation(ex.Message);
                }
            }
            return InvokedResult.SucceededResult;
        }

        private void SetTimingOption(TimingOption timingOption)
        {
            if (timingOption == null)
            {
                m_TimingOption = new TimingOption { Days = 1, Mouths = 0 };
            }
        }

        private async Task<IEnumerable<EmployerInfo>> GetEmployersInfoAsync(IEnumerable<int> ids)
        {
            return await m_FirmRepository.GetEmployersInfoByIdsAsync(ids);
        }
    }
}