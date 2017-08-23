using SmartQQ.Models;
using System;
using static SmartQQ.SmartQQClient;

namespace SmartQQ.Builder
{
    public partial class SmartQQClientBuilder
    {
        #region FriendMessage

        private Action<FriendMessage> actionFriendMessage;
        public SmartQQClientBuilder ReceivedFriendMessage(Action<FriendMessage> action)
        {
            this.actionFriendMessage = (sender) =>
            {
                action(sender);
            };
            return this;
           
        }

        #endregion

        #region GroupMessage

        private Action<GroupMessage> actionGroupMessage;
        public SmartQQClientBuilder ReceivedGroupMessage(Action<GroupMessage> action)
        {
            this.actionGroupMessage = (sender) =>
            {
                action(sender);
            };
            return this;

        }

        #endregion

        #region DiscussionMessage

        private Action<DiscussionMessage> actionDiscussionMessage;
        public SmartQQClientBuilder ReceivedDiscussionMessage(Action<DiscussionMessage> action)
        {
            this.actionDiscussionMessage = (sender) =>
            {
                action(sender);
            };
            return this;

        }

        #endregion

        #region MessageEchoEventArgs

        private Action<MessageEchoEventArgs> actionMessageEchoEventArgs;
        public SmartQQClientBuilder ReceivedMessageEchoEventArgs(Action<MessageEchoEventArgs> action)
        {
            this.actionMessageEchoEventArgs = (sender) =>
            {
                action(sender);
            };
            return this;

        }

        #endregion

        #region MessageEchoEventArgs

        private Action<string> actionExtraLoginNeeded;
        public SmartQQClientBuilder ExtraLoginNeeded(Action<string> action)
        {
            this.actionExtraLoginNeeded = (sender) =>
            {
                action(sender);
            };
            return this;

        }

        #endregion  

        #region ConnectionLost

        private Action<Object> actionConnectionLost;
        public SmartQQClientBuilder ConnectionLost(Action<object> action)
        {
            this.actionConnectionLost = (sender) =>
            {
                action(sender);
            };
            return this;

        }

        #endregion  
        

    }
}
