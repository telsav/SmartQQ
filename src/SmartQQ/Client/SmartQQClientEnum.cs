namespace SmartQQ.Client
{
    public partial class SmartQQClient
    {
        /// <summary>
        ///     表示客户端状态的枚举。
        /// </summary>
        public enum ClientStatus
        {
            /// <summary>
            ///     客户端并没有连接到SmartQQ。
            /// </summary>
            Idle,

            /// <summary>
            ///     客户端正在登录。
            /// </summary>
            LoggingIn,

            /// <summary>
            ///     客户端已登录到SmartQQ。
            /// </summary>
            Active
        }

        /// <summary>
        ///     登录结果。
        /// </summary>
        public enum LoginResult
        {
            /// <summary>
            ///     登录成功。
            /// </summary>
            Succeeded,

            /// <summary>
            ///     二维码失效。登录失败。
            /// </summary>
            QrCodeExpired,

            /// <summary>
            ///     cookie失效。登录失败。
            /// </summary>
            CookieExpired,

            /// <summary>
            ///     发生了二维码失效和cookie失效以外的错误。
            /// </summary>
            Failed
        }

        /// <summary>
        ///     发送消息的目标类型。
        /// </summary>
        public enum TargetType
        {
            /// <summary>
            ///     好友。
            /// </summary>
            Friend,

            /// <summary>
            ///     群。
            /// </summary>
            Group,

            /// <summary>
            ///     讨论组。
            /// </summary>
            Discussion
        }
    }
}
