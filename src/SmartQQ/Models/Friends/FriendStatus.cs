using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartQQ.Client;
using SmartQQ.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartQQ.Models
{
    /// <summary>
    ///     好友状态。
    /// </summary>
    public class FriendStatus : IListable
    {
        /// <summary>
        ///     用于发送信息的编号。不等于群号。
        /// </summary>
        [JsonProperty("uin")]
        public long Id { get; set; }

        /// <summary>
        ///     当前状态。
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        ///     客户端类型。
        /// </summary>
        [JsonProperty("client_type")]
        public int ClientType { get; set; }

        internal static List<FriendStatus> GetList(SmartQQClient client)
        {
            Logger.Instance.Debug("开始获取好友状态列表");
            var response = client.Client.GetAsync(ApiUrl.GetFriendStatus, client.Vfwebqq, client.Psessionid);
            return
                ((JArray)client.GetResponseJson(response.Result)["result"])
                .ToObject<List<FriendStatus>>();
        }
    }
}
