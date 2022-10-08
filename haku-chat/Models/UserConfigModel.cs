using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace haku_chat.Models
{
    /// <summary>
    /// ユーザー設定管理用モデル
    /// </summary>
    [DisplayName("ユーザー設定管理テーブル")]
    [Table("user_config")]
    public class UserConfigModel
    {
        /// <summary>
        /// ユーザーID（システム）
        /// </summary>
        [Key]
        [DisplayName("ユーザーID（システム）")]
        [Required(ErrorMessage = "{0}は必須です。")]
        [StringLength(95, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
        [Column("id", Order = 1)]
        public string Id { get; set; }

        /// <summary>
        /// 投稿者名文字色ID
        /// </summary>
        [DisplayName("投稿者名文字色ID")]
        [Required(ErrorMessage = "{0}は必須です。")]
        [Column("name_color_id", Order = 2)]
        public uint NameColorId { get; set; }

        /// <summary>
        /// 通知メール送信フラグ
        /// </summary>
        [DisplayName("通知メール送信フラグ")]
        [Required(ErrorMessage = "{0}は必須です。")]
        [Column("send_info_mail", Order = 3)]
        public bool SendInfoMail { get; set; }

        /// <summary>
        /// チャットログ表示件数
        /// </summary>
        [DisplayName("チャットログ表示件数")]
        [Required(ErrorMessage = "{0}は必須です。")]
        [Column("show_chat_log_count", Order = 4)]
        public uint ShowChatLogCount { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>
        [DisplayName("更新日時")]
        [Required(ErrorMessage = "{0}は必須です。")]
        [Column("update_datetime", Order = 5)]
        public DateTime UpdateDatetime { get; set; }

        /// <summary>
        /// コンストラクター
        /// 　引数未指定時は最終アクセス日時を現在時刻で初期化
        /// </summary>
        public UserConfigModel()
        {
            UpdateDatetime = DateTime.Now;
        }

    }
}
