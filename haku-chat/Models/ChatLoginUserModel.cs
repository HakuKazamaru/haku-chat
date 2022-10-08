using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace haku_chat.Models
{
    /// <summary>
    /// チャット入室ステータス管理用モデル
    /// </summary>
    [DisplayName("チャット入室ステータステーブル")]
    [Table("chat_login_user")]
    public class ChatLoginUserModel
    {
#nullable enable
        /// <summary>
        /// ログインユーザー名
        /// </summary>
        [Key]
        [DisplayName("ログインユーザー名")]
        [Required(ErrorMessage = "{0}は必須です。")]
        [StringLength(100, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
        [Column("name", Order = 1)]
        public string Name { get; set; }

        /// <summary>
        /// ログインステータス
        /// </summary>
        [DisplayName("ログインステータス")]
        [Required(ErrorMessage = "{0}は必須です。")]
        [Column("status", Order = 2)]
        public uint Status { get; set; }

        /// <summary>
        /// IPアドレス
        /// </summary>
        [DisplayName("IPアドレス")]
        [StringLength(43, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
        [Column("ip_address", Order = 3)]
        public string? IpAddress { get; set; }

        /// <summary>
        /// ホスト名
        /// </summary>
        [DisplayName("ホスト名")]
        [Column("host_name", Order = 4)]
        public string? HostName { get; set; }

        /// <summary>
        /// コンピューター名
        /// </summary>
        [DisplayName("コンピューター名")]
        [StringLength(128, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
        [Column("machine_name", Order = 5)]
        public string? MachineName { get; set; }

        /// <summary>
        /// OS名称
        /// </summary>
        [DisplayName("OS名称")]
        [StringLength(128, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
        [Column("os_name", Order = 6)]
        public string? OsName { get; set; }

        /// <summary>
        /// ブラウザユーザーエージェント
        /// </summary>
        [DisplayName("ブラウザユーザーエージェント")]
        [StringLength(256, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
        [Column("user_agent", Order = 7)]
        public string? UserAgent { get; set; }

        /// <summary>
        /// ログイン日時
        /// </summary>
        [DisplayName("ログイン日時")]
        [Column("login_time", Order = 8)]
        public DateTime? LoginTime { get; set; }

        /// <summary>
        /// ログアウト日時
        /// </summary>
        [DisplayName("ログアウト日時")]
        [Column("logout_time", Order = 9)]
        public DateTime? LogoutTime { get; set; }

        /// <summary>
        /// 最終アクセス日時
        /// </summary>
        [DisplayName("最終アクセス日時")]
        [Required(ErrorMessage = "{0}は必須です。")]
        [Column("last_update", Order = 10)]
        public DateTime LastUpdate { get; set; }

#nullable disable
        /// <summary>
        /// コンストラクター
        /// 　引数未指定時は最終アクセス日時を現在時刻で初期化
        /// </summary>
        public ChatLoginUserModel()
        {
            LastUpdate = DateTime.Now;
        }

        /// <summary>
        /// コンストラクター
        /// 　引数にログインユーザー名のみ指定時は最終アクセス日時を現在時刻で初期化
        /// </summary>
        /// <param name="name"></param>
        public ChatLoginUserModel(string name)
        {
            Name = name;
            LastUpdate = DateTime.Now;
        }

    }

    /// <summary>
    /// ログインステータスコード定義体
    /// </summary>
    public enum ChatLoginStatus : uint
    {
        Logout = 0,
        Login = 1
    }
}
