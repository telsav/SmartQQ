using SmartQQ.Constants;
using SmartQQ.Models;
using System;
using System.Net.Http;

namespace SmartQQ.Builder
{
    public partial class SmartQQClientBuilder
    {

        #region Public function

        public SmartQQClientBuilder(Action<byte[]> bytes, Func<string, LoginResult> json)
        {
            this._bytes = bytes;
            this.json = json;
        }

        public SmartQQClientBuilder(int port)
        {
            this._port = port;
        }

        public void Start(Action<SmartQQClientBuilder> action)
        {
            
            Start();
            action(this);
        }

        public void Start()
        {
            Logger.Instance.OutputType = OutputType.Console;
            Handler.UseCookies = true;
            Handler.CookieContainer = Cookies;
            Handler.AllowAutoRedirect = true;
            Client = new HttpClient(Handler);
            Client.DefaultRequestHeaders.Add("User-Agent", ApiUrl.UserAgent);
            Client.DefaultRequestHeaders.Add("KeepAlive", "true");
            _cache = new CacheDepot(CacheTimeout);
            _myInfoCache = new Cache<FriendInfo>(CacheTimeout);
            _qqNumberCache = new CacheDictionary<long, long>(CacheTimeout);
            Start(this._bytes);
            Start(this.json);
        }

        #endregion

    }
}
