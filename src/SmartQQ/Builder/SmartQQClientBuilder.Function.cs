using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartQQ.Constants;
using SmartQQ.Models;
using SmartQQ.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace SmartQQ.Builder
{
    public partial class SmartQQClientBuilder
    {
        #region Function
        /// <summary>
        ///     查询列表。
        /// </summary>
        /// <returns></returns>
        internal List<T> GetListOf<T>() where T : class, IListable
        {
            if (Status != ClientStatus.Active)
                throw new InvalidOperationException("尚未登录，无法进行该操作");
            List<T> tempData;
            if (_cache.GetCache<List<T>>().TryGetValue(out tempData))
            {
                Logger.Instance.Debug("加载了缓存的" + typeof(T).Name + "列表");
                return tempData;
            }
            // 为了性能所以不使用reflection而采用硬编码
            List<T> result;
            switch (typeof(T).Name)
            {
                case "ChatHistory":
                    result = (List<T>)(object)ChatHistory.GetList(this);
                    break;
                case "Discussion":
                    result = (List<T>)(object)Discussion.GetList(this);
                    break;
                case "Friend":
                    result = (List<T>)(object)Friend.GetList(this);
                    break;
                case "FriendCategory":
                    result = (List<T>)(object)FriendCategory.GetList(this);
                    break;
                case "FriendStatus":
                    result = (List<T>)(object)FriendStatus.GetList(this);
                    break;
                case "Group":
                    result = (List<T>)(object)Group.GetList(this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _cache.GetCache<List<T>>().SetValue(result);
            return result;
        }

        /// <summary>
        ///     根据ID获取QQ号。
        /// </summary>
        /// <param name="userId">用户ID。</param>
        /// <returns>QQ号。</returns>
        public long GetQQNumberOf(long userId)
        {
            if (Status != ClientStatus.Active)
                throw new InvalidOperationException("尚未登录，无法进行该操作");
            Logger.Instance.Debug("开始获取QQ号");

            if (_qqNumberCache.ContainsKey(userId))
            {
                Logger.Instance.Debug("加载了缓存的QQ号");
                return _qqNumberCache[userId];
            }

            var qq =
            ((JObject)
                JObject.Parse(Client.GetStringAsync(ApiUrl.GetQQById, userId, Vfwebqq, RandomHelper.GetRandomDouble()).Result)[
                    "result"])["account"].Value<long>();
            _qqNumberCache.Put(userId, qq);
            return qq;
        }

        /// <summary>
        ///     发送消息。
        /// </summary>
        /// <param name="type">目标类型。</param>
        /// <param name="id">用于发送的ID。</param>
        /// <param name="content">消息内容。</param>
        public void Message(TargetType type, long id, string content)
        {
            if (Status != ClientStatus.Active)
                throw new InvalidOperationException("尚未登录，无法进行该操作");
            Logger.Instance.Debug("开始发送消息，对象类型：" + type);

            string paramName;
            ApiUrl url;

            switch (type)
            {
                case TargetType.Friend:
                    paramName = "to";
                    url = ApiUrl.SendMessageToFriend;
                    break;
                case TargetType.Group:
                    paramName = "group_uin";
                    url = ApiUrl.SendMessageToGroup;
                    break;
                case TargetType.Discussion:
                    paramName = "did";
                    url = ApiUrl.SendMessageToDiscussion;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            var response = Client.PostAsyncWithRetry(url, new JObject
            {
                {paramName, id},
                {
                    "content",
                    new JArray
                        {
                            StringHelper.TranslateEmoticons(content),
                            new JArray {"font", JObject.FromObject(Models.Font.DefaultFont)}
                        }
                        .ToString(Formatting.None)
                },
                {"face", 573},
                {"clientid", ClientId},
                {"msg_id", _messageId++},
                {"psessionid", Psessionid}
            }, RetryTimes);

            if (response.Result.StatusCode != HttpStatusCode.OK)
                Logger.Instance.Error("消息发送失败，HTTP返回码" + (int)response.Result.StatusCode);

            var status = JObject.Parse(response.Result.RawText().Result)["retcode"].ToObject<int?>();
            if (status != null && (status == 0 || status == 100100))
            {
                Logger.Instance.Debug("消息发送成功");
                if (actionMessageEchoEventArgs == null) return;
                MessageEchoEventArgs args;
                switch (type)
                {
                    case TargetType.Friend:
                        {
                            args = new MessageEchoEventArgs(Friends.Find(_ => _.Id == id), content);
                            break;
                        }
                    case TargetType.Group:
                        {
                            args = new MessageEchoEventArgs(Groups.Find(_ => _.Id == id), content);
                            break;
                        }
                    case TargetType.Discussion:
                        {
                            args = new MessageEchoEventArgs(Discussions.Find(_ => _.Id == id), content);
                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
                actionMessageEchoEventArgs(args);
            }
            else
            {
                Logger.Instance.Error("消息发送失败，API返回码" + status);
            }
        }

        /// <summary>
        ///     导出当前cookie集合。
        /// </summary>
        /// <returns>当前cookie集合的JSON字符串。</returns>
        public string SmartQQCookies()
        {
            if (Status != ClientStatus.Active)
                throw new InvalidOperationException("仅在登录后才能导出cookie");
            return new JObject
            {
                {"hash", Hash},
                {"psessionid", Psessionid},
                {"ptwebqq", Ptwebqq},
                {"uin", Uin},
                {"vfwebqq", Vfwebqq},
                {
                    "cookies",
                    JArray.FromObject(Cookies.GetAllCookies())
                }
            }.ToString(Formatting.None);
        }

        /// <summary>
        ///     使用cookie连接到SmartQQ。
        /// </summary>
        /// <param name="json">由DumpCookies()导出的JSON字符串。</param>
        private LoginResult Start(string json)
        {
            if (Status != ClientStatus.Idle)
                throw new InvalidOperationException("已在登录或者已经登录，不能重复进行登录操作");
            try
            {
                Logger.Instance.Debug("开始通过cookie登录");
                Status = ClientStatus.LoggingIn;
                var dump = JObject.Parse(json);
                Hash = dump["hash"].Value<string>();
                Psessionid = dump["psessionid"].Value<string>();
                Ptwebqq = dump["ptwebqq"].Value<string>();
                Uin = dump["uin"].Value<long>();
                Vfwebqq = dump["vfwebqq"].Value<string>();
                foreach (var cookie in dump["cookies"].Value<JArray>().ToObject<List<Cookie>>())
                {
                    Cookies.Add(new Uri("http://" + cookie.Domain), cookie);
                }
                Handler.CookieContainer = Cookies;

                if (TestLogin())
                {
                    Status = ClientStatus.Active;
                    StartMessageLoop();
                    return LoginResult.Succeeded;
                }
                Status = ClientStatus.Idle;
                return LoginResult.CookieExpired;
            }
            catch (Exception ex)
            {
                Status = ClientStatus.Idle;
                Logger.Instance.Error("登录失败，抛出异常：" + ex);
                return LoginResult.Failed;
            }
        }

        public LoginResult Start(Func<string, LoginResult> json)
        {
            var result = Login(json);
            return result;
        }

        /// <summary>
        ///     连接到SmartQQ。
        /// </summary>
        /// <param name="qrCodeDownloadedCallback">二维码已下载时的回调函数。回调函数的参数为二维码图像的字节数组。</param>
        public LoginResult Start(Action<byte[]> qrCodeDownloadedCallback)
        {
            if (Status != ClientStatus.Idle)
                throw new InvalidOperationException("已在登录或者已经登录，不能重复进行登录操作");
            var result = Login(qrCodeDownloadedCallback);
            if (result != LoginResult.Succeeded)
            {
                Status = ClientStatus.Idle;
                return result;
            }
            Status = ClientStatus.Active;
            StartMessageLoop();
            return result;
        }

        /// <summary>
        ///     连接到SmartQQ。
        /// </summary>
        /// <param name="qrCodeDownloadedCallback">二维码已下载时的回调函数。回调函数的参数为已下载的二维码的绝对路径。</param>
        [Obsolete("此方法已不赞成使用，并可能在未来版本中移除。请考虑改为使用Start(Action<byte[]>)。")]
        public LoginResult Start(Action<string> qrCodeDownloadedCallback) => Start(_ =>
        {
            var filePath = Path.GetFullPath("qrcode" + RandomHelper.GetRandomInt() + ".png");
            File.WriteAllBytes(filePath, _);
            SmartQQClient.ConsoleWriteImage(filePath);
            qrCodeDownloadedCallback(filePath);
        });

        private LoginResult Login(Func<string, LoginResult> json)
        { 
           return Login(json);  
        }

        // 登录
        private LoginResult Login(Action<byte[]> qrCodeDownloadedCallback)
        {
            try
            {
                Status = ClientStatus.LoggingIn;
                GetQrCode(qrCodeDownloadedCallback);
                var url = VerifyQrCode();
                GetPtwebqq(url);
                GetVfwebqq();
                GetUinAndPsessionid();
                if (!TestLogin())
#pragma warning disable 612
                    actionExtraLoginNeeded(@"http://w.qq.com");
#pragma warning restore 612
                Hash = StringHelper.SomewhatHash(Uin, Ptwebqq);
                return LoginResult.Succeeded;
            }
            catch (TimeoutException)
            {
                return LoginResult.QrCodeExpired;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("登录失败，抛出异常：" + ex);
                return LoginResult.Failed;
            }
        }

        // 获取二维码
        private void GetQrCode(Action<byte[]> qrCodeDownloadedCallback)
        {
            Logger.Instance.Debug("开始获取二维码");

            var response = Client.GetByteArrayAsync(ApiUrl.GetQrCode.Url);
            response.Wait();

            foreach (Cookie cookie in Cookies.GetCookies(new Uri(ApiUrl.GetQrCode.Url)))
            {
                if (cookie.Name != "qrsig") continue;
                _qrsig = cookie.Value;
                break;
            }
            Logger.Instance.Info("二维码已获取");

            qrCodeDownloadedCallback.Invoke(response.Result);
        }

        private static int Hash33(string s)
        {
            int e = 0, i = 0, n = s.Length;
            for (; n > i; ++i)
                e += (e << 5) + s[i];
            return 2147483647 & e;
        }

        // 校验二维码
        private string VerifyQrCode()
        {
            Logger.Instance.Debug("等待扫描二维码");

            // 阻塞直到确认二维码认证成功
            while (true)
            {
                Thread.Sleep(1000);
                var result = Client.GetStringAsync(ApiUrl.VerifyQrCode, Hash33(_qrsig)).Result;
                var response = Cookies.GetCookies(ApiUrl.VerifyQrCode, Hash33(_qrsig));
                if (result.Contains("成功"))
                {
                    foreach (Cookie cookie in response)
                    {
                        if (cookie.Name == "ptwebqq")
                        {
                            Ptwebqq = cookie.Value;
                        }
                    }
                    foreach (var content in result.Split(new[] { "','" }, StringSplitOptions.None))
                    {
                        if (!content.StartsWith("http"))
                            continue;
                        Logger.Instance.Info("正在登录，请稍后");
                        return content;
                    }
                }
                else if (result.Contains("已失效"))
                {
                    Logger.Instance.Warning("二维码已失效，终止登录流程");
                    throw new TimeoutException();
                }
            }
        }

        // 获取ptwebqq
        private void GetPtwebqq(string url)
        {
            Logger.Instance.Debug("开始获取ptwebqq");
            var response = Client.GetAsync(ApiUrl.GetPtwebqq, url);
            response.Wait();
        }

        // 获取vfwebqq
        private void GetVfwebqq()
        {
            Logger.Instance.Debug("开始获取vfwebqq");
            var response = Client.GetAsync(ApiUrl.GetVfwebqq, Ptwebqq);
            response.Wait();
            Vfwebqq = ((JObject)GetResponseJson(response.Result)["result"])["vfwebqq"].Value<string>();
        }

        // 获取uin和psessionid
        private void GetUinAndPsessionid()
        {
            Logger.Instance.Debug("开始获取uin和psessionid");

            var r = new JObject
            {
                {"ptwebqq", Ptwebqq},
                {"clientid", ClientId},
                {"psessionid", ""},
                {"status", "online"}
            };

            var response = Client.PostAsync(ApiUrl.GetUinAndPsessionid, r);
            var result = (JObject)GetResponseJson(response.Result)["result"];
            Psessionid = result["psessionid"].Value<string>();
            Uin = result["uin"].Value<long>();
        }

        // 解决103错误码
        private bool TestLogin()
        {
            Logger.Instance.Debug("开始向服务器发送测试连接请求");

            var result = Client.GetStringAsync(ApiUrl.TestLogin, Vfwebqq, ClientId, Psessionid, RandomHelper.GetRandomDouble());
            result.Wait();
            return result.IsCompleted &&
                   JObject.Parse(result.Result)["retcode"].Value<int?>() == 0;
        }

        // 开始消息轮询
        private void StartMessageLoop()
        {
            _pollStarted = true;
            new Thread(() =>
            {
                while (true)
                {
                    if (!_pollStarted) return;
                    if (actionFriendMessage == null && actionGroupMessage == null && actionDiscussionMessage == null)
                        continue;
                    try
                    {
                        PollMessage();
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Error(ex);
                        // 自动掉线
                        if (TestLogin())
                            continue;
                        Close();
                        actionConnectionLost(EventArgs.Empty);
                    }
                }
            })
            { IsBackground = true }.Start();
        }

        // 拉取消息
        private void PollMessage()
        {
            Logger.Instance.Debug(DateTime.Now.ToString() + " 开始接收消息");

            var r = new JObject
            {
                {"ptwebqq", Ptwebqq},
                {"clientid", ClientId},
                {"psessionid", Psessionid},
                {"key", ""}
            };

            var response = Client.PostAsync(ApiUrl.PollMessage, r, 120000);
            response.Wait();
            var array = GetResponseJson(response.Result)["result"] as JArray;
            for (var i = 0; array != null && i < array.Count; i++)
            {
                var message = (JObject)array[i];
                var type = message["poll_type"].Value<string>();
                switch (type)
                {
                    case "message":
                        var fmsg = message["value"].ToObject<FriendMessage>();
                        fmsg.Client = this;
                        actionFriendMessage(fmsg);
                        break;
                    case "group_message":
                        var gmsg = message["value"].ToObject<GroupMessage>();
                        gmsg.Client = this;
                        actionGroupMessage(gmsg);
                        break;
                    case "discu_message":
                        var dmsg = message["value"].ToObject<DiscussionMessage>();
                        dmsg.Client = this;
                        actionDiscussionMessage(dmsg);
                        break;
                    default:
                        Logger.Instance.Warning("意外的消息类型：" + type);
                        break;
                }
            }
        }

        /// <summary>
        ///     停止通讯。
        /// </summary>
        private void Close()
        {
            if (Status == ClientStatus.Idle)
                throw new InvalidOperationException("尚未登录，无法进行该操作");
            _pollStarted = false;
            // 清除缓存
            _cache.Clear();
            _myInfoCache.Clear();
            _qqNumberCache.Clear();
            Client.Dispose();
        }

        internal JObject GetResponseJson(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
                Logger.Instance.Error("请求失败，Http返回码" + (int)response.StatusCode);
            var json = JObject.Parse(response.RawText().Result);
            var retCode = json["retcode"].Value<int?>();
            switch (retCode)
            {
                case 0:
                    return json;
                case null:
                    throw new HttpRequestException("请求失败，API未返回状态码");
                case 103:
                    Logger.Instance.Error("请求失败，API返回码103；可能需要进一步登录。");
                    break;
                default:
                    throw new HttpRequestException("请求失败，API返回码" + retCode, new ApiException((int)retCode));
            }
            return json;
        }

        internal static Dictionary<long, Friend> ParseFriendDictionary(JObject result)
        {
            var friends = new Dictionary<long, Friend>();
            var info = result["info"] as JArray;
            for (var i = 0; info != null && i < info.Count; i++)
            {
                var x = (JObject)info[i];
                var friend = new Friend
                {
                    Id = x["uin"].Value<long>(),
                    Nickname = x["nick"].Value<string>()
                };
                friends.Add(friend.Id, friend);
            }
            var marknames = result["marknames"] as JArray;
            for (var i = 0; marknames != null && i < marknames.Count; i++)
            {
                var item = (JObject)marknames[i];
                friends[item["uin"].ToObject<long>()].Alias = item["markname"].ToObject<string>();
            }
            var vipinfo = result["vipinfo"] as JArray;
            for (var i = 0; vipinfo != null && i < vipinfo.Count; i++)
            {
                var item = (JObject)vipinfo[i];
                var friend = friends[item["u"].Value<long>()];
                friend.IsVip = item["is_vip"].Value<int>() == 1;
                friend.VipLevel = item["vip_level"].Value<int>();
            }
            return friends;
        }

        #endregion
    }
}
