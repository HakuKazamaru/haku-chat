using System.Collections.Generic;

namespace haku_chat.Models
{
    /// <summary>
    /// 利用規約管理モデル
    /// </summary>
    public class PolicyModel
    {
        /// <summary>
        /// ポリシー使用フラグ
        /// </summary>
        public bool Use { get; set; }

        /// <summary>
        /// ポリシー文
        /// </summary>
        public List<PolicyString> PolicyStrings { get; set; }

    }

    /// <summary>
    /// ポリシー文
    /// </summary>
    public class PolicyString
    {
        /// <summary>
        /// 章名
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 本文
        /// </summary>
        public List<string> Texts { get; set; }
    }
}
