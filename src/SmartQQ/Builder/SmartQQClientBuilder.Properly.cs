using System;
using System.Net;
using System.Net.Http;

namespace SmartQQ.Builder
{
    public partial class SmartQQClientBuilder
    {
        #region Filed

        public const long ClientId = 53999199;
        private long _messageId = 43690001;
        private Action<byte[]> _bytes;
        private int _port = 6060;
        private static HttpClientHandler Handler = new HttpClientHandler();
        public HttpClient Client { get; set; }
        private static CookieContainer Cookies = new CookieContainer();
        private Func<string, LoginResult> json;

        #endregion
    }
}
