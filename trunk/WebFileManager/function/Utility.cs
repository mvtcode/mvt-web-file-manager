using System;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Net.Mail;
using System.Net;
using System.Threading;

namespace MVT.Core
{
    public class Utility
    {
        public Utility()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region "Event Log"

        public static void LogEvent(string message, EventLogEntryType type)
        {
            string fileName = DateTime.Now.ToString("yyyyMMdd") + ".txt";
            string Source = @"D:\WebTamTrinh\Log\" + fileName;

            StreamWriter writer = null;
            message = "[" + type.ToString() + "] " + DateTime.Now.ToString() + ":" + message;

            try
            {
                writer = new StreamWriter(Source, true);
                writer.WriteLine(message);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (writer != null)
                {
                    writer.Dispose();
                    writer.Close();
                }
            }
        }

        public static void LogEvent(Exception ex)
        {
            string message = "EXCEPTION: " + ex.GetType().Name;
            message += ex.Message + Environment.NewLine;
            message += ex.StackTrace;

            LogEvent(message, EventLogEntryType.Error);
        }

        public static void LogEventADODB(Exception ex, EventLogEntryType type)
        {
            string message = "EXCEPTION: " + ex.GetType().Name;
            message += ex.Message + Environment.NewLine;
            message += ex.InnerException.Message + Environment.NewLine;
            message += ex.StackTrace;

            LogEvent(message, type);
        }

        public static void LogEventADODB(Exception ex)
        {
            string message = "EXCEPTION: " + ex.GetType().Name;
            message += ex.Message + Environment.NewLine;
            message += ex.InnerException.Message + Environment.NewLine;
            message += ex.StackTrace;

            LogEvent(message, EventLogEntryType.Error);
        }

        #endregion

        public static string UrlRoot
        {
            get
            {
                string sRet = System.Web.HttpContext.Current.Request.ApplicationPath;
                if (!sRet.EndsWith("/"))
                    sRet = sRet + "/";
                return sRet;
            }
        }

