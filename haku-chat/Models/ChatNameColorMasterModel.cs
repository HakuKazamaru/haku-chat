using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace haku_chat.Models
{
    /// <summary>
    /// 名前色管理用モデル
    /// </summary>
    public class ChatNameColorMasterModel
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public ChatNameColorMasterModel()
        {

        }

        public ChatNameColorMasterModel(uint id, string name)
        {
            Id = id;
            Name = name;
        }

        public ChatNameColorMasterModel(uint id, string name, string code)
        {
            Id = id;
            Name = name;
            Code = code;
        }
    }
}
