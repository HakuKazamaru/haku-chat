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

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
    }
}
