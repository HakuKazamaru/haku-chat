using Microsoft.Extensions.Logging;

using System;
using System.Threading.Tasks;

using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

using NLog;
using NLog.Web;
using Microsoft.Extensions.Configuration;

namespace haku_chat.Common.Utility
{
    /// <summary>
    /// メール送信機能
    /// </summary>
    public class Mail
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static Logger logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        /// <summary>
        /// メール送信メソッド
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task SendEmailAsync(IConfiguration configuration, string email, string subject, string message)
        {
            string fromAddress = Config.GetAppsettingsToSectionStringValue(configuration,"FromAddress");
            string serverAddress = Config.GetAppsettingsToSectionStringValue(configuration, "SmtpServerAddress");
            string serverPortStr = Config.GetAppsettingsToSectionStringValue(configuration, "SmtpServerPort");
            string secureSocketOption = Config.GetAppsettingsToSectionStringValue(configuration, "SmtpSecureSocketOption");
            string userID = Config.GetAppsettingsToSectionStringValue(configuration, "SmtpUserID");
            string password = Config.GetAppsettingsToSectionStringValue(configuration, "SmtpPassword");

            int portNum = 25;
            SecureSocketOptions secureSocketOptions = SecureSocketOptions.Auto;

            logger.Info("========== Func Start! ==================================================");
            logger.Debug("Parameter[email]             ：{0}", email);
            logger.Debug("Parameter[subject]           ：{0}", subject);
            logger.Debug("Parameter[message]           ：{0}", message);
            logger.Debug("Config   [fromAddress]       ：{0}", fromAddress);
            logger.Debug("Config   [serverAddress]     ：{0}", serverAddress);
            logger.Debug("Config   [serverPortStr]     ：{0}", serverPortStr);
            logger.Debug("Config   [secureSocketOption]：{0}", secureSocketOption);
            logger.Debug("Config   [userID]            ：{0}", userID);
            logger.Trace("Config   [password]          ：{0}", password);

            if (!int.TryParse(serverPortStr, out portNum))
            {
                portNum = 25;
                logger.Warn("SMTPサーバーポートに数値以外が設定されています。25番ポートを使用します。");
            }

            switch (secureSocketOption.ToLower())
            {
                case "none":
                    {
                        secureSocketOptions = SecureSocketOptions.None;
                        break;
                    }
                case "auto":
                    {
                        secureSocketOptions = SecureSocketOptions.Auto;
                        break;
                    }
                case "ssl":
                    {
                        secureSocketOptions = SecureSocketOptions.SslOnConnect;
                        break;
                    }
                case "tls":
                    {
                        secureSocketOptions = SecureSocketOptions.StartTls;
                        break;
                    }
                case "tls_available":
                    {
                        secureSocketOptions = SecureSocketOptions.StartTlsWhenAvailable;
                        break;
                    }
                default:
                    {
                        logger.Warn("SMTPサーバーセキュア接続設定に不正な値が設定されています。自動識別を使用します。");
                        secureSocketOptions = SecureSocketOptions.Auto;
                        break;
                    }
            }

            try
            {
                // メールのオブジェクトを作成する
                var emailMessage = new MimeMessage();
                // メール送信元の名前とメールアドレスを指定する
                emailMessage.From.Add(new MailboxAddress("Haku's Chat System", fromAddress));
                // メール送信先を指定する
                emailMessage.To.Add(new MailboxAddress("", email));
                // メールの件名を設定する
                emailMessage.Subject = subject;
                // メールの本文を指定する（テキストメッセージ）
                emailMessage.Body = new TextPart("plain") { Text = message };

                using (var client = new SmtpClient())
                {
                    // SMTPサーバに接続する（SSL有効）
                    await client.ConnectAsync(serverAddress, portNum, secureSocketOptions);

                    // SMTPサーバで認証する（IDとパスワード）
                    await client.AuthenticateAsync(userID, password);

                    // メールを送信する
                    await client.SendAsync(emailMessage);

                    // SMTPサーバとの接続を切る
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, string.Format("SendEmailAsyncでエラーが発生しました。{0}", ex.Message));
            }

        }

        /// <summary>
        /// 入室通知送信メソッド
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="email"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async Task SendLoginInfoMailAsync(IConfiguration configuration, string email, string name)
        {
            logger.Info("========== Func Start! ==================================================");
            string url = Config.GetAppsettingsToSectionStringValue(configuration, "LoginFullUrl");
            string title = "[入室通知]Haku's Chat System";
            string body = "チャット入室通知：{0}\r\n"
                + "{1}さんが入室しました。\r\n"
                + "\r\n"
                + "-------------------------\r\n"
                + "入室URL：{2}";

            await SendEmailAsync(configuration, email, title, String.Format(body, DateTime.Now, name, url));
        }


    }
}
