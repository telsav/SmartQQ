using System;
using System.Linq;

namespace SmartQQ.Constants
{
    public class ApiUrl
    {
        public const string UserAgent =
            "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:51.0) Gecko/20100101 Firefox/51.0";

        public static readonly ApiUrl GetQrCode = new ApiUrl(
            "https://ssl.ptlogin2.qq.com/ptqrshow?appid=501004106&e=0&l=M&s=5&d=72&v=4&t=0.1",
            ""
        );

        public static readonly ApiUrl VerifyQrCode = new ApiUrl(
            "https://ssl.ptlogin2.qq.com/ptqrlogin?ptqrtoken={1}&webqq_type=10&remember_uin=1&login2qq=1&aid=501004106&u1=http%3A%2F%2Fw.qq.com%2Fproxy.html%3Flogin2qq%3D1%26webqq_type%3D10&ptredirect=0&ptlang=2052&daid=164&from_ui=1&pttype=1&dumy=&fp=loginerroralert&0-0-157510&mibao_css=m_webqq&t=undefined&g=1&js_type=0&js_ver=10184&login_sig=&pt_randsalt=3",
            "https://ui.ptlogin2.qq.com/cgi-bin/login?daid=164&target=self&style=16&mibao_css=m_webqq&appid=501004106&enable_qlogin=0&no_verifyimg=1&s_url=http%3A%2F%2Fw.qq.com%2Fproxy.html&f_url=loginerroralert&strong_login=1&login_state=10&t=20131024001"
        );

        public static readonly ApiUrl GetPtwebqq = new ApiUrl(
            "{1}",
            null
        );

        public static readonly ApiUrl GetVfwebqq = new ApiUrl(
            "http://s.web2.qq.com/api/getvfwebqq?ptwebqq={1}&clientid=53999199&psessionid=&t=0.1",
            "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1"
        );

        public static readonly ApiUrl GetUinAndPsessionid = new ApiUrl(
            "http://d1.web2.qq.com/channel/login2",
            "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2"
        );

        public static readonly ApiUrl TestLogin = new ApiUrl(
            "http://d1.web2.qq.com/channel/get_online_buddies2?vfwebqq={1}&clientid={2}&psessionid={3}&t={4}",
            "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2"
        );

        public static readonly ApiUrl GetGroupList = new ApiUrl(
            "http://s.web2.qq.com/api/get_group_name_list_mask2",
            "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2"
        );

        public static readonly ApiUrl PollMessage = new ApiUrl(
            "http://d1.web2.qq.com/channel/poll2",
            "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2"
        );

        public static readonly ApiUrl SendMessageToGroup = new ApiUrl(
            "http://d1.web2.qq.com/channel/send_qun_msg2",
            "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2"
        );

        public static readonly ApiUrl GetFriendList = new ApiUrl(
            "http://s.web2.qq.com/api/get_user_friends2",
            "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1"
        );

        public static readonly ApiUrl SendMessageToFriend = new ApiUrl(
            "http://d1.web2.qq.com/channel/send_buddy_msg2",
            "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2"
        );

        public static readonly ApiUrl GetDiscussionList = new ApiUrl(
            "http://s.web2.qq.com/api/get_discus_list?clientid=53999199&psessionid={1}&vfwebqq={2}&t=0.1",
            "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1"
        );

        public static readonly ApiUrl SendMessageToDiscussion = new ApiUrl(
            "http://d1.web2.qq.com/channel/send_discu_msg2",
            "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2"
        );

        public static readonly ApiUrl GetAccountInfo = new ApiUrl(
            "http://s.web2.qq.com/api/get_self_info2?t=0.1",
            "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1"
        );

        public static readonly ApiUrl GetChatHistoryList = new ApiUrl(
            "http://d1.web2.qq.com/channel/get_recent_list2",
            "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2"
        );

        public static readonly ApiUrl GetFriendStatus = new ApiUrl(
            "http://d1.web2.qq.com/channel/get_online_buddies2?vfwebqq={1}&clientid=53999199&psessionid={2}&t=0.1",
            "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2"
        );

        public static readonly ApiUrl GetGroupInfo = new ApiUrl(
            "http://s.web2.qq.com/api/get_group_info_ext2?gcode={1}&vfwebqq={2}&t=0.1",
            "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1"
        );

        public static readonly ApiUrl GetQQById = new ApiUrl(
            "http://s.web2.qq.com/api/get_friend_uin2?tuin={1}&type=1&vfwebqq={2}&t=0.1",
            "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2"
        );

        public static readonly ApiUrl GetDiscussionInfo = new ApiUrl(
            "http://d1.web2.qq.com/channel/get_discu_info?did={1}&vfwebqq={2}&clientid=53999199&psessionid={3}&t=0.1",
            "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2"
        );

        public static readonly ApiUrl GetFriendInfo = new ApiUrl(
            "http://s.web2.qq.com/api/get_friend_info2?tuin={1}&vfwebqq={2}&clientid=53999199&psessionid={3}&t=0.1",
            "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1"
        );

        public readonly string Referer;

        public readonly string Url;

        public ApiUrl(string url, string referer)
        {
            Url = url;
            Referer = referer;
        }

        public string Origin
            =>
                Url.Substring(0,
                    Url.LastIndexOf("/", StringComparison.Ordinal));

        public string BuildUrl(params object[] args)
        {
            var i = 1;
            return args.Aggregate(Url, (current, arg) => current.Replace("{" + i++ + "}", arg.ToString()));
        }
    }
}
