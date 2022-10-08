using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace haku_chat.Models
{
    /// <summary>
    /// 名前色コードマスターテーブル用モデル
    /// </summary>
    [DisplayName("名前色コードマスターテーブル")]
    [Table("chat_name_color_master")]
    public class ChatNameColorMasterModel
    {
        /// <summary>
        /// 投稿者名文字色ID
        /// </summary>
        [Key]
        [DisplayName("投稿者名文字色ID")]
        [Required(ErrorMessage = "{0}は必須です。")]
        [Column("id", Order = 1)]
        public uint Id { get; set; }

        /// <summary>
        /// 投稿者名文字色名称
        /// </summary>
        [DisplayName("投稿者名文字色名称")]
        [Required(ErrorMessage = "{0}は必須です。")]
        [StringLength(32, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
        [Column("name", Order = 2)]
        public string Name { get; set; }

        /// <summary>
        /// 投稿者名文字色コード
        /// </summary>
        [DisplayName("投稿者名文字色コード")]
        [Required(ErrorMessage = "{0}は必須です。")]
        [StringLength(32, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
        [Column("code", Order = 3)]
        public string Code { get; set; }

        /// <summary>
        /// コンストラクター
        ///  引数未指定時は投稿者名文字色を「黒」で初期化
        /// </summary>
        public ChatNameColorMasterModel()
        {
            Id = 0;
            Name = "黒";
            Code = "#000000";
        }

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="id">投稿者文字色ID</param>
        /// <param name="name">投稿者文字色名称</param>
        public ChatNameColorMasterModel(uint id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="id">投稿者文字色ID</param>
        /// <param name="name">投稿者文字色名称</param>
        /// <param name="code">投稿者文字色コード</param>
        public ChatNameColorMasterModel(uint id, string name, string code)
        {
            Id = id;
            Name = name;
            Code = code;
        }
    }
}
