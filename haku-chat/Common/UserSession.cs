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
    public class UserSession
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static Logger logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        /// <summary>
        /// ユーザーログイン情報の取得
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<Models.ChatLoginUserModel> GetUserLogin(string name)
        {
            List<Models.ChatLoginUserModel> retVal = new List<Models.ChatLoginUserModel>();

            try
            {
                int result = 0;
                StringBuilder sqlString = new StringBuilder();
                Common.MySQLConnecter connecter = new Common.MySQLConnecter(
                    "192.168.101.251", "3306", "haku-chat", "haku-chat", "haku-chat-db"
                    );
                List<MySqlParameter> mySqlParameterCollection = new List<MySqlParameter>();
                MySqlParameter mySqlParameter = new MySqlParameter();

                sqlString.Append("select ");
                sqlString.Append(" clu.name, clu.status, clu.ip_address, clu.host_name, clu.machine_name, clu.os_name, clu.user_agent, clu.login_time, clu.logout_time, clu.last_update ");
                sqlString.Append("from ");
                sqlString.Append(" chat_login_user as clu ");
                sqlString.Append("where ");
                sqlString.Append(" clu.name = @name ");
                sqlString.Append("LIMIT 1;");

                mySqlParameter = new MySqlParameter("name", MySqlDbType.VarChar);
                mySqlParameter.Value = name;
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

                using (MySqlDataReader dataReader = connecter.ExecuteSql(sqlString.ToString(), mySqlParameterCollection))
                {
                    while (dataReader.Read())
                    {
                        Models.ChatLoginUserModel chatLoginUserModel = new Models.ChatLoginUserModel();

                        chatLoginUserModel.Name = dataReader.GetString("name");
                        if (!DBNull.Value.Equals(dataReader.GetValue("status")))
                        {
                            chatLoginUserModel.Status = dataReader.GetUInt32("status");
                        }
                        else
                        {
                            chatLoginUserModel.Status = 255;
                        }
                        chatLoginUserModel.IpAddress = dataReader.GetString("ip_address");
                        chatLoginUserModel.HostName = dataReader.GetString("host_name");
                        chatLoginUserModel.MachineName = dataReader.GetString("machine_name");
                        chatLoginUserModel.OsName = dataReader.GetString("os_name");
                        chatLoginUserModel.UserAgent = dataReader.GetString("user_agent");
                        if (!DBNull.Value.Equals(dataReader.GetValue("login_time")))
                        {
                            chatLoginUserModel.LastUpdate = dataReader.GetDateTime("login_time");
                        }
                        if (!DBNull.Value.Equals(dataReader.GetValue("logout_time")))
                        {
                            chatLoginUserModel.LastUpdate = dataReader.GetDateTime("logout_time");
                        }
                        if (!DBNull.Value.Equals(dataReader.GetValue("last_update")))
                        {
                            chatLoginUserModel.LastUpdate = dataReader.GetDateTime("last_update");
                        }
                        else
                        {
                            chatLoginUserModel.LastUpdate = DateTime.Now;
                        }

                        retVal.Add(chatLoginUserModel);
                        result++;
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                retVal = null;
            }

            return retVal;
        }

        /// <summary>
        /// ユーザーログイン情報にログインをセット
        /// </summary>
        /// <param name="name"></param>
        /// <param name="IpAddress"></param>
        /// <param name="HostName"></param>
        /// <param name="MachineName"></param>
        /// <param name="OsName"></param>
        /// <param name="UserAgent"></param>
        /// <returns></returns>
        public static bool SetUserLogin(
            string name,
            string IpAddress = "",
            string HostName = "",
            string MachineName = "",
            string OsName = "",
            string UserAgent = "")
        {
            bool retVal = false;

            try
            {
                int instVal = -1;
                StringBuilder sqlString = new StringBuilder();
                Common.MySQLConnecter connecter = new Common.MySQLConnecter(
                    "192.168.101.251", "3306", "haku-chat", "haku-chat", "haku-chat-db"
                    );
                List<MySqlParameter> mySqlParameterCollection = new List<MySqlParameter>();
                MySqlParameter mySqlParameter = new MySqlParameter();

                sqlString.Append("replace into ");
                sqlString.Append(" chat_login_user ");
                sqlString.Append("( ");
                sqlString.Append(" name, status, ip_address, host_name, machine_name, os_name, user_agent, login_time, last_update ");
                sqlString.Append(") value ( ");
                sqlString.Append(" @name, 1, @ip_address, @host_name, @machine_name, @os_name, @user_agent, now(), now() ");
                sqlString.Append(");");

                mySqlParameter = new MySqlParameter("name", MySqlDbType.VarChar);
                mySqlParameter.Value = name;
                mySqlParameterCollection.Add(mySqlParameter);

                mySqlParameter = new MySqlParameter("ip_address", MySqlDbType.VarChar);
                mySqlParameter.Value = IpAddress;
                mySqlParameterCollection.Add(mySqlParameter);

                mySqlParameter = new MySqlParameter("host_name", MySqlDbType.VarChar);
                mySqlParameter.Value = HostName;
                mySqlParameterCollection.Add(mySqlParameter);

                mySqlParameter = new MySqlParameter("machine_name", MySqlDbType.VarChar);
                mySqlParameter.Value = MachineName;
                mySqlParameterCollection.Add(mySqlParameter);

                mySqlParameter = new MySqlParameter("os_name", MySqlDbType.VarChar);
                mySqlParameter.Value = OsName;
                mySqlParameterCollection.Add(mySqlParameter);

                mySqlParameter = new MySqlParameter("user_agent", MySqlDbType.VarChar);
                mySqlParameter.Value = UserAgent;
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

            return retVal;
        }

        /// <summary>
        /// ユーザーログイン情報にログアウトをセット
        /// </summary>
        /// <param name="name"></param>
        /// <param name="IpAddress"></param>
        /// <param name="HostName"></param>
        /// <param name="MachineName"></param>
        /// <param name="OsName"></param>
        /// <param name="UserAgent"></param>
        /// <returns></returns>
        public static bool SetUserLogout(
            string name,
            string IpAddress = "",
            string HostName = "",
            string MachineName = "",
            string OsName = "",
            string UserAgent = "")
        {
            bool retVal = false;

            try
            {
                int instVal = -1;
                StringBuilder sqlString = new StringBuilder();
                Common.MySQLConnecter connecter = new Common.MySQLConnecter(
                    "192.168.101.251", "3306", "haku-chat", "haku-chat", "haku-chat-db"
                    );
                List<MySqlParameter> mySqlParameterCollection = new List<MySqlParameter>();
                MySqlParameter mySqlParameter = new MySqlParameter();

                sqlString.Append("replace into ");
                sqlString.Append(" chat_login_user ");
                sqlString.Append("( ");
                sqlString.Append(" name, status, ip_address, host_name, machine_name, os_name, user_agent, logout_time, last_update ");
                sqlString.Append(") value ( ");
                sqlString.Append(" @name, 0, @ip_address, @host_name, @machine_name, @os_name, @user_agent, now(), now() ");
                sqlString.Append(");");

                mySqlParameter = new MySqlParameter("name", MySqlDbType.VarChar);
                mySqlParameter.Value = name;
                mySqlParameterCollection.Add(mySqlParameter);

                mySqlParameter = new MySqlParameter("ip_address", MySqlDbType.VarChar);
                mySqlParameter.Value = IpAddress;
                mySqlParameterCollection.Add(mySqlParameter);

                mySqlParameter = new MySqlParameter("host_name", MySqlDbType.VarChar);
                mySqlParameter.Value = HostName;
                mySqlParameterCollection.Add(mySqlParameter);

                mySqlParameter = new MySqlParameter("machine_name", MySqlDbType.VarChar);
                mySqlParameter.Value = MachineName;
                mySqlParameterCollection.Add(mySqlParameter);

                mySqlParameter = new MySqlParameter("os_name", MySqlDbType.VarChar);
                mySqlParameter.Value = OsName;
                mySqlParameterCollection.Add(mySqlParameter);

                mySqlParameter = new MySqlParameter("user_agent", MySqlDbType.VarChar);
                mySqlParameter.Value = UserAgent;
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

            return retVal;
        }

        /// <summary>
        /// ユーザーログイン情報の更新
        /// </summary>
        /// <param name="name"></param>
        /// <param name="IpAddress"></param>
        /// <param name="HostName"></param>
        /// <param name="MachineName"></param>
        /// <param name="OsName"></param>
        /// <param name="UserAgent"></param>
        /// <returns></returns>
        public static bool UpdateUserLogin(
            string name,
            string IpAddress = "",
            string HostName = "",
            string MachineName = "",
            string OsName = "",
            string UserAgent = "")
        {
            bool retVal = false;

            try
            {
                int instVal = -1;
                StringBuilder sqlString = new StringBuilder();
                Common.MySQLConnecter connecter = new Common.MySQLConnecter(
                    "192.168.101.251", "3306", "haku-chat", "haku-chat", "haku-chat-db"
                    );
                List<MySqlParameter> mySqlParameterCollection = new List<MySqlParameter>();
                MySqlParameter mySqlParameter = new MySqlParameter();

                sqlString.Append("replace into ");
                sqlString.Append(" chat_login_user ");
                sqlString.Append("( ");
                sqlString.Append(" name,ip_address, host_name, machine_name, os_name, user_agent, last_update ");
                sqlString.Append(") value ( ");
                sqlString.Append(" @name, @ip_address, @host_name, @machine_name, @os_name, @user_agent, now() ");
                sqlString.Append(");");

                mySqlParameter = new MySqlParameter("name", MySqlDbType.VarChar);
                mySqlParameter.Value = name;
                mySqlParameterCollection.Add(mySqlParameter);

                mySqlParameter = new MySqlParameter("ip_address", MySqlDbType.VarChar);
                mySqlParameter.Value = IpAddress;
                mySqlParameterCollection.Add(mySqlParameter);

                mySqlParameter = new MySqlParameter("host_name", MySqlDbType.VarChar);
                mySqlParameter.Value = HostName;
                mySqlParameterCollection.Add(mySqlParameter);

                mySqlParameter = new MySqlParameter("machine_name", MySqlDbType.VarChar);
                mySqlParameter.Value = MachineName;
                mySqlParameterCollection.Add(mySqlParameter);

                mySqlParameter = new MySqlParameter("os_name", MySqlDbType.VarChar);
                mySqlParameter.Value = OsName;
                mySqlParameterCollection.Add(mySqlParameter);

                mySqlParameter = new MySqlParameter("user_agent", MySqlDbType.VarChar);
                mySqlParameter.Value = UserAgent;
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

            return retVal;
        }

    }
}