        #region "Upload File To Server"
        public static string UploadFile(System.Web.UI.HtmlControls.HtmlInputFile clientFile, string folderToUp, bool autoGenerateName, bool overwrite, string limitExtension)
        {
            if ((!(clientFile == null)) && (!(clientFile.PostedFile == null)) && !string.IsNullOrEmpty(clientFile.PostedFile.FileName))
            {
                try
                {
                    var postedFile = clientFile.PostedFile;
                    var sFolder = folderToUp;
                    if (postedFile != null)
                    {
                        //Check exist folder
                        try
                        {
                            if (Directory.Exists(sFolder) == false)
                            {
                                Directory.CreateDirectory(sFolder);
                            }
                        }
                        catch
                        {
                            throw new Exception("Thư mục upload chưa được chỉ định quyền ghi dữ liệu");
                        }

                        //Check validate file extension
                        var fileExtension = Path.GetExtension(postedFile.FileName.ToLower());
                        limitExtension = limitExtension.ToLower();
                        if (limitExtension.IndexOf("*.*") == -1 && limitExtension.IndexOf(fileExtension) == -1)
                        {
                            throw new Exception("Không cho upload định dạng file này");
                        }

                        //Generate file name and check overwrite
                        var fileName = Path.GetFileName(postedFile.FileName);
                        //var sFileName = Path.GetFileNameWithoutExtension(postedFile.FileName);
                        var vFileName = fileName;

                        if (autoGenerateName)
                        {
                            fileName = fileName.Substring(fileName.LastIndexOf("."));
                            vFileName = fileName.Insert(fileName.LastIndexOf("."), DateTime.Now.ToString("yyyyMMdd_hhmmss"));
                        }

                        vFileName = vFileName.Replace(" ", string.Empty);

                        if (UploadFile(postedFile.InputStream, folderToUp, vFileName, false))
                            return vFileName;
                        throw new Exception("Upload file không thành công!");
                    }
                    return string.Empty;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return string.Empty;
        }

        public static string UploadFile(System.Web.UI.HtmlControls.HtmlInputFile clientFile, string strFileName, string folderToUp, bool overwrite, string limitExtension)
        {
            if ((!(clientFile == null)) && (!(clientFile.PostedFile == null)) && !string.IsNullOrEmpty(clientFile.PostedFile.FileName))
            {
                try
                {
                    var postedFile = clientFile.PostedFile;
                    //string ContentTypeFile = postedFile.ContentType;

                    if (postedFile != null)
                    {
                        var sFolder = folderToUp;
                        try
                        {
                            if (Directory.Exists(sFolder) == false)
                            {
                                Directory.CreateDirectory(sFolder);
                            }
                        }
                        catch
                        {
                            return string.Empty;
                        }


                        //Check validate file extension
                        var fileExtension = Path.GetExtension(postedFile.FileName.ToLower());
                        limitExtension = limitExtension.ToLower();
                        if (limitExtension.IndexOf("*.*") == -1 && limitExtension.IndexOf(fileExtension) == -1) return "";

                        var vFileName = strFileName;

                        UploadFile(postedFile.InputStream, folderToUp, vFileName, overwrite);
                        return vFileName;
                    }
                    return string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        private static bool UploadFile(Stream inputStream, string uploadPath, string fileName, bool overwrite)
        {
            if (overwrite)
            {
                if (File.Exists(Path.Combine(uploadPath, fileName)))
                {
                    File.Delete(Path.Combine(uploadPath, fileName));
                }
            }

            var result = true;
            var filePath = Path.Combine(uploadPath, fileName);
            var dir = Path.GetDirectoryName(filePath);

            try
            {
                using (var outputStream = new FileStream(filePath, FileMode.Create))
                {
                    StreamCopyToStream(inputStream, outputStream);

                    inputStream.Flush();
                    outputStream.Flush();
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        private static void StreamCopyToStream(Stream input, Stream output)
        {
            const int bufferSize = 2048;
            var buffer = new byte[bufferSize];
            int bytes;
            input.Position = 0;
            while ((bytes = input.Read(buffer, 0, bufferSize)) > 0)
            {
                output.Write(buffer, 0, bytes);
            }
        }
        #endregion

        #region "Read file Excel"
        public static DataTable ExcelToDataTable(string filelocation)
        {
            try
            {
                OleDbCommand excelCommand = new OleDbCommand();

                OleDbDataAdapter excelDataAdapter = new OleDbDataAdapter();

                string excelConnStr = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + filelocation + "; Extended Properties =Excel 8.0;";

                OleDbConnection excelConn = new OleDbConnection(excelConnStr);

                excelConn.Open();

                DataTable dtSheetName = excelConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                if (dtSheetName.Rows.Count > 0)
                {
                    DataTable dtPatterns = new DataTable();

                    //excelCommand = new OleDbCommand("Select * from [" + dtSheetName.Rows[0]["TABLE_NAME"].ToString() + "] order by trdate,trref", excelConn);
                    excelCommand = new OleDbCommand("Select * from [" + dtSheetName.Rows[0]["TABLE_NAME"].ToString() + "]", excelConn);

                    excelDataAdapter.SelectCommand = excelCommand;

                    excelDataAdapter.Fill(dtPatterns);

                    excelCommand.Dispose();
                    excelConn.Close();
                    excelConn.Dispose();

                    return dtPatterns;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Hiện tại hệ thống server đang bận, bạn hãy quay lại vào lúc khác.");
            }
            finally
            {
                if (File.Exists(filelocation))
                    File.Delete(filelocation);
            }
        }
        #endregion


        #region Datetime
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ddMMyyyy"></param>
        /// <returns></returns>
        public static DateTime fGetDateTimeBy(string ddMMyyyy)
        {
            try
            {
                string[] sArr = ddMMyyyy.Split('/');
                if (sArr.Length == 3)
                {
                    int dd = 0;
                    int MM = 0;
                    int yyyy = 0;
                    if (int.TryParse(sArr[0], out dd) && dd > 0 && dd <= 31)
                    {
                        if (int.TryParse(sArr[1], out MM) && MM > 0 && MM <= 12)
                            if (int.TryParse(sArr[2], out yyyy) && yyyy > 1987 && yyyy <= 9999)
                                return (new DateTime(yyyy, MM, dd));
                    }
                }
                return DateTime.MinValue;
            }
            catch (Exception ex)
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Kiểm tra xem dữ liệu vào đã đúng định dạng dd/MM/yyyy hay chưa?
        /// Nếu là dạng d/M/yyyy thì đưa về dạng dd/MM/yyyy
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string fFormatToDDMMYYYY(string input)
        {
            try
            {
                string result = string.Empty;
                input = input.Trim();
                if (input.Length == 0)
                {
                    return string.Empty;
                }
                if (input.Length < 8 || input.Length > 10)
                {
                    throw new Exception("Ngày tháng phải định dạng theo dd/MM/yyyy.");
                }
                string[] sArr = input.Split('/');
                if (sArr.Length != 3)
                {
                    throw new Exception("Ngày tháng phải định dạng theo dd/MM/yyyy.");
                }
                int dd = int.Parse(sArr[0]);
                int MM = int.Parse(sArr[1]);
                int yyyy = int.Parse(sArr[2]);
                if (MM == 1 || MM == 3 || MM == 5 || MM == 7 || MM == 8 || MM == 10 || MM == 12)
                {
                    if (dd <= 0 || dd > 31)
                    {
                        throw new Exception("Ngày không hợp lệ.");
                    }

                }
                else if (MM == 4 || MM == 6 || MM == 9 || MM == 11)
                {
                    if (dd <= 0 || dd > 30)
                    {
                        throw new Exception("Tháng " + MM + " chỉ có 30 ngày.");
                    }
                }
                else if (MM == 2)
                {
                    if (yyyy % 4 == 0)
                    {
                        if (dd <= 0 || dd > 29)
                        {
                            throw new Exception("Năm nhuận tháng 2 chỉ có 29 ngày.");
                        }
                    }
                    else
                    {
                        if (dd <= 0 || dd > 28)
                        {
                            throw new Exception("Tháng 2 chỉ có 28 ngày.");
                        }
                    }
                }
                else
                {
                    throw new Exception("Tháng không hợp lệ.");
                }

                if (dd < 10)
                    result += "0" + dd;
                else
                    result += dd.ToString();

                result += "/";
                if (MM <= 0 || MM > 12)
                {
                    throw new Exception("Tháng không hợp lệ.");
                }
                else if (MM < 10)
                    result += "0" + MM;
                else
                    result += MM.ToString();

                result += "/";
                if (yyyy < 1900 || yyyy > 9999)
                {
                    throw new Exception("Năm không hợp lệ.");
                }
                else
                    result += yyyy;

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion Datetime

        #region Excel

        //public static DataTable ExcelToDataTable(string filelocation, string query)
        //{
        //    DataTable dtPatterns = null;
        //    try
        //    {
        //        OleDbCommand excelCommand = new OleDbCommand();

        //        OleDbDataAdapter excelDataAdapter = new OleDbDataAdapter();

        //        string excelConnStr = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + filelocation + "; Extended Properties =Excel 8.0;";

        //        OleDbConnection excelConn = new OleDbConnection(excelConnStr);

        //        excelConn.Open();

        //        dtPatterns = new DataTable();

        //        excelCommand = new OleDbCommand(query, excelConn);

        //        excelDataAdapter.SelectCommand = excelCommand;

        //        excelDataAdapter.Fill(dtPatterns);

        //        excelCommand.Dispose();
        //        excelConn.Close();
        //        excelConn.Dispose();
        //    }
        //    catch (Exception ex)
        //    { }
        //    return dtPatterns;
        //}

        public static bool InsertGiaoDichDen(string filelocation, DataTable dataInsert, string tableInsert)
        {
            OleDbCommand excelCommand = null;
            bool result = false;
            string excelConnStr = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + filelocation + @"; Extended Properties=""Excel 8.0;HDR=YES;""";

            OleDbConnection excelConn = new OleDbConnection(excelConnStr);

            excelConn.Open();

            try
            {
                for (int i = 0; i < dataInsert.Rows.Count; i++)
                {
                    excelCommand = new OleDbCommand();
                    StringBuilder sb = new StringBuilder();

                    sb.Append("INSERT INTO [" + tableInsert + "$] ([ID],[GiayNop],[MaDaiLy],[TenDaiLy],[NguoiNop],[SoTienHang],[SoTienCuoc],[ThoiGian],[HinhThuc])");
                    sb.Append(" Values(");
                    sb.Append("'" + (i + 1).ToString() + "',");
                    sb.Append("'" + dataInsert.Rows[i]["Code"].ToString() + "',");
                    sb.Append("'" + dataInsert.Rows[i]["CodeFactory"].ToString() + "',");
                    sb.Append("'" + dataInsert.Rows[i]["NameFactory"].ToString() + "',");
                    sb.Append("'" + dataInsert.Rows[i]["NguoiNop"].ToString() + "',");
                    sb.Append("'" + dataInsert.Rows[i]["PhatSinh3"].ToString() + "',");
                    sb.Append("'0',");
                    sb.Append("'" + dataInsert.Rows[i]["TransactionDate"].ToString() + "',");
                    sb.Append("'" + dataInsert.Rows[i]["TransactionCat"].ToString() + "'");
                    sb.Append(")");
                    excelCommand.CommandText = sb.ToString();
                    excelCommand.CommandType = System.Data.CommandType.Text;
                    excelCommand.Connection = excelConn;
                    excelCommand.ExecuteNonQuery();
                }
                result = true;

            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                if (excelCommand != null)
                    excelCommand.Dispose();
                if (excelConn.State == System.Data.ConnectionState.Open)
                {
                    excelConn.Dispose();
                    excelConn.Close();
                }
            }

            return result;
        }

        public static bool InsertGiaoDichDi(string filelocation, DataTable dataInsert, string tableInsert)
        {
            OleDbCommand excelCommand = null;
            bool result = false;
            string excelConnStr = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + filelocation + @"; Extended Properties=""Excel 8.0;HDR=YES;""";

            OleDbConnection excelConn = new OleDbConnection(excelConnStr);

            excelConn.Open();

            try
            {
                for (int i = 0; i < dataInsert.Rows.Count; i++)
                {
                    excelCommand = new OleDbCommand();
                    StringBuilder sb = new StringBuilder();

                    sb.Append("INSERT INTO [" + tableInsert + "$] ([ID],[NgayThucHien],[SoPhieuChi],[SoTien],[NoiDung],[TaiKhoanThuHuong],[DonViThuHuong])");
                    sb.Append(" Values(");
                    sb.Append("'" + (i + 1).ToString() + "',");
                    sb.Append("'" + dataInsert.Rows[i]["TransactionDate"].ToString() + "',");
                    sb.Append("'" + dataInsert.Rows[i]["Code"].ToString() + "',");
                    sb.Append("'" + dataInsert.Rows[i]["PhatSinh3"].ToString() + "',");
                    sb.Append("'" + dataInsert.Rows[i]["NoiDung"].ToString() + "',");
                    sb.Append("'" + dataInsert.Rows[i]["AccountBank"].ToString() + "',");
                    sb.Append("'" + dataInsert.Rows[i]["NameFactory"].ToString() + "'");
                    sb.Append(")");
                    excelCommand.CommandText = sb.ToString();
                    excelCommand.CommandType = System.Data.CommandType.Text;
                    excelCommand.Connection = excelConn;
                    excelCommand.ExecuteNonQuery();
                }
                result = true;

            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                if (excelCommand != null)
                    excelCommand.Dispose();
                if (excelConn.State == System.Data.ConnectionState.Open)
                {
                    excelConn.Dispose();
                    excelConn.Close();
                }
            }

            return result;
        }


        #endregion


        #region Functions Utility for Numberic
        public static bool IsInteger(object obj)
        {
            try
            {
                Convert.ToInt32(obj);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool IsIntegerNull(object obj)
        {
            if (obj == null) return true;
            if (IsInteger(obj) && obj.ToString().Trim() == "") return true;
            if (IsInteger(obj) && obj.ToString().Trim() == "0") return true;
            return false;
        }
        public static int InitializeInteger
        {
            get
            {
                return 0;
            }
        }


        public static bool IsDouble(object obj)
        {
            try
            {
                Convert.ToDouble(obj);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static int InitializeDouble
        {
            get
            {
                return 0;
            }
        }
        #endregion

        #region Functions Utility for DateTime
        public static bool IsDateTime(object obj)
        {
            try
            {
                Convert.ToDateTime(obj);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool IsDateTimeNull(object obj)
        {
            if (obj == null) return true;
            if (IsDateTime(obj) && obj.ToString().Trim() == "") return true;
            if (IsDateTime(obj) && Convert.ToDateTime(obj) == DateTime.MinValue) return true;
            if (IsDateTime(obj) && obj.ToString().IndexOf("1/1/1900") > -1) return true;
            if (IsDateTime(obj) && obj.ToString().IndexOf("01/01/1900") > -1) return true;
            return false;
        }
        public static DateTime InitializeDateTime
        {
            get
            {
                return Convert.ToDateTime("01/01/1900");
            }
        }
        #endregion

        #region Functions Utility for String
        public static bool IsStringNullOrEmpty(object obj)
        {
            if (obj == null) return true;
            if (obj.ToString().Trim() == string.Empty) return true;
            if (obj.ToString().Trim() == "") return true;
            return false;
        }
        public static bool IsMultilineNullOrEmpty(object obj)
        {
            if (obj == null) return true;
            if (obj.ToString() == string.Empty) return true;
            if (obj.ToString() == "") return true;
            return false;
        }
        public static string InitializeString
        {
            get
            {
                return string.Empty;
            }
        }


        #endregion

        #region Functions Utility for Boolean
        public static bool InitializeBool
        {
            get
            {
                return false;
            }
        }
        #endregion
        #region Send Mail

        public static void SendEmail(string to, string subject, string body, string pathAttack, string from, string pass)
        {
            new Thread(() =>
            {
                try
                {
                    MailMessage msg = new MailMessage
                    {
                        From = new MailAddress(from, "LIC Logistics System", Encoding.UTF8)
                    };
                    msg.To.Add(to);
                    msg.Subject = subject;
                    msg.IsBodyHtml = true;
                    msg.Body = body;
                    msg.Priority = MailPriority.High;
                    if (!string.IsNullOrEmpty(pathAttack))
                    {
                        Attachment att = new Attachment(pathAttack);
                        msg.Attachments.Add(att);
                    }
                    msg.SubjectEncoding = Encoding.UTF8;
                    msg.BodyEncoding = Encoding.UTF8;


                    SmtpClient client = new SmtpClient("mail.lic.vn");
                    client.Port = 25;
                    //client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(from, pass);
                    client.EnableSsl = false;
                    client.Send(msg);

                    //new SmtpClient { Credentials = new NetworkCredential("longlx@lic.vn", "chj1mjnhem"), Host = "mail.lic.vn", Port = 25, EnableSsl = true }.Send(msg);
                }
                catch (Exception ex)
                {
                    LogEvent(ex);
                }
            }).Start();
        }
        #endregion

    }
}
