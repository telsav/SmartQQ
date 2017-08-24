using Newtonsoft.Json.Linq;
using SmartQQ.Constants;
using SmartQQ.Models;
using System;
using System.Collections.Generic;

namespace SmartQQ.Builder
{
    public partial class SmartQQClientBuilder
    {
        #region Properly

        // 数据缓存
        private CacheDepot _cache;
        private Cache<FriendInfo> _myInfoCache;
        private CacheDictionary<long, long> _qqNumberCache;

        private TimeSpan _cacheTimeout = TimeSpan.FromHours(2);
        /// <summary>
        ///     发送消息的重试次数。
        /// </summary>
        public int RetryTimes { get; set; } = 5;
        // 线程开关
        private volatile bool _pollStarted;

        // 二维码验证参数
        private string _qrsig;
        public string Hash;
        public string Psessionid;

        // 鉴权参数
        public string Ptwebqq;
        public long Uin;
        public string Vfwebqq;

        /// <summary>
        ///     客户端当前状态。
        /// </summary>
        public ClientStatus Status { get; private set; } = ClientStatus.Idle;

        /// <summary>
        ///     已登录账户的最近会话。
        /// </summary>
        public List<ChatHistory> RecentConversations => GetListOf<ChatHistory>();

        /// <summary>
        ///     已登录账户加入的讨论组。
        /// </summary>
        public List<Discussion> Discussions => GetListOf<Discussion>();

        /// <summary>
        ///     已登录账户的好友。
        /// </summary>
        public List<Friend> Friends => GetListOf<Friend>();

        /// <summary>
        ///     已登录账户的好友分组。
        /// </summary>
        public List<FriendCategory> Categories => GetListOf<FriendCategory>();

        /// <summary>
        ///     已登录账户加入的群。
        /// </summary>
        public List<Group> Groups => GetListOf<Group>();

        /// <summary>
        ///     缓存的超时时间。
        /// </summary>
        public TimeSpan CacheTimeout
        {
            get { return _cacheTimeout; }
            set
            {
                _cacheTimeout = value;
                _cache.Timeout = value;
                _myInfoCache.Timeout = value;
                _qqNumberCache.Timeout = value;
            }
        }

        private FriendInfo MyInfo
        {
            get
            {
                if (Status != ClientStatus.Active)
                    throw new InvalidOperationException("尚未登录，无法进行该操作");
                FriendInfo cachedInfo;
                if (_myInfoCache.TryGetValue(out cachedInfo))
                    return cachedInfo;
                Logger.Instance.Debug("开始获取登录账户信息");

                var response = Client.GetAsync(ApiUrl.GetAccountInfo);
                var info = ((JObject)GetResponseJson(response.Result)["result"]).ToObject<FriendInfo>();
                _myInfoCache.SetValue(info);
                return info;
            }
        }

        /// <summary>
        ///     已登录账户的编号。
        /// </summary>
        public long Id => MyInfo.Id;

        /// <summary>
        ///     已登录账户的QQ号。
        /// </summary>
        public long QQNumber => GetQQNumberOf(Id);

        /// <summary>
        ///     已登录账户的昵称。
        /// </summary>
        public string Nickname => MyInfo.Nickname;

        /// <summary>
        ///     已登录账户的个性签名。
        /// </summary>
        public string Bio => MyInfo.Bio;

        /// <summary>
        ///     已登录账户的生日。
        /// </summary>
        public Birthday Birthday => MyInfo.Birthday;

        /// <summary>
        ///     已登录账户的座机号码。
        /// </summary>
        public string Phone => MyInfo.Phone;

        /// <summary>
        ///     已登录账户的手机号码。
        /// </summary>
        public string Cellphone => MyInfo.Cellphone;

        /// <summary>
        ///     已登录账户的邮箱地址。
        /// </summary>
        public string Email => MyInfo.Email;

        /// <summary>
        ///     已登录账户的职业。
        /// </summary>
        public string Job => MyInfo.Job;

        /// <summary>
        ///     已登录账户的个人主页。
        /// </summary>
        public string Homepage => MyInfo.Homepage;

        /// <summary>
        ///     已登录账户的学校。
        /// </summary>
        public string School => MyInfo.School;

        /// <summary>
        ///     已登录账户的国家。
        /// </summary>
        public string Country => MyInfo.Country;

        /// <summary>
        ///     已登录账户的省份。
        /// </summary>
        public string Province => MyInfo.Province;

        /// <summary>
        ///     已登录账户的城市。
        /// </summary>
        public string City => MyInfo.City;

        /// <summary>
        ///     已登录账户的性别。
        /// </summary>
        public string Gender => MyInfo.Gender;

        /// <summary>
        ///     已登录账户的生肖。
        /// </summary>
        public int Shengxiao => MyInfo.Shengxiao;

        /// <summary>
        ///     已登录账户的某信息字段。意义暂不明确。
        /// </summary>
        public string Personal => MyInfo.Personal;

        /// <summary>
        ///     已登录账户的某信息字段。意义暂不明确。
        /// </summary>
        public int VipInfo => MyInfo.VipInfo;


        #endregion
    }
}
