using System;

namespace haku_chat.Models
{
    /// <summary>
    /// チャット入室ステータス管理用モデル
    /// </summary>
    public class ChatLoginUserModel
    {
        public string Name;
        public uint Status;
        public string IpAddress;
        public string HostName;
        public string MachineName;
        public string OsName;
        public string UserAgent;
        public DateTime LoginTime;
        public DateTime LogoutTime;
        public DateTime LastUpdate;

        public ChatLoginUserModel()
        {
            LastUpdate = DateTime.Now;
        }

        public ChatLoginUserModel(string name)
        {
            Name = name;
            LastUpdate = DateTime.Now;
        }

    }
}
