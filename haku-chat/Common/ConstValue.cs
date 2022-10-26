using System;

namespace haku_chat.Common
{
    /// <summary>
    /// 投稿者名色関連定数
    /// </summary>
    public static class NameColor
    {
        /// <summary>
        /// カラーID関連
        /// </summary>
        public enum ColorCodeID
        {
            [StringValue("黒")]
            BLACK = 0,
            [StringValue("白")]
            WHITE = 1,
            [StringValue("赤")]
            RED = 2,
            [StringValue("緑")]
            GREEN = 3,
            [StringValue("青")]
            BLUE = 4,
            [StringValue("黄")]
            YELLOW = 5,
            [StringValue("マジェンタ")]
            MAGENTA = 6,
            [StringValue("シアン")]
            CYAN = 7,
        }

        /// <summary>
        /// カラーコード関連
        /// </summary>
        public enum ColorCode
        {
            [StringValue("#000000")]
            BLACK = 0x000000,
            [StringValue("#FFFFFF")]
            WHITE = 0xFFFFFF,
            [StringValue("#FF0000")]
            RED = 0xFF0000,
            [StringValue("#00FF00")]
            GREEN = 0x00FF00,
            [StringValue("#0000FF")]
            BLUE = 0x0000FF,
            [StringValue("#FFFF00")]
            YELLOW = 0xFFFF00,
            [StringValue("#FF00FF")]
            MAGENTA = 0xFF00FF,
            [StringValue("#00FFFF")]
            CYAN = 0x00FFFF,
        }
    }

    /// <summary>
    /// ログイン関連定数
    /// </summary>
    public static class Login
    {
        /// <summary>
        /// ログインセッション定数
        /// </summary>
        public enum LoginSessionStatus
        {
            [StringValue("ログアウト済みセッション")]
            SESSION_UNKOWNERROR = -1,
            [StringValue("ログアウト済みセッション")]
            SESSION_OFFLINE = 0,
            [StringValue("有効なセッション")]
            SESSION_ONLINE = 1,
            [StringValue("セッションタイムアウト")]
            SESSION_TIMEOUT = 2,
            [StringValue("存在しないセッション")]
            SESSION_NOTFOUND = 3,
        }
    }

    /// <summary>
    /// Enumに文字列を付加するためのAttributeクラス
    /// </summary>
    public class StringValueAttribute : Attribute
    {
        /// <summary>
        /// Holds the stringvalue for a value in an enum.
        /// </summary>
        public string StringValue { get; protected set; }

        /// <summary>
        /// Constructor used to init a StringValue Attribute
        /// </summary>
        /// <param name="value"></param>
        public StringValueAttribute(string value)
        {
            this.StringValue = value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class CommonAttribute
    {

        /// <summary>
        /// Will get the string value for a given enums value, this will
        /// only work if you assign the StringValue attribute to
        /// the items in your enum.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetStringValue(this Enum value)
        {
            // Get the type
            Type type = value.GetType();

            // Get fieldinfo for this type
            System.Reflection.FieldInfo fieldInfo = type.GetField(value.ToString());

            //範囲外の値チェック
            if (fieldInfo == null) return null;

            StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

            // Return the first if there was a match.
            return attribs.Length > 0 ? attribs[0].StringValue : null;

        }
    }

}
