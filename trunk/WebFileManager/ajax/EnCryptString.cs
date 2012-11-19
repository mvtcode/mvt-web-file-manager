using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace WebFileManager.ajax
{
    public class EnCryptString
    {
        public static string EnCrypt(string strEnCrypt)
        {
            byte[] EnCryptArr = UTF8Encoding.UTF8.GetBytes(strEnCrypt);
            return Convert.ToBase64String(EnCryptArr).Replace("=","*");
        }

        public static string DeCrypt(string strDecypt)
        {
            byte[] DeCryptArr = Convert.FromBase64String(strDecypt.Replace("*","="));
            return UTF8Encoding.UTF8.GetString(DeCryptArr);
        }
    }
}