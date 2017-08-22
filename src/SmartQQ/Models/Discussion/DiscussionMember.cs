using Newtonsoft.Json;
using SmartQQ.Client;

namespace SmartQQ.Models
{
    /// <summary>
    ///     讨论组成员。
    /// </summary>
    public class DiscussionMember : User
    {
        [JsonIgnore] internal SmartQQClient Client;

        /// <inheritdoc />
        [JsonIgnore]
        public override long QQNumber => Client.GetQQNumberOf(Id);

        /// <inheritdoc />
        [JsonProperty("uin")]
        public override long Id { get; internal set; }

        /// <inheritdoc />
        [JsonProperty("nick")]
        public override string Nickname { get; internal set; }

        /// <summary>
        ///     客户端类型。
        /// </summary>
        [JsonProperty("clientType")]
        public int ClientType { get; set; }

        /// <summary>
        ///     当前状态。
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
