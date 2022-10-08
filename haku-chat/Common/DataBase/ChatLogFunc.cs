using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using NLog;
using NLog.Web;

using haku_chat.DbContexts;

namespace haku_chat.Common.DataBase
{
    /// <summary>
    /// 入室・退室システム処理用
    /// </summary>
    public static class ChatLogFunc
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static Logger logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        /// <summary>
        /// ログイン処理
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="dbContext"></param>
        /// <param name="name"></param>
        /// <param name="nameColor"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static int LoginChatRoom(HttpContext httpContext,
            ChatDbContext dbContext,
            string name,
            uint nameColor = (uint)NameColor.ColorCodeID.BLACK,
            string message = "")
        {
            int retVal = -1;
            logger.Info("========== Func Start! ==================================================");
            logger.Debug("Parameter[name]     :{0}", name);
            logger.Debug("Parameter[nameColor]:{0}", nameColor);
            logger.Debug("Parameter[message]  :{0}", message);

            try
            {
                Models.ChatLoginUserModel chatLoginUser = dbContext
                    .ChatLoginUser
                    .SingleOrDefault(c => c.Name == name);

                // 入室状態をチェック
                if (chatLoginUser != null)
                {
                    if (chatLoginUser.Status == (uint)Models.ChatLoginStatus.Login)
                    {
                        logger.Debug("ログイン済みを確認しました。");
                        if ((DateTime.Now - chatLoginUser.LastUpdate).TotalSeconds >= (30 * 60))
                        {
                            logger.Debug("セッションがタイムアウトしています。");
                            retVal = 2;
                        }
                        else
                        {
                            logger.Debug("セッションが有効です。");
                            retVal = 1;
                        }
                    }
                    else
                    {
                        logger.Debug("ログアウト済みを確認しました。");
                        retVal = 0;
                    }
                }
                else
                {
                    logger.Debug("ログイン情報が見つかりませんでした。");
                    retVal = 3;
                }

                // 入室状態を更新
                if (retVal == 3)
                {
                    // 新規ユーザーの場合入室ステータステーブルのレコードを新規登録
                    logger.Info("セッションデータを新たに作成します。");

                    chatLoginUser = new Models.ChatLoginUserModel();

                    chatLoginUser.Status = (uint)Models.ChatLoginStatus.Login;
                    chatLoginUser.Name = name;
                    if (httpContext.Connection.RemoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        chatLoginUser.IpAddress = httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    }
                    else
                    {
                        chatLoginUser.IpAddress = httpContext.Connection.RemoteIpAddress.MapToIPv6().ToString();
                    }
                    chatLoginUser.UserAgent = httpContext.Request.Headers.UserAgent;
                    chatLoginUser.LoginTime = DateTime.Now;
                    chatLoginUser.LastUpdate = DateTime.Now;

                    dbContext.ChatLoginUser.Add(chatLoginUser);
                    dbContext.SaveChanges();
                    logger.Debug("入室処理に成功しました。");

                }
                else if (retVal == 1)
                {
                    // ログイン済みの場合、最終更新日時を更新
                    logger.Warn("すでにセッションが有効です。");

                    if (httpContext.Connection.RemoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        chatLoginUser.IpAddress = httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    }
                    else
                    {
                        chatLoginUser.IpAddress = httpContext.Connection.RemoteIpAddress.MapToIPv6().ToString();
                    }
                    chatLoginUser.UserAgent = httpContext.Request.Headers.UserAgent;
                    chatLoginUser.LastUpdate = DateTime.Now;

                    dbContext.SaveChanges();
                    logger.Debug("入室処理に成功しました。セッションが有効なためシステムメッセージを投稿しません。");

                    retVal = 0;
                }
                else if (retVal == 2)
                {
                    // セッションタイムアウトの場合、入室ステータスと入室日時、最終更新日時を更新
                    logger.Debug("ログイン処理を試行します。(セッションタイムアウト)");

                    if (httpContext.Connection.RemoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        chatLoginUser.IpAddress = httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    }
                    else
                    {
                        chatLoginUser.IpAddress = httpContext.Connection.RemoteIpAddress.MapToIPv6().ToString();
                    }
                    chatLoginUser.UserAgent = httpContext.Request.Headers.UserAgent;
                    chatLoginUser.LoginTime = DateTime.Now;
                    chatLoginUser.LastUpdate = DateTime.Now;

                    dbContext.SaveChanges();
                    logger.Debug("入室処理に成功しました。");
                }
                else if (retVal == 0)
                {
                    // ログアウト済みの場合
                    logger.Info("ログイン処理を試行します。(ログアウト)");

                    chatLoginUser.Status = (uint)Models.ChatLoginStatus.Login;
                    if (httpContext.Connection.RemoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        chatLoginUser.IpAddress = httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    }
                    else
                    {
                        chatLoginUser.IpAddress = httpContext.Connection.RemoteIpAddress.MapToIPv6().ToString();
                    }
                    chatLoginUser.UserAgent = httpContext.Request.Headers.UserAgent;
                    chatLoginUser.LoginTime = DateTime.Now;
                    chatLoginUser.LastUpdate = DateTime.Now;

                    dbContext.SaveChanges();
                    logger.Debug("入室処理に成功しました。");
                }
                else
                {
                    logger.Error("不明なエラーが発生しました。");
                }

                // 入室時のシステムメッセージ投稿
                if (retVal == 3 || retVal == 2 || retVal == 0)
                {
                    Models.ChatLogModel chatLogModel = new Models.ChatLogModel();
                    logger.Debug("システムメッセージを投稿します。");

                    chatLogModel.RegTime = DateTime.Now;
                    chatLogModel.Name = "システム";
                    chatLogModel.NameColorId = (uint)Common.NameColor.ColorCodeID.BLACK;
                    chatLogModel.Message = string.Format("{0}さんが入室しました。", name);

                    dbContext.ChatLog.Add(chatLogModel);
                    dbContext.SaveChanges();

                    logger.Debug("システムメッセージを投稿に成功しました。");
                }

                // 入室時のメッセージ投稿
                if (retVal >= 0 && retVal < 4 && !string.IsNullOrWhiteSpace(message))
                {
                    Models.ChatLogModel chatLogModel = new Models.ChatLogModel();
                    logger.Debug("入室時メッセージを投稿します。");

                    chatLogModel.RegTime = DateTime.Now;
                    chatLogModel.Name = name;
                    chatLogModel.NameColorId = nameColor;
                    chatLogModel.Message = message;

                    dbContext.ChatLog.Add(chatLogModel);
                    dbContext.SaveChanges();

                    logger.Debug("入室時メッセージを投稿に成功しました。");
                }

                // 正常終了判定
                if (retVal >= 0 && retVal < 4)
                {
                    // 正常終了を返す
                    retVal = 0;

                    if (Utility.Config.GetAppsettingsToSectionStringValue("UseMailInfo").ToLower() == "true")
                    {
                        // 正常終了時のみメール送信
                        SendLoginMail(dbContext, name);
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retVal = -1;
            }
            logger.Info("========== Func End!   ==================================================");

            return retVal;
        }

        /// <summary>
        /// ログアウト処理
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="dbContext"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int LogoutChatRoom(HttpContext httpContext, ChatDbContext dbContext, string name)
        {
            int retVal = -1;
            logger.Info("========== Func Start! ==================================================");
            logger.Debug("Parameter[name]     :{0}", name);

            try
            {
                Models.ChatLoginUserModel chatLoginUser = dbContext
                    .ChatLoginUser
                    .SingleOrDefault(c => c.Name == name);

                // 入室状態をチェック
                if (chatLoginUser != null)
                {
                    if (chatLoginUser.Status == (uint)Models.ChatLoginStatus.Login)
                    {
                        logger.Debug("ログイン済みを確認しました。");
                        if ((DateTime.Now - chatLoginUser.LastUpdate).TotalSeconds >= (30 * 60))
                        {
                            logger.Debug("セッションがタイムアウトしています。");
                            retVal = 2;
                        }
                        else
                        {
                            logger.Debug("セッションが有効です。");
                            retVal = 1;
                        }
                    }
                    else
                    {
                        logger.Debug("ログアウト済みを確認しました。");
                        retVal = 0;
                    }
                }
                else
                {
                    logger.Debug("ログイン情報が見つかりませんでした。");
                    retVal = 3;
                }

                // 入室状態を更新
                if (retVal == 3)
                {
                    // 新規ユーザーの場合
                    logger.Warn("ユーザーデータが存在しません。");
                    retVal = -2;
                }
                else if (retVal == 1)
                {
                    // ログイン済みの場合
                    logger.Info("有効なセッションを破棄します。");

                    chatLoginUser.Status = (uint)Models.ChatLoginStatus.Logout;
                    if (httpContext.Connection.RemoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        chatLoginUser.IpAddress = httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    }
                    else
                    {
                        chatLoginUser.IpAddress = httpContext.Connection.RemoteIpAddress.MapToIPv6().ToString();
                    }
                    chatLoginUser.UserAgent = httpContext.Request.Headers.UserAgent;
                    chatLoginUser.LogoutTime = DateTime.Now;
                    chatLoginUser.LastUpdate = DateTime.Now;

                    dbContext.SaveChanges();
                    logger.Debug("退出処理に成功しました。");

                    retVal = 1;
                }
                else if (retVal == 2)
                {
                    // セッションタイムアウトの場合
                    logger.Debug("セッションがタイムアウトしています。");

                    chatLoginUser.Status = (uint)Models.ChatLoginStatus.Logout;
                    if (httpContext.Connection.RemoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        chatLoginUser.IpAddress = httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    }
                    else
                    {
                        chatLoginUser.IpAddress = httpContext.Connection.RemoteIpAddress.MapToIPv6().ToString();
                    }
                    chatLoginUser.UserAgent = httpContext.Request.Headers.UserAgent;
                    chatLoginUser.LogoutTime = DateTime.Now;
                    chatLoginUser.LastUpdate = DateTime.Now;

                    dbContext.SaveChanges();

                    retVal = 0;
                }
                else if (retVal == 0)
                {
                    // ログアウト済みの場合
                    logger.Info("ログアウト済みです。");

                    if (httpContext.Connection.RemoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        chatLoginUser.IpAddress = httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    }
                    else
                    {
                        chatLoginUser.IpAddress = httpContext.Connection.RemoteIpAddress.MapToIPv6().ToString();
                    }
                    chatLoginUser.UserAgent = httpContext.Request.Headers.UserAgent;
                    chatLoginUser.LastUpdate = DateTime.Now;

                    dbContext.SaveChanges();

                    retVal = 0;
                }
                else
                {
                    logger.Error("不明なエラーが発生しました。");
                    retVal = -1;
                }

                // 退出時のシステムメッセージ投稿
                if (retVal == 1)
                {
                    Models.ChatLogModel chatLogModel = new Models.ChatLogModel();
                    logger.Debug("システムメッセージを投稿します。");

                    chatLogModel.RegTime = DateTime.Now;
                    chatLogModel.Name = "システム";
                    chatLogModel.NameColorId = (uint)NameColor.ColorCodeID.BLACK;
                    chatLogModel.Message = string.Format("{0}さんが退室しました。", name);

                    dbContext.ChatLog.Add(chatLogModel);
                    dbContext.SaveChanges();

                    logger.Debug("システムメッセージを投稿に成功しました。");
                }

                // 正常終了判定
                if (retVal >= 0 && retVal < 3)
                {
                    // 正常終了を返す
                    retVal = 0;
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retVal = -1;
            }

            logger.Info("========== Func End!   ==================================================");

            return retVal;
        }

        /// <summary>
        /// ログインメール一括送信メソッド
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="name"></param>
        private static async Task SendLoginMail(ChatDbContext dbContext, string name)
        {
            logger.Info("========== Func Start! ==================================================");
            var targetUsers = dbContext
                .UserMaster
                .Join(
                    dbContext.UserConfig,
                    um => um.Id,
                    uc => uc.Id,
                    (um, uc) => new
                    {
                        um.Email,
                        uc.SendInfoMail
                    }
                )
                .Where(l => l.SendInfoMail == true)
                .ToList();

            foreach (var targetUser in targetUsers.Select((item, index) => new { item, index }))
            {
                logger.Debug("[{0}]送信対象：{1}", targetUser.index, targetUser.item.Email);
                await Utility.Mail.SendLoginInfoMailAsync(targetUser.item.Email, name);
            }

            logger.Info("========== Func End!   ==================================================");
        }


    }
}
