using Microsoft.AspNetCore.Identity;

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace haku_chat.Models
{
    /// <summary>
    /// ユーザー管理用モデル
    /// </summary>
    [DisplayName("ユーザーマスターテーブル")]
    [Table("user_master")]
    public class UserMasterModel : IdentityUser
    {
        /// <summary>
        /// ユーザーID（システム）
        /// </summary>
        [Key]
        [DisplayName("ユーザーID（システム）")]
        [Required(ErrorMessage = "{0}は必須です。")]
        [Column("id", Order = 1)]
        public override string Id { get; set; }

        /// <summary>
        /// ユーザーID（ユーザー）
        /// </summary>
        [DisplayName("ユーザーID（ユーザー）")]
        [Required(ErrorMessage = "{0}は必須です。")]
        [Column("user_id", Order = 2)]
        public string UserId { get; set; }

        /// <summary>
        /// ユーザー名
        /// </summary>
        [DisplayName("ユーザー名")]
        [Required(ErrorMessage = "{0}は必須です。")]
        [Column("user_name")]
        public override string UserName { get; set; }

        /// <summary>
        /// パスワード
        /// </summary>
        [DisplayName("パスワード")]
        [Column("password")]
        public string? Password { get; set; }

        /// <summary>
        /// ユーザー名(一意)
        /// </summary>
        [NotMapped] public override string NormalizedUserName { get; set; }

        /// <summary>
        /// メールアドレス
        /// </summary>
        [DisplayName("メールアドレス")]
        [Required(ErrorMessage = "{0}は必須です。")]
        [Column("email")]
        public override string Email { get; set; }

        /// <summary>
        /// メールアドレス(一意)
        /// </summary>
        [NotMapped] public override string NormalizedEmail { get; set; }

        /// <summary>
        /// メールアドレス認証済みフラグ
        /// </summary>
        [NotMapped] public override bool EmailConfirmed { get; set; }

        /// <summary>
        /// パスワードハッシュ
        /// </summary>
        [DisplayName("パスワードハッシュ")]
        [Column("passwordhash")]
        public override string? PasswordHash { get; set; }

        [NotMapped] public override string SecurityStamp { get; set; }
        [NotMapped] public override string ConcurrencyStamp { get; set; }
        [NotMapped] public override string PhoneNumber { get; set; }
        [NotMapped] public override bool PhoneNumberConfirmed { get; set; }
        [NotMapped] public override bool TwoFactorEnabled { get; set; }
        [NotMapped] public override DateTimeOffset? LockoutEnd { get; set; }
        [NotMapped] public override bool LockoutEnabled { get; set; }
        [NotMapped] public override int AccessFailedCount { get; set; }
    }
}
