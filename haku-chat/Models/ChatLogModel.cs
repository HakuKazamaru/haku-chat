using System;

namespace haku_chat.Models
{
    /// <summary>
    /// チャットログ管理用モデル
    /// </summary>
    public class ChatLogModel
    {
        public DateTime RegTime { get; set; }
        public string Name { get; set; }
        public uint NameColorId { get; set; }
        public string NameColorCode { get; set; }
        public string Message { get; set; }

        public ChatLogModel()
        {
            NameColorId = 0;
            NameColorCode = "#000000";
        }

        public ChatLogModel(DateTime dateTime, string name, string message)
        {
            RegTime = dateTime;
            Name = name;
            NameColorId = 0;
            NameColorCode = "#000000";
            Message = message;
        }

        public ChatLogModel(DateTime dateTime, string name, uint nameColorId,string nameColorCode,string message)
        {
            RegTime = dateTime;
            Name = name;
            NameColorId = nameColorId;
            NameColorCode = nameColorCode;
            Message = message;
        }
    }
}