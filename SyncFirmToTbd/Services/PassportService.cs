using Baza.ComponentModel;
using Baza.TBD.Common.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SyncFirmToTbd.Config;
using System;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace SyncFirmToTbd.Services
{
    public class PassportService
    {
        protected readonly ILogger m_Logger;
        private PassportOption m_PassportOption;


        public PassportService(ILogger<PassportService> logger, IConfiguration config)
        {
            m_Logger = logger;
            m_PassportOption = new PassportOption();
            var section = config.GetSection("PassportOption");
            section.Bind(m_PassportOption);
        }

        protected HttpClient GetClient(HttpClientHandler handler)
        {
            handler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;
            handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            var httpClient = new HttpClient(handler) { BaseAddress = new Uri(m_PassportOption.Url) };
            httpClient.DefaultRequestHeaders.Add("Auth",
                $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{m_PassportOption.UserName}:{m_PassportOption.Password}"))}");
            return httpClient;
        }
        
        public async Task<InvokedResult> UpdateUserFirmId(string openId, string firmId)
        {
            Requires.NotNullOrEmpty(openId, nameof(openId));
            Requires.NotNullOrEmpty(firmId, nameof(firmId));

            try
            {
                using (var handler = new HttpClientHandler())
                {
                    using (var httpClient = GetClient(handler))
                    {
                        var postContent = new StringContent(JsonConvert.SerializeObject(new {openId, firmId}),
                            Encoding.UTF8, "application/json");

                        var response = await httpClient.PostAsync("passport/UnionId/UpdateFirmId", postContent);
                        var result =
                            JsonConvert.DeserializeObject<WebApiResponse<string>>(await response.Content
                                .ReadAsStringAsync());
                        return result.Success ? InvokedResult.SucceededResult : throw new Exception(result.Error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                m_Logger.LogError($"更新用户公司Id失败，userOpenId: {openId}：{ex.Message}");
                return InvokedResult.Fail("UpdateFirmIdFailed", $"更新用户FirmId至Passport失败，userOpenId: {openId}");
            }
        }
    }
}
