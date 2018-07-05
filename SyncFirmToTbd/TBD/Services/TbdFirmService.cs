using Baza.ComponentModel;
//using Baza.Infrastructure.Configuration;
using Baza.TBD.Common.Web;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SyncFirmToTbd.TBD.Models;
using System;
using System.Threading.Tasks;
using Baza.TBD.OpenAPI.Client;

namespace SyncFirmToTbd.TBD.Services
{
    public class TbdFirmService
    {
        protected string m_OpenApiUrl;
        protected string m_SourceChannel;
        protected TbdClient m_TbdClient;

        public TbdFirmService(IConfiguration config)
        {
            var section = config.GetSection("TbdOption");
            var tbdOptions = new TbdOption();
            section.Bind(tbdOptions);

            m_OpenApiUrl = tbdOptions.TbdOpenApi?.Url;
            m_SourceChannel = tbdOptions.TbdOpenApi?.SourceChannel; ;
            var authServer = tbdOptions.TbdServiceOption?.AuthServer;
            var clientId = tbdOptions.TbdServiceOption?.ClientId;
            var clientSecret = tbdOptions.TbdServiceOption?.ClientSecret;
            var scope = tbdOptions.TbdServiceOption?.Scope;
            var cache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
            m_TbdClient = new TbdClient(authServer, clientId, clientSecret, scope, cache);
        }

        public async Task<InvokedResult<string>> CreateFirmAsync(Firm firm)
        {
            var responseBody = await m_TbdClient.PostAsync($"{m_OpenApiUrl}Firms", new
            {
                name = firm.Name,
                shortName = string.IsNullOrEmpty(firm.ShortName) ? firm.Name : firm.ShortName,
                sourceChannel = m_SourceChannel,
                sourceId = firm.Id,
                location = firm.Location,
                industry = firm.Industry,
                identityType = 1,
                status = firm.Status
            });

            var result = JsonConvert.DeserializeObject<WebApiResponse<string>>(responseBody);

            if (!result.Success)
                throw new Exception($"创建公司出错：{result.Error.Description}");

            return InvokedResult.Ok(result.Data);
        }

        //public async Task<InvokedResult> UpdateFirmAsync(Firm firm)
        //{
        //    var tdbFirm = DbContext.Set<TBDFirm>().FirstOrDefault(f => f.FirmId == firm.Id);
        //    if (tdbFirm == null)
        //        return InvokedResult.Fail("FirmNotFind", "没找到这个公司");

        //    var url = $"{m_OpenApiUrl}Firms/{tdbFirm.TBDObjectId}";
        //    //var url = $"{m_OpenApiUrl}Firms/";
        //    var responseBody = await m_TbdClient.PutAsync(url, new
        //    {
        //        name = firm.Name,
        //        shortName = string.IsNullOrEmpty(firm.ShortName) ? firm.Name : firm.ShortName,
        //        sourceChannel = m_SourceChannel,
        //        sourceId = firm.Id,
        //        location = firm.Location,
        //        industry = firm.Industry,
        //        identityType = 1,
        //        status = firm.Status
        //    });

        //    var result = JsonConvert.DeserializeObject<WebApiResponse>(responseBody);
        //    if (!result.Success)
        //        throw new Exception($"更新公司出错：{result.Error.Description}");

        //    return InvokedResult.SucceededResult;
        //}
    }
}