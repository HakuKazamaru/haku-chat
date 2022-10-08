using haku_chat.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace haku_chat.Models
{
    /// <summary>
    /// チャットログテーブル管理用モデル
    /// </summary>
    [DisplayName("チャットログテーブル")]
    [Table("chat_log")]
    public class ChatLogModel
    {
#nullable enable
        /// <summary>
        /// チャット投稿日時
        /// </summary>
        [Key]
        [DisplayName("チャット投稿日時")]
        [Required(ErrorMessage = "{0}は必須です。")]
        [Column("reg_date", Order = 1)]
        public DateTime RegTime { get; set; }

        /// <summary>
        /// 投稿者名
        /// </summary>
        [Column("name", Order = 2)]
        [DisplayName("投稿者名")]
        [Required(ErrorMessage = "{0}は必須です。")]
        [StringLength(100, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
        public string Name { get; set; }

        /// <summary>
        /// 投稿者名文字色ID
        /// </summary>
        [DisplayName("投稿者名文字色ID")]
        [Column("name_color_id", Order = 3)]
        public uint? NameColorId { get; set; }

        /// <summary>
        /// 投稿者名文字色コード
        /// </summary>
        [NotMapped]
        [DisplayName("投稿者名文字色コード")]
        [StringLength(32, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
        public string? NameColorCode { get; set; }

        /// <summary>
        /// チャットメッセージ
        /// </summary>
        [DisplayName("チャットメッセージ")]
        [Required(ErrorMessage = "{0}は必須です。")]
        [Column("message", Order = 4)]
        public string Message { get; set; }

#nullable disable
        /// <summary>
        /// コンストラクター
        /// 　引数未指定時は投稿者名文字色を「黒」で初期化
        /// </summary>
        public ChatLogModel()
        {
            NameColorId = (uint)Common.NameColor.ColorCodeID.BLACK;
            NameColorCode = Common.NameColor.ColorCode.BLACK.GetStringValue();
        }

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="dateTime">チャット投稿日時</param>
        /// <param name="name">投稿者名</param>
        /// <param name="message">チャットメッセージ</param>
        public ChatLogModel(DateTime dateTime, string name, string message)
        {
            RegTime = dateTime;
            Name = name;
            NameColorId = (uint)Common.NameColor.ColorCodeID.BLACK;
            NameColorCode = Common.NameColor.ColorCode.BLACK.GetStringValue();
            Message = message;
        }

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="dateTime">チャット投稿日時</param>
        /// <param name="name">投稿者名</param>
        /// <param name="nameColorId">投稿者名文字色ID</param>
        /// <param name="nameColorCode">投稿者名文字色カラーコード</param>
        /// <param name="message">チャットメッセージ</param>
        public ChatLogModel(DateTime dateTime, string name, uint nameColorId, string nameColorCode, string message)
        {
            RegTime = dateTime;
            Name = name;
            NameColorId = nameColorId;
            NameColorCode = nameColorCode;
            Message = message;
        }
    }
}