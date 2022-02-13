using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;
using NLog;
using NLog.Web;

namespace haku_chat.Common
{
    public class ChatLog
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static Logger logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        /// <summary>
        /// チャットログ取得メソッド
        /// </summary>
        /// <param name="limit">取得件数制限</param>
        /// <returns></returns>
        public static List<Models.ChatLogModel> GetChatLog(int limit)
        {
            List<Models.ChatLogModel> chatLogs = new List<Models.ChatLogModel>();
            logger.Info("========== Func Start! ==================================================");
            try
            {
                StringBuilder sqlString = new StringBuilder();
                MySQLConnecter connecter = new MySQLConnecter(
                    "192.168.101.251", "3306", "haku-chat", "haku-chat", "haku-chat-db"
                    );

                sqlString.Append("select ");
                sqlString.Append(" cl.reg_date, cl.name, cl.name_color_id, cncm.code as name_color_code, cl.message ");
                sqlString.Append("from ");
                sqlString.Append(" chat_log as cl ");
                sqlString.Append("inner join chat_name_color_master cncm ");
                sqlString.Append(" on cncm.id = cl.name_color_id ");
                sqlString.Append("order by ");
                sqlString.Append(" reg_date desc ");
                sqlString.Append("LIMIT " + limit + ";");

                logger.Debug("SQL:{0}", sqlString.ToString());

                using (MySqlDataReader dataReader = connecter.ExecuteSql(sqlString.ToString()))
                {
                    while (dataReader.Read())
                    {
                        Models.ChatLogModel chatLog = new Models.ChatLogModel();
                        chatLog.RegTime = dataReader.GetDateTime("reg_date");
                        chatLog.Name = dataReader.GetString("name");
                        chatLog.NameColorId = dataReader.GetUInt32("name_color_id");
                        chatLog.NameColorCode = dataReader.GetString("name_color_code");
                        chatLog.Message = dataReader.GetString("message");
                        chatLogs.Add(chatLog);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            logger.Info("========== Func End!   ==================================================");
            return chatLogs;
        }

        /// <summary>
        /// チャット投稿メソッド
        /// </summary>
        /// <param name="name">名前</param>
        /// <param name="nameColor">名前色</param>
        /// <param name="message">チャット文章</param>
        /// <param name="messageColor">チャット文章色</param>
        /// <returns></returns>
        public static bool PostChatLog(string name, uint nameColor, string message, uint messageColor)
        {
            bool retVal = false;
            logger.Info("========== Func Start! ==================================================");
            try
            {
                int instVal = -1;
                StringBuilder sqlString = new StringBuilder();
                Common.MySQLConnecter connecter = new Common.MySQLConnecter(
                    "192.168.101.251", "3306", "haku-chat", "haku-chat", "haku-chat-db"
                    );
                List<MySqlParameter> mySqlParameterCollection = new List<MySqlParameter>();
                MySqlParameter mySqlParameter = new MySqlParameter();

                sqlString.Append("insert into ");
                sqlString.Append(" chat_log ");
                sqlString.Append("( ");
                sqlString.Append(" reg_date, name, name_color_id, message ");
                sqlString.Append(") value ( ");
                sqlString.Append(" now(), @name, @name_color_id, @message ");
                sqlString.Append(");");

                mySqlParameter = new MySqlParameter("name", MySqlDbType.VarChar);
                mySqlParameter.Value = name;
                mySqlParameterCollection.Add(mySqlParameter);

                mySqlParameter = new MySqlParameter("name_color_id", MySqlDbType.VarChar);
                mySqlParameter.Value = nameColor;
                mySqlParameterCollection.Add(mySqlParameter);

                mySqlParameter = new MySqlParameter("message", MySqlDbType.VarChar);
                mySqlParameter.Value = message;
                mySqlParameterCollection.Add(mySqlParameter);

                logger.Debug("SQL:{0}", sqlString.ToString());
                foreach (var objMySqlParameter in mySqlParameterCollection.Select((item, index) => new { item, index }))
                {
                    if (objMySqlParameter.item.Value != null)
                    {
                        logger.Debug("Value[{0}]:{1}", objMySqlParameter.index, objMySqlParameter.item.Value.ToString());
                    }
                    else
                    {
                        logger.Debug("Value[{0}]:DBNull", objMySqlParameter.index);
                    }
                }

                instVal = connecter.ExecuteSqlNonQuery(sqlString.ToString(), mySqlParameterCollection);

                if (instVal == 1)
                {
                    retVal = true;
                }
                else
                {
                    retVal = false;
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retVal = false;
            }
            logger.Info("========== Func End!   ==================================================");
            return retVal;
        }
    }
}
