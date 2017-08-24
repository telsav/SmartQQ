using SmartQQ.Builder;
using SmartQQ.Provider;
using System;

namespace SmartQQ
{
    public static partial class SmartQQClient
    {
        public static ISmartQQClientBuilder Login(Action<byte[]> action)
        {
            return new SmartQQClientBuilder(action, null);
        }

        public static ISmartQQClientBuilder Login(Action<byte[]> action, Func<string, LoginResult> json)
        {
            return new SmartQQClientBuilder(action, json);
        }

        public static ISmartQQClientBuilder StartListen(int port)
        {
            return new SmartQQClientBuilder(port);
        }
    }
}