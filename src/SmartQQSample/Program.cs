using SmartQQ.Client;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace SmartQQSample
{
    class Program
    {
        private const string CookiePath = "dump.json";
        private static readonly SmartQQClient Client = new SmartQQClient { CacheTimeout = TimeSpan.FromDays(1) };
        static void Main(string[] args)
        {
            // 好友消息回调
            Client.FriendMessageReceived +=(sender, message) =>
            {
                Console.WriteLine($"{message.Sender.Alias ?? message.Sender.Nickname}:{message.Content}");
            };
            // 群消息回调
            Client.GroupMessageReceived += (sender, message) =>
            {
                Console.WriteLine(
                    $"[{message.Group.Name}]{message.Sender.Alias ?? message.Sender.Nickname}:{message.Content}");
                if (message.Content.IsMatch(@"^\s*Knock knock\s*$"))
                    message.Reply("Who's there?");
                else if (message.StrictlyMentionedMe)
                    message.Reply("什么事？");
            };

            // 讨论组消息回调
            Client.DiscussionMessageReceived +=(sender, message) =>
            {
                Console.WriteLine($"[{message.Discussion.Name}]{message.Sender.Nickname}:{message.Content}");
            };

            // 消息回显
            Client.MessageEcho += (sender, e) =>
            {
                Console.WriteLine($"{e.Target.Name}>{e.Content}");
            };

            if (File.Exists(CookiePath))
            {
                // 尝试使用cookie登录
                if (Client.Start(File.ReadAllText(CookiePath)) != SmartQQClient.LoginResult.Succeeded)
                    QrLogin();
            }
            else
            {
                QrLogin();
            }

            Console.WriteLine($"欢迎，{Client.Nickname}!");
            // 导出cookie
            try
            {
                File.WriteAllText(CookiePath, Client.SmartQQCookies());
            }
            catch
            {
                // Ignored
            }
            // 防止程序终止
            while (Client.Status == SmartQQClient.ClientStatus.Active)
            {
            }
        }

        private static void QrLogin()
        {
            while (true)
                switch (Client.Start(path => {
                    using (var ms = new MemoryStream(path))
                    {
                        SmartQQClient.ConsoleWriteImage(new Bitmap(Image.FromStream(ms)));
                        Logger.Instance.Info("二维码已打印在屏幕，请使用手机QQ扫描。");
                    }
                }))
                {
                    case SmartQQClient.LoginResult.Succeeded:
                        return;
                    case SmartQQClient.LoginResult.QrCodeExpired:
                        continue;
                    default:
                        Console.WriteLine("登录失败，需要重试吗？(y/n)");
                        var response = Console.ReadLine();
                        if (response.IsMatch(@"^\s*y(es)?\s*$", RegexOptions.IgnoreCase))
                            continue;
                        Environment.Exit(1);
                        return;
                }
        }
    }
}