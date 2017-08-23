using SmartQQ.Builder;
using System;

namespace SmartQQ
{
    public static partial class SmartQQClient
    {
        public static SmartQQClientBuilder Login(Action<byte[]> action)
        {
            return new SmartQQClientBuilder(action);
        }

        public static SmartQQClientBuilder StartListen(int port)
        {
            return new SmartQQClientBuilder(port);
        }
    }
}