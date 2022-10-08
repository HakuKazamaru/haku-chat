using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using NLog;
using NLog.Web;
using NLog.Extensions.Logging;

using haku_chat.Models;
using haku_chat.DbContexts;

namespace haku_chat.Common.DataBase.Store
{
    /// <summary>
    /// ユーザーマスター用ストアークラス
    /// </summary>
    public class UserMasterStore : IUserStore<UserMasterModel>,
                                   IUserPasswordStore<UserMasterModel>,
                                   IUserEmailStore<UserMasterModel>,
                                   IUserLoginStore<UserMasterModel>,
                                   IQueryableUserStore<UserMasterModel>
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static Logger logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private ChatDbContext dbContext;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="dbContext"></param>
        public UserMasterStore(ChatDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// デスコンストラクター
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// 解放処理
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                    dbContext = null;
                }
            }
        }

        /// <summary>
        /// IQueryableUserStore<User> のメンバー
        /// </summary>
        public IQueryable<UserMasterModel> Users
        {
            get { return dbContext.UserMaster.Select(u => u); }
        }

        /// <summary>
        /// ユーザーマスターの挿入
        /// </summary>
        /// <param name="userMasterModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IdentityResult> CreateAsync(UserMasterModel userMasterModel, CancellationToken cancellationToken = default)
        {
            logger.Debug("========== Func Call!  ==================================================");
            cancellationToken.ThrowIfCancellationRequested();
            if (userMasterModel == null) throw new ArgumentNullException(nameof(userMasterModel));
            logger.Debug("Id          :{0}", userMasterModel.Id);
            logger.Debug("UserId      :{0}", userMasterModel.UserId);
            logger.Debug("UserName    :{0}", userMasterModel.UserName);
            logger.Debug("Email       :{0}", userMasterModel.Email);
            logger.Debug("Password    :{0}", userMasterModel.Password);
            logger.Debug("PasswordHash:{0}", userMasterModel.PasswordHash);

            dbContext.Add(userMasterModel);

            int rows = await dbContext.SaveChangesAsync(cancellationToken);

            if (rows > 0)
            {
                await SetPasswordHashAsync(userMasterModel, userMasterModel.Password, cancellationToken);
                return IdentityResult.Success;
            }

            return IdentityResult.Failed(new IdentityError { Description = $"{userMasterModel.UserName} 登録失敗" });
        }

        /// <summary>
        /// ユーザーマスターの更新
        /// </summary>
        /// <param name="userMasterModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IdentityResult> UpdateAsync(UserMasterModel userMasterModel, CancellationToken cancellationToken = default)
        {
            logger.Debug("========== Func Call!  ==================================================");
            cancellationToken.ThrowIfCancellationRequested();
            if (userMasterModel == null) throw new ArgumentNullException(nameof(userMasterModel));
            logger.Debug("Id          :{0}", userMasterModel.Id);
            logger.Debug("UserId      :{0}", userMasterModel.UserId);
            logger.Debug("UserName    :{0}", userMasterModel.UserName);
            logger.Debug("Email       :{0}", userMasterModel.Email);
            logger.Debug("Password    :{0}", userMasterModel.Password);
            logger.Debug("PasswordHash:{0}", userMasterModel.PasswordHash);

            dbContext.Update(userMasterModel);

            int rows = await dbContext.SaveChangesAsync(cancellationToken);

            if (rows > 0)
            {
                return IdentityResult.Success;
            }

            return IdentityResult.Failed(new IdentityError { Description = $"{userMasterModel.UserName} 更新失敗" });
        }

        /// <summary>
        /// ユーザーマスターの削除
        /// </summary>
        /// <param name="userMasterModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IdentityResult> DeleteAsync(UserMasterModel userMasterModel, CancellationToken cancellationToken = default)
        {
            logger.Debug("========== Func Call!  ==================================================");
            cancellationToken.ThrowIfCancellationRequested();
            if (userMasterModel == null) throw new ArgumentNullException(nameof(userMasterModel));
            logger.Debug("Id          :{0}", userMasterModel.Id);
            logger.Debug("UserId      :{0}", userMasterModel.UserId);
            logger.Debug("UserName    :{0}", userMasterModel.UserName);
            logger.Debug("Email       :{0}", userMasterModel.Email);
            logger.Debug("Password    :{0}", userMasterModel.Password);
            logger.Debug("PasswordHash:{0}", userMasterModel.PasswordHash);

            dbContext.Remove(userMasterModel);

            int rows = await dbContext.SaveChangesAsync(cancellationToken);

            if (rows > 0)
            {
                return IdentityResult.Success;
            }

            return IdentityResult.Failed(new IdentityError { Description = $"{userMasterModel.UserName} 削除失敗" });
        }

        /// <summary>
        /// ユーザーID（システム）で検索する
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<UserMasterModel> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            logger.Debug("========== Func Call!  ==================================================");
            cancellationToken.ThrowIfCancellationRequested();

            var user = await dbContext.UserMaster.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user == null) { return null; }

            return user;

        }

        /// <summary>
        /// ユーザー名で検索する
        /// </summary>
        /// <param name="normalizedUserName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<UserMasterModel> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
        {
            logger.Debug("========== Func Call!  ==================================================");
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(normalizedUserName)) throw new ArgumentException("normalizedUserName");
            return await dbContext.UserMaster.SingleOrDefaultAsync(u => u.UserName == normalizedUserName, cancellationToken);
        }

        /// <summary>
        /// EmailAddressで検索する
        /// </summary>
        /// <param name="email"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<UserMasterModel> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            logger.Debug("========== Func Call!  ==================================================");
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrEmpty(email)) throw new ArgumentException("email");

            var user = await dbContext.UserMaster.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
            if (user == null) { return null; }

            return user;
        }

        /// <summary>
        /// パスワード(ハッシュ化済み)を取得する
        /// </summary>
        /// <param name="userMasterModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string> GetPasswordHashAsync(UserMasterModel userMasterModel, CancellationToken cancellationToken = default)
        {
            logger.Debug("========== Func Call!  ==================================================");
            if (userMasterModel == null) throw new ArgumentNullException(nameof(userMasterModel));
            return Task.Run(() => userMasterModel.PasswordHash, cancellationToken);
        }

        /// <summary>
        /// パスワードがハッシュ化されているか取得する
        /// </summary>
        /// <param name="userMasterModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> HasPasswordAsync(UserMasterModel userMasterModel, CancellationToken cancellationToken = default)
        {
            logger.Debug("========== Func Call!  ==================================================");
            cancellationToken.ThrowIfCancellationRequested();
            if (userMasterModel == null) throw new ArgumentNullException(nameof(userMasterModel));
            return Task.FromResult(!string.IsNullOrWhiteSpace(userMasterModel.PasswordHash));
        }

        /// <summary>
        /// パスワードをハッシュ化して格納する
        /// </summary>
        /// <param name="userMasterModel"></param>
        /// <param name="passwordHash"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SetPasswordHashAsync(UserMasterModel userMasterModel, string passwordHash, CancellationToken cancellationToken = default)
        {
            logger.Debug("========== Func Start! ==================================================");
            cancellationToken.ThrowIfCancellationRequested();
            if (userMasterModel == null) throw new ArgumentNullException(nameof(userMasterModel));
            if (string.IsNullOrEmpty(passwordHash)) throw new ArgumentException("passwordHash");
            await Task.Run(async () =>
            {
                var target = await this.FindByIdAsync(userMasterModel.Id, cancellationToken);
                if (target != null)
                {
                    target.PasswordHash = new PasswordHasher<UserMasterModel>().HashPassword(userMasterModel, passwordHash);

                    int rows = await dbContext.SaveChangesAsync(cancellationToken);

                    if (rows > 0)
                    {
                        logger.Debug("データ更新に成功しました。ID：{0}", userMasterModel.Id);
                    }
                    else
                    {
                        logger.Error("データ更新に失敗しました。ID：{0}", userMasterModel.Id);
                    }

                }
                else
                {
                    logger.Error("更新対象が存在しません。ID：{0}", userMasterModel.Id);
                }
            }, cancellationToken);
            logger.Debug("========== Func End!   ==================================================");
        }

        /// <summary>
        /// ユーザーID（システム）を取得する
        /// </summary>
        /// <param name="userMasterModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> GetUserIdAsync(UserMasterModel userMasterModel, CancellationToken cancellationToken = default)
        {
            logger.Debug("========== Func Call!  ==================================================");
            cancellationToken.ThrowIfCancellationRequested();
            if (userMasterModel == null) throw new ArgumentNullException(nameof(userMasterModel));

            var user = await dbContext.UserMaster.SingleOrDefaultAsync(u => u.Id == userMasterModel.Id, cancellationToken);
            if (user == null) return null;

            return user.Id;
        }

        /// <summary>
        /// ユーザー名を取得する
        /// </summary>
        /// <param name="userMasterModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> GetUserNameAsync(UserMasterModel userMasterModel, CancellationToken cancellationToken = default)
        {
            logger.Debug("========== Func Call!  ==================================================");
            cancellationToken.ThrowIfCancellationRequested();
            if (userMasterModel == null) throw new ArgumentNullException(nameof(userMasterModel));

            var user = await dbContext.UserMaster.SingleOrDefaultAsync(u => u.Id == userMasterModel.Id, cancellationToken);
            if (user == null) return userMasterModel.Email;

            return user.UserName;
        }

        /// <summary>
        /// ユーザー名を設定する
        /// </summary>
        /// <param name="userMasterModel"></param>
        /// <param name="userName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task SetUserNameAsync(UserMasterModel userMasterModel, string userName, CancellationToken cancellationToken = default)
        {
            logger.Debug("========== Func Start! ==================================================");
            cancellationToken.ThrowIfCancellationRequested();
            if (userMasterModel == null) throw new ArgumentNullException(nameof(userMasterModel));
            if (string.IsNullOrEmpty(userName)) throw new ArgumentException("userName");
            await Task.Run(async () =>
            {
                var target = await this.FindByIdAsync(userMasterModel.Id);
                if (target != null)
                {
                    target.UserName = userName;

                    int rows = await dbContext.SaveChangesAsync(cancellationToken);
                    if (rows > 0)
                    {
                        logger.Debug("データ更新に成功しました。ID：{0}", userMasterModel.Id);
                    }
                    else
                    {
                        logger.Error("データ更新に失敗しました。ID：{0}", userMasterModel.Id);
                    }

                }
                else
                {
                    logger.Error("データ更新に失敗しました。ID：{0}", userMasterModel.Id);
                }

            }, cancellationToken);
            logger.Debug("========== Func End!   ==================================================");
        }

        /// <summary>
        /// ユーザー名を取得する
        /// </summary>
        /// <param name="userMasterModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> GetNormalizedUserNameAsync(UserMasterModel userMasterModel, CancellationToken cancellationToken = default)
        {
            logger.Debug("========== Func Call!  ==================================================");
            cancellationToken.ThrowIfCancellationRequested();
            if (userMasterModel == null) throw new ArgumentNullException(nameof(userMasterModel));

            var user = await dbContext.UserMaster.SingleOrDefaultAsync(x => x.Id == userMasterModel.Id, cancellationToken);
            if (user == null) { return null; }

            return user.UserName;
        }

        /// <summary>
        /// ユーザー名を設定する
        /// </summary>
        /// <param name="userMasterModel"></param>
        /// <param name="normalizedName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SetNormalizedUserNameAsync(UserMasterModel userMasterModel, string normalizedName, CancellationToken cancellationToken = default)
        {
            logger.Debug("========== Func Start! ==================================================");
            if (userMasterModel == null) throw new ArgumentNullException(nameof(userMasterModel));
            if (string.IsNullOrEmpty(normalizedName)) throw new ArgumentException("normalizedName");
            await Task.Run(async () =>
            {
                var target = await this.FindByIdAsync(userMasterModel.Id);
                if (target != null)
                {
                    target.UserName = normalizedName;

                    int rows = await dbContext.SaveChangesAsync(cancellationToken);

                    if (rows > 0)
                    {
                        logger.Debug("データ更新に成功しました。ID：{0}", userMasterModel.Id);
                    }
                    else
                    {
                        logger.Error("データ更新に失敗しました。ID：{0}", userMasterModel.Id);
                    }
                }
                else
                {
                    logger.Error("データ更新に失敗しました。ID：{0}", userMasterModel.Id);
                }
            }, cancellationToken);
            logger.Debug("========== Func End!   ==================================================");
        }

        /// <summary>
        /// メールアドレスを取得する
        /// </summary>
        /// <param name="userMasterModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> GetEmailAsync(UserMasterModel userMasterModel, CancellationToken cancellationToken = default)
        {
            logger.Debug("========== Func Call!  ==================================================");
            cancellationToken.ThrowIfCancellationRequested();
            if (userMasterModel == null) throw new ArgumentNullException(nameof(userMasterModel));

            var user = await dbContext.UserMaster.SingleOrDefaultAsync(u => u.Id == userMasterModel.Id, cancellationToken);
            if (user == null) { return userMasterModel.Email; }

            return user.Email;
        }

        /// <summary>
        /// メールアドレスを設定する
        /// </summary>
        /// <param name="userMasterModel"></param>
        /// <param name="email"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task SetEmailAsync(UserMasterModel userMasterModel, string email, CancellationToken cancellationToken = default)
        {
            logger.Debug("========== Func Start! ==================================================");
            cancellationToken.ThrowIfCancellationRequested();
            if (userMasterModel == null) throw new ArgumentNullException(nameof(userMasterModel));
            if (string.IsNullOrEmpty(email)) throw new ArgumentException("email");
            await Task.Run(async () =>
            {
                var target = await this.FindByIdAsync(userMasterModel.Id);
                if (target != null)
                {
                    target.Email = email;

                    int rows = await dbContext.SaveChangesAsync(cancellationToken);
                    if (rows > 0)
                    {
                        logger.Debug("データ更新に成功しました。ID：{0}", userMasterModel.Id);
                    }
                    else
                    {
                        logger.Error("データ更新に失敗しました。ID：{0}", userMasterModel.Id);
                    }

                }
                else
                {
                    logger.Error("データ更新に失敗しました。ID：{0}", userMasterModel.Id);
                }

            }, cancellationToken);
            logger.Debug("========== Func End!   ==================================================");
        }

        /// <summary>
        /// メールアドレスが確認済みか取得する
        /// </summary>
        /// <param name="userMasterModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> GetEmailConfirmedAsync(UserMasterModel userMasterModel, CancellationToken cancellationToken = default)
        {
            logger.Debug("========== Func Call!  ==================================================");
            cancellationToken.ThrowIfCancellationRequested();
            if (userMasterModel == null) throw new ArgumentNullException(nameof(userMasterModel));

            var user = await dbContext.UserMaster.SingleOrDefaultAsync(u => u.Id == userMasterModel.Id, cancellationToken);
            if (user == null) { return false; }

            return user.EmailConfirmed;
        }

        /// <summary>
        /// メールアドレスが確認済みか設定する
        /// </summary>
        /// <param name="userMasterModel"></param>
        /// <param name="email"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task SetEmailConfirmedAsync(UserMasterModel userMasterModel, bool flag, CancellationToken cancellationToken = default)
        {
            logger.Debug("========== Func Start! ==================================================");
            cancellationToken.ThrowIfCancellationRequested();
            if (userMasterModel == null) throw new ArgumentNullException(nameof(userMasterModel));
            await Task.Run(async () =>
            {
                var target = await this.FindByIdAsync(userMasterModel.Id);
                if (target != null)
                {
                    target.EmailConfirmed = flag;

                    int rows = await dbContext.SaveChangesAsync(cancellationToken);
                    if (rows > 0)
                    {
                        logger.Debug("データ更新に成功しました。ID：{0}", userMasterModel.Id);
                    }
                    else
                    {
                        logger.Error("データ更新に失敗しました。ID：{0}", userMasterModel.Id);
                    }

                }
                else
                {
                    logger.Error("データ更新に失敗しました。ID：{0}", userMasterModel.Id);
                }

            }, cancellationToken);
            logger.Debug("========== Func End!   ==================================================");
        }

        /// <summary>
        /// メールアドレスを取得する
        /// </summary>
        /// <param name="userMasterModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> GetNormalizedEmailAsync(UserMasterModel userMasterModel, CancellationToken cancellationToken = default)
        {
            logger.Debug("========== Func Call!  ==================================================");
            cancellationToken.ThrowIfCancellationRequested();
            if (userMasterModel == null) throw new ArgumentNullException(nameof(userMasterModel));

            var user = await dbContext.UserMaster.SingleOrDefaultAsync(u => u.Id == userMasterModel.Id, cancellationToken);
            if (user == null) { return null; }

            return user.NormalizedEmail;
        }

        /// <summary>
        /// メールアドレスを設定する
        /// </summary>
        /// <param name="userMasterModel"></param>
        /// <param name="email"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task SetNormalizedEmailAsync(UserMasterModel userMasterModel, string email, CancellationToken cancellationToken = default)
        {
            logger.Debug("========== Func Start! ==================================================");
            cancellationToken.ThrowIfCancellationRequested();
            if (userMasterModel == null) throw new ArgumentNullException(nameof(userMasterModel));
            if (string.IsNullOrEmpty(email)) throw new ArgumentException("email");
            await Task.Run(async () =>
            {
                var target = await this.FindByIdAsync(userMasterModel.Id);
                if (target != null)
                {
                    target.Email = email;

                    int rows = await dbContext.SaveChangesAsync(cancellationToken);
                    if (rows > 0)
                    {
                        logger.Debug("データ更新に成功しました。ID：{0}", userMasterModel.Id);
                    }
                    else
                    {
                        logger.Error("データ更新に失敗しました。ID：{0}", userMasterModel.Id);
                    }

                }
                else
                {
                    logger.Error("データ更新に失敗しました。ID：{0}", userMasterModel.Id);
                }

            }, cancellationToken);
            logger.Debug("========== Func End!   ==================================================");
        }

        public Task AddLoginAsync(UserMasterModel user, UserLoginInfo login, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task RemoveLoginAsync(UserMasterModel user, string loginProvider, string providerKey, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(UserMasterModel user, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UserMasterModel> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken = default)
        {
            logger.Debug("========== Func Start! ==================================================");
            logger.Debug("ログインプロバイダー名　　　　：", loginProvider);
            logger.Debug("ログインプロバイダーユーザ情報：", providerKey);
            logger.Debug("========== Func End!   ==================================================");
            cancellationToken.ThrowIfCancellationRequested();
            return Task.Run(() => dbContext.UserMaster.FirstOrDefault(u => u.Id == string.Format("{0}:{1}", loginProvider, providerKey)), cancellationToken);
        }

    }
}
