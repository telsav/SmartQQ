using System;

namespace SmartQQ.Models
{
    /// <summary>
    ///     消息回显事件参数。
    /// </summary>
    public class MessageEchoEventArgs : EventArgs
    {
        internal MessageEchoEventArgs(IMessageable target, string content)
        {
            Target = target;
            Content = content;
        }

        /// <summary>
        ///     消息目标。
        /// </summary>
        public IMessageable Target { get; }

        /// <summary>
        ///     消息内容。
        /// </summary>
        public string Content { get; }
    }
}
