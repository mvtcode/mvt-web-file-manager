using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MVT.Core
{
    public class UntilityFunction
    {
        public static string EncodePassword(string sPassword)
        {
            //Declarations
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(sPassword);
            encodedBytes = md5.ComputeHash(originalBytes);

            //Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes);
        }

        private static string ReplaceHTML(object sStr)
        {
            if (ReferenceEquals(sStr, DBNull.Value))
            {
                return "";
            }
            if (sStr == null)
            {
                return "";
            }
            string s = Convert.ToString(sStr);
            s = s.Replace("<", "&lt;");
            s = s.Replace(">", "&gt;");
            return s;
        }

        public static string StringForUpdateAllowingNULL(object x)
        {
            try
            {
                if (ReferenceEquals(x, DBNull.Value))
                {
                    return "NULL";
                }
                if (x == null)
                {
                    return "NULL";
                }
                if (x.ToString().Length == 0)
                {
                    return "NULL";
                }
                x = ReplaceHTML(x);
                return "'" + x + "'";
            }
            catch
            {
                return "NULL";
            }
        }

        public static string StringUpdateUnicode(object x)
        {
            if (x is DBNull)
            {
                return "NULL";
            }
            if ((x == null))
            {
                return "NULL";
            }
            if (x.ToString().Length == 0)
            {
                return "NULL";
            }
            string strTmp = x.ToString();
            strTmp = strTmp.Replace("'", "''");
            strTmp = ReplaceHTML(strTmp);
            return "N" + "'" + strTmp + "'";
        }

        public static string DateForUpdate(object x)
        {
            try
            {
                string s = DateForDisplay(x);
                if (isDate(s))
                {
                    DateTime xDate = DateTime.Parse(s);
                    if (StripBackSlash(s).Trim().Length == 0)
                    {
                        return "NULL";
                    }
                    if (xDate.Year > 1600)
                    {
                    }
                    else
                    {
                        return "NULL";
                    }
                    return "'" + string.Format("{0:MM/dd/yyyy}", xDate) + "'";
                }
                return "NULL";
            }
            catch (Exception)
            {
                return "NULL";
            }
        }

        public static string StripBackSlash(object x)
        {
            string s = StringForNull(x);
            while (s.IndexOf('/') > 0)
            {
                int iloc = s.IndexOf('/');
                s = s.Substring(0, iloc - 1) + s.Substring(iloc + 1);
            }
            s = s.Trim();
            if (s.Length == 0)
            {
                return "";
            }
            return s;
        }

        public static string DateForDisplay(object sDate)
        {
            if (string.IsNullOrEmpty(StringForNull(sDate)))
            {
                return "";
            }
            sDate = string.Format("{0:dd/MM/yyyy}", sDate);
            string[] sCldInput = sDate.ToString().Split('/');
            var oDate = new DateTime(Convert.ToInt16(sCldInput[2]), Convert.ToInt16(sCldInput[1]), Convert.ToInt16(sCldInput[0]));
            return oDate.ToString();
        }

        public static string StringForNull(object x)
        {
            if (x is DBNull)
            {
                return "";
            }
            if (x == null)
            {
                return "";
            }
            return x.ToString().Trim();
        }

        public static Boolean isDate(string sDate)
        {
            bool b = true;
            try
            {
                DateTime.Parse(sDate);
            }
            catch (Exception)
            {
                b = false;
            }
            return b;
        }

        public static int IntegerForNull(object x)
        {
            if ((x == null)) return 0;
            if (x is DBNull) return 0;
            if (!IsNumeric(x)) return 0;
            if(!checkFormatInt(x.ToString())) return 0;
            return Convert.ToInt32(x);
        }

        public static double DoubleForNull(object x)
        {
            if ((x == null))
            {
                return 0;
            }
            else if (ReferenceEquals(x, DBNull.Value))
            {
                return 0;
            }
            else if (!IsNumeric(x))
            {
                return 0;
            }
            else
            {
                return Convert.ToDouble(x);
            }
        }

        public static Boolean IsNumeric(Object Expression)
        {
            if (Expression == null || Expression is DateTime)
                return false;

            if (Expression is Int16 || Expression is Int32 || Expression is Int64 || Expression is Decimal || Expression is Single || Expression is Double || Expression is Boolean)
                return true;

            try
            {
                if (Expression is string)
                    Double.Parse(Expression as string);
                else
                    Double.Parse(Expression.ToString());
                return true;
            }
            catch { }
            return false;
        }

        public static string GetCharFormatNum()
        {
            string s = string.Format("{0:N0}", 1000);//return 1.000 or 1,000
            return s.Substring(1, 1);
        }

        public static double StringToDouble(string s)
        {
            s = s.Replace(GetCharFormatNum(), "");
            if ((s == ""))
            {
                return 0;
            }

            else if (!IsNumeric(s))
            {
                return 0;
            }
            else
            {
                return Convert.ToDouble(s);
            }
        }

        public static int StringToInt(string s)
        {
            s = s.Replace(GetCharFormatNum(), "");
            if ((s == ""))
            {
                return 0;
            }

            else if (!IsNumeric(s))
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(s);
            }

        }

        public static Boolean checkFormatInt(object x)
        {
            if (x == null) return true;
            if (x is DBNull) return true;
            string s = x.ToString();
            Regex isNumber = new Regex(@"^\d+$");
            Match m = isNumber.Match(s);
            return m.Success;
        }

        ///////////////////////////

        //public static string GetPathImg(string s)
        //{
        //    if (s == "")
        //    {
        //        return "/Images/NoImage.jpg";
        //    }
        //    else
        //    {
        //        if (s.StartsWith("http://") || s.StartsWith("https://"))
        //        {
        //            return s;
        //        }
        //        else
        //        {
        //            return Config.GetPathImage + s;
        //        }
        //    }
        //}

        //public static string GetPathImgThumb(string s)
        //{
        //    if (s == "")
        //    {
        //        return "/Images/NoImage.jpg";
        //    }
        //    else
        //    {
        //        if (s.StartsWith("http://") || s.StartsWith("https://"))
        //        {
        //            return s;
        //        }
        //        else
        //        {
        //            return Config.GetPathImageThumb + s;
        //        }
        //    }
        //}

        //public static string GetPathImgProduct(string s)
        //{
        //    if (s == "")
        //    {
        //        return "/Images/NoImage.jpg";
        //    }
        //    else
        //    {
        //        if (s.StartsWith("http://") || s.StartsWith("https://"))
        //        {
        //            return s;
        //        }
        //        else
        //        {
        //            return Config.PathProductShow + "/" + s;
        //        }
        //    }
        //}

        public static string ShowCappacityFile(long iByte)
        {
            double d = iByte;
            if(d<1024) return string.Format("{0:N0} Byte",d);
            d = d/1024;
            if (d < 1024) return string.Format("{0:N3} KB", d);
            d = d / 1024;
            if (d < 1024) return string.Format("{0:N3} MB", d);
            d = d / 1024;
            if (d < 1024) return string.Format("{0:N3} GB", d);
            d = d / 1024;
            return string.Format("{0:N3} TB", d);
        }
    }
}
