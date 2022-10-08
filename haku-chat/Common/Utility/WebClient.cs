using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using NLog;
using NLog.Web;

namespace haku_chat.Common.Utility
{
    public static class WebClient
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static Logger logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        /// <summary>
        /// HTTPクライアント
        /// </summary>
        private static HttpClient Client = new HttpClient();

        /// <summary>
        /// GETリクエスト
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async static Task GetAsync(string url)
        {
            try
            {
                var resource = await Client.GetAsync(url);
                resource.EnsureSuccessStatusCode();
                var responseBody = await resource.Content.ReadAsStringAsync();
                logger.Debug(responseBody);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// POSTリクエスト
        /// </summary>
        /// <param name="url"></param>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public async static Task PostAsync(string url, string jsonString)
        {
            try
            {
                var data = new StringContent(jsonString, Encoding.UTF8, mediaType: "application/json");
                var response = await Client.PostAsync(url, data);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                logger.Debug(responseBody);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
