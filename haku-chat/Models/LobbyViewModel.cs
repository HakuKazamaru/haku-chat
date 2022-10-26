using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.AspNetCore.Authentication;

namespace haku_chat.Models
{
    public class LobbyViewModel
    {
        /// <summary>
        /// ユーザーがログイン前にアクセスしたURLを入れる
        /// ログイン完了後にリダイレクトするURL
        /// </summary>
        public string ReturnUrl { get; set; }
        /// <summary>
        /// 外部認証
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        /// 利用規約
        /// </summary>
        public  PolicyModel ServicePolicy { get; set; }

        /// <summary>
        /// プライバシーポリシー
        /// </summary>
        public PolicyModel PrivacyPolicy { get; set; }

        /// <summary>
        /// クッキーポリシー
        /// </summary>
        public PolicyModel CookiePolicy { get; set; }
    }
}
