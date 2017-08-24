using SmartQQ.Models;
using SmartQQ.Provider;
using System;

namespace SmartQQ.Builder
{
    public partial class SmartQQClientBuilder: ISmartQQClientBuilder
    {
        #region FriendMessage

        private Action<FriendMessage> actionFriendMessage;
        public ISmartQQClientBuilder ReceivedFriendMessage(Action<FriendMessage> action)
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
        public ISmartQQClientBuilder ReceivedGroupMessage(Action<GroupMessage> action)
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
        public ISmartQQClientBuilder ReceivedDiscussionMessage(Action<DiscussionMessage> action)
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
        public ISmartQQClientBuilder ReceivedMessageEchoEventArgs(Action<MessageEchoEventArgs> action)
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
        public ISmartQQClientBuilder ExtraLoginNeeded(Action<string> action)
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
        public ISmartQQClientBuilder ConnectionLost(Action<object> action)
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
