using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SmartQQ.Constants
{
    public static class ApiUrlExtension
    {
        /// <summary>
        ///     发送GET请求。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="url">URL。</param>
        /// <param name="args">附加的参数。</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> GetAsync(this HttpClient client, ApiUrl url, params object[] args)
            => await client.GetAsync(url, null, args);

        /// <summary>
        /// get string from http client
        /// </summary>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task<string> GetStringAsync(this HttpClient client, ApiUrl url, params object[] args)
            => await client.GetStringAsync(url, null, args);

        /// <summary>
        ///     发送GET请求。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="url">URL。</param>
        /// <param name="allowAutoRedirect">允许自动重定向。</param>
        /// <param name="args">附加的参数。</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> GetAsync(this HttpClient client, ApiUrl url, bool? allowAutoRedirect, params object[] args)
        {
            var referer = client.DefaultRequestHeaders.Referrer;
            var autoRedirect = false;// client.DefaultRequestHeaders.TryGetValue ;

            client.DefaultRequestHeaders.Add("Referer", url.Referer);
            if (allowAutoRedirect.HasValue)
                client.DefaultRequestHeaders.Add("AllowAutoRedirect",allowAutoRedirect.Value?"true":"false");
            var response = client.GetAsync(url.BuildUrl(args));

            // 复原client
            //client.Request.Referer = referer;
            client.DefaultRequestHeaders.Add("Referer", url.Referer);
            //client.Request.AllowAutoRedirect = autoRedirect;
            client.DefaultRequestHeaders.Add("AllowAutoRedirect", autoRedirect.ToString());

            return await response;
        }


        /// <summary>
        /// get string from http client
        /// </summary>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <param name="allowAutoRedirect"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task<string> GetStringAsync(this HttpClient client, ApiUrl url, bool? allowAutoRedirect, params object[] args)
        {
            var referer = client.DefaultRequestHeaders.Referrer;
            var autoRedirect = false;// client.DefaultRequestHeaders.TryGetValue ;

            client.DefaultRequestHeaders.Add("Referer", url.Referer);
            if (allowAutoRedirect.HasValue)
                client.DefaultRequestHeaders.Add("AllowAutoRedirect", allowAutoRedirect.Value ? "true" : "false");
            var response = client.GetStringAsync(url.BuildUrl(args));

            // 复原client
            //client.Request.Referer = referer;
            client.DefaultRequestHeaders.Add("Referer", url.Referer);
            //client.Request.AllowAutoRedirect = autoRedirect;
            client.DefaultRequestHeaders.Add("AllowAutoRedirect", autoRedirect.ToString());

            return await response;
        }

        /// <summary>
        ///     发送POST请求。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="url">URL。</param>
        /// <param name="json">JSON。</param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> PostAsync(this HttpClient client, ApiUrl url, JObject json) => client.PostAsync(url, json, -1);

        /// <summary>
        ///     发送POST请求。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="url">URL。</param>
        /// <param name="json">JSON。</param>
        /// <param name="timeout">超时。</param>
        /// <returns></returns>
        internal static async Task<HttpResponseMessage> PostAsync(this HttpClient client, ApiUrl url, JObject json, int timeout)
        {
            IEnumerable<string> origin;
            var hasOrigin = client.DefaultRequestHeaders.TryGetValues("Origin", out origin);
            var time = client.DefaultRequestHeaders.GetValues("Timeout").FirstOrDefault();

            client.DefaultRequestHeaders.Add("Referer" , url.Referer);
            if (client.DefaultRequestHeaders.Contains("Origin"))
                client.DefaultRequestHeaders.Add("Origin", url.Origin);
            else
                client.DefaultRequestHeaders.Add("Origin", url.Origin);
            if (timeout > 0)
                client.DefaultRequestHeaders.Add("Timeout",timeout.ToString());


            var response = client.PostAsync(url.Url,new StringContent("r=" + UrlEncoder.Default.Encode(json.ToString(Formatting.None)),Encoding.UTF8,
                "application/x-www-form-urlencoded"));

            // 复原client
            if (hasOrigin)
            {
                client.DefaultRequestHeaders.Add("Origin", origin.FirstOrDefault());
            }
            
            client.DefaultRequestHeaders.Remove("Origin");
            client.DefaultRequestHeaders.Add("Timeout", time);

            return await response;
        }

        /// <summary>
        ///     带重试的发送。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="url">URL。</param>
        /// <param name="json">JSON。</param>
        /// <param name="retryTimes">重试次数。</param>
        /// <returns></returns>
        internal static Task<HttpResponseMessage> PostWithRetry(this HttpClient client, ApiUrl url, JObject json, int retryTimes)
        {
            Task<HttpResponseMessage> response;
            do
            {
                response = client.PostAsync(url.Url, new StringContent(json.ToString(Formatting.None),Encoding.UTF8, "application/json"));
                retryTimes++;
            } while (retryTimes >= 0 && response.Result.StatusCode != System.Net.HttpStatusCode.OK);
            return response;
        }


        internal static async Task<string>  RawText(this HttpResponseMessage response)
        {
            
            return  await response.Content.ReadAsStringAsync();
        }
    }


    public static class CookieExtension
    {
        public static CookieCollection GetCookies(this CookieContainer cookies, ApiUrl url, params object[] args)
        {
           return cookies.GetCookies(new System.Uri(url.BuildUrl(args)));
        }
    }
}
