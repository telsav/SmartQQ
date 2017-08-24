using SmartQQ.Builder;
using SmartQQ.Models;
using System;

namespace SmartQQ.Provider
{
    public partial interface ISmartQQClientBuilder
    {
        ISmartQQClientBuilder ReceivedFriendMessage(Action<FriendMessage> action);
        ISmartQQClientBuilder ReceivedGroupMessage(Action<GroupMessage> action);
        ISmartQQClientBuilder ReceivedDiscussionMessage(Action<DiscussionMessage> action);
        ISmartQQClientBuilder ReceivedMessageEchoEventArgs(Action<MessageEchoEventArgs> action);
        ISmartQQClientBuilder ExtraLoginNeeded(Action<string> action);
        ISmartQQClientBuilder ConnectionLost(Action<object> action);
        void Start(Action<SmartQQClientBuilder> action = null);
    }
}
