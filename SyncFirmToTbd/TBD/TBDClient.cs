using Baza.ComponentModel;
using Baza.TBD.OpenAPI.Client;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SyncFirmToTbd.TBD
{
    public class TbdClient
    {
        private readonly IMemoryCache m_MemoryCache;

        TBDService m_TBDService;
        readonly string m_AccessTokenCacheKey = "TbdAccessTokenCache";

        public TbdClient(string authServer, string clientId, string clientSerect, string scope, IMemoryCache memoryCache)
        {
            var serviceOption = new TBDServiceOption
            {
                AuthServer = authServer,
                ClientId = clientId,
                ClientSecret = clientSerect,
                Scope = scope
            };
            m_TBDService = new TBDService(serviceOption);
            m_MemoryCache = memoryCache;
        }
        public async Task<string> PostAsync(string url, object data)
        {
            return await m_TBDService.PostAsync(await GetAccessToken(), url, data);
        }

        public async Task<string> PutAsync(string url, object data)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(await GetAccessToken());

                var postContent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var response = await client.PutAsync(url, postContent);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(response.StatusCode.ToString());
                }
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> GetAsync(string url)
        {
            Requires.NotNullOrEmpty(url, nameof(url));

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.SetBearerToken(await GetAccessToken());
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                throw new HttpRequestException(response.StatusCode.ToString());
            }
        }

        private async Task<string> GetAccessToken()
        {
            if (m_MemoryCache.TryGetValue(m_AccessTokenCacheKey, out string accessToken))
            {
                return accessToken;
            }

            var response = await m_TBDService.GetTokenResponseAsync();
            if (!string.IsNullOrEmpty(response.Error))
            {
                throw new Exception("获取accessToken失败");
            }

            m_MemoryCache.Set(m_AccessTokenCacheKey, response.AccessToken, DateTimeOffset.Now.AddHours(1));
            return response.AccessToken;
        }
    }
}