using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using MVT.Core;
using Newtonsoft.Json.Converters;

namespace WebFileManager.ajax
{
    /// <summary>
    /// Summary description for command
    /// </summary>
    public class command : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            //context.Response.ContentType = "text/plain";
            //if (!AllowCall(context))
            //{
            //    context.Response.Write("Hello World");
            //    return;
            //}
            var cmd = context.Request.QueryString["cmd"];
            switch (cmd)
            {
                case "login":
                    login(context);
                    break;
                case "logout":
                    logout(context);
                    break;
                case "checkLogin":
                    checkLogin(context);
                    break;
                case "getlist":
                    getlist(context);
                    break;
                case "rename":
                    rename(context);
                    break;
                case "delete":

                    break;
                case "move":
                    break;
                case "upload":
                    uploads(context);
                    break;
                case "checkexist":

                    break;
                case "edit":

                    break;
                case "getproperty":

                    break;
                case "setproperty":

                    break;
                case "unzip":

                    break;
                case "zip":

                    break;
                case "viewzip":

                    break;
                case "treeview":
                    Load_Treeview(context);
                    break;
                case "download":
                    DownloadFile(context);
                    break;
            }
        }

        private void getlist(HttpContext context)
        {
            var serializer = new JavaScriptSerializer();
            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = Encoding.UTF8;
            string sContent = "";

            string sRoot = System.Configuration.ConfigurationManager.AppSettings["RootFolder"];
            string sFolder = sRoot;
            if (sFolder.IndexOf(@":\") < 0) sFolder = context.Server.MapPath(sFolder);
            if (!sFolder.EndsWith(@"\")) sFolder += @"\";
            if (context.Request.QueryString["fd"] != null && context.Request.QueryString["fd"].Length > 0)
            {
                string s = context.Request.QueryString["fd"];
                if (s.StartsWith("Root"))
                    sFolder = sRoot + s.Substring(4, s.Length - 4).Replace('/', '\\');
            }
            if (!Directory.Exists(sFolder))
            {
                context.Response.Write("{error:'folder not exist'}");
                return;
            }

            List<FileInfo> oList = new List<FileInfo>();
            var directories = Directory.GetDirectories(sFolder);
            foreach (var directory in directories)//load all folder
            {
                var di = new DirectoryInfo(directory);
                string sPath = di.FullName.Replace(sRoot, "Root").Replace('\\', '/');
                FileInfo info = new FileInfo
                                    {
                                        id = EnCryptString.EnCrypt(sPath),
                                        error = "",
                                        path = sPath,
                                        name = di.Name,
                                        isFile = false,
                                        type = "folder",
                                        length = "",
                                        DateCreate = di.CreationTime,
                                        DateEdit = di.LastWriteTime,
                                        isHidden = di.Attributes == FileAttributes.Hidden,
                                        isReadOnly = di.Attributes == FileAttributes.ReadOnly,
                                        isSystem = di.Attributes == FileAttributes.System,
                                        url = ""
                                    };
                oList.Add(info);
            }

            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(sFolder);
            foreach (System.IO.FileInfo f in dir.GetFiles("*.*"))//load all file
            {
                string sPath = f.FullName.Replace(sRoot, "Root").Replace('\\', '/');
                FileInfo info = new FileInfo
                {
                    id = EnCryptString.EnCrypt(sPath),
                    error = "",
                    path = sPath,
                    name = f.Name,
                    isFile = true,
                    type = Path.GetExtension(f.FullName).Replace(".", string.Empty),
                    length = UntilityFunction.ShowCappacityFile(f.Length),
                    DateCreate = f.CreationTime,
                    DateEdit = f.LastWriteTime,
                    isHidden = f.Attributes == FileAttributes.Hidden,
                    isReadOnly = f.Attributes == FileAttributes.ReadOnly,
                    isSystem = f.Attributes == FileAttributes.System
                };
                oList.Add(info);
            }

            sContent = Newtonsoft.Json.JsonConvert.SerializeObject(oList, new JavaScriptDateTimeConverter());
            context.Response.Write(context.Request["jsoncallback"] + "(" + sContent + ")");
        }

        private void rename(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            FileInfo oItem = new FileInfo();
            try
            {
                string sOld =IdToFile(context.Request["id"],context);
                string sNew = context.Request["newname"];
                bool isFile = (context.Request["isFile"] == "true");
                //string stype = context.Request["type"];
                if(isFile)
                {
                    File.Move(sOld,Path.GetDirectoryName(sOld) + "\\" + sNew + Path.GetExtension(sOld));
                }
                else
                {
                    System.IO.Directory.Move(sOld,sNew);
                }
            }
            catch (Exception ex)
            {
                oItem.error = ex.Message;
                throw;
            }
            finally
            {
                string sContent = Newtonsoft.Json.JsonConvert.SerializeObject(oItem, new JavaScriptDateTimeConverter());
                context.Response.Write(context.Request["jsoncallback"] + "(" + sContent + ")");
            }

            
        }

        private void delete(HttpContext context)
        {
            context.Response.Write("");
        }
        //move
        //upload
        //checkexist
        //edit
        //getproperty
        //setproperty
        //unzip
        //zip
        //viewzip

        private void DownloadFile(HttpContext context)
        {
            string sid = context.Request["id"];
            if (sid != null)
            {
                string[] arr = sid.Split(';');
                if (arr != null && arr.Length > 0)
                {
                    if (arr.Length == 1)
                    {
                        string sFile = IdToFile(arr[0],context);
                        if (!File.Exists(sFile))
                        {
                            context.Response.ContentType = "application/json";
                            context.Response.Write("{error:'file not exist'}");
                            return;
                        }
                        else
                        {
                            System.IO.FileInfo fi = new System.IO.FileInfo(sFile);
                            long sz = fi.Length;
                            context.Response.ClearContent();
                            context.Response.ContentType = MimeType(Path.GetExtension(sFile));
                            context.Response.AddHeader("Content-Disposition", string.Format("attachment; filename = {0}", fi.Name));
                            context.Response.AddHeader("Content-Length", sz.ToString("F0"));
                            context.Response.TransmitFile(sFile);
                            context.Response.End();
                        }

                    }
                    //else
                    //{
                    //    using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
                    //    {
                    //        foreach (var s in arr)
                    //        {
                    //            zip.AddFile(s, "\\");
                    //        }
                    //        zip.Save();
                    //    }
                    //}
                }
            }
            context.Response.Write("");
        }

        public static string MimeType(string Extension)
        {
            string mime = "application/octetstream";
            if (string.IsNullOrEmpty(Extension))
                return mime;
            string ext = Extension.ToLower();
            Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (rk != null && rk.GetValue("Content Type") != null)
                mime = rk.GetValue("Content Type").ToString();
            return mime;
        }

        public static string IdToFile(string id, HttpContext context)
        {
            string sRoot = System.Configuration.ConfigurationManager.AppSettings["RootFolder"];
            string sFolder = EnCryptString.DeCrypt(id);
            if (sRoot.IndexOf(@":\") < 0) sRoot = context.Server.MapPath(sRoot);
            if (!sRoot.EndsWith(@"\")) sRoot += @"\";

            if (sFolder != null && sFolder.Length > 0)
            {
                if (sFolder.StartsWith("Root"))
                    sFolder = (sRoot + sFolder.Substring(5, sFolder.Length - 5)).Replace('/', '\\');
            }
            return sFolder;
        }
        /********************************************************************/
        private bool checkexistFile(string sf)
        {
            return false;
        }
        private bool checkexistFolder(string sf)
        {
            return false;
        }

        private bool createFolder(string sf)
        {
            return false;
        }

        private void uploads(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Expires = -1;
            try
            {
                HttpPostedFile postedFile = context.Request.Files["Filedata"];
                string sRoot = System.Configuration.ConfigurationManager.AppSettings["RootFolder"];
                string sFolder = sRoot;
                if (sFolder.IndexOf(@":\") < 0) sFolder = context.Server.MapPath(sFolder);
                if (!sFolder.EndsWith(@"\")) sFolder += @"\";
                if (context.Request["fd"] != null && context.Request["fd"].Length > 0)
                {
                    string s = context.Request["fd"];
                    if (s.StartsWith("Root"))
                        sFolder = sRoot + s.Substring(4, s.Length - 4).Replace('/', '\\');
                }

                if (!Directory.Exists(sFolder))
                    Directory.CreateDirectory(sFolder);

                string filename = postedFile.FileName;
                postedFile.SaveAs(sFolder + @"\" + filename);
                context.Response.Write(context.Request["fd"] + "/" + filename);
                context.Response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                context.Response.Write("Error: " + ex.Message);
            }
        }
        /********************************************************************/
        private void login(HttpContext context)
        {
            context.Response.Write("");
        }

        private bool logout(HttpContext context)
        {
            return false;
        }

        private bool checkLogin(HttpContext context)
        {
            return false;
        }

        /********************************************************************/
        private void Load_Treeview(HttpContext context)
        {
            string sFolder = System.Configuration.ConfigurationManager.AppSettings["RootFolder"];
            string sTree = "";
            sTree = "<li><span title=\"Root\" class=\"open\">Root</span>";
            ListDirectories(sFolder, ref sTree);
            sTree += "</li>";

            context.Response.ContentType = "text/plain";
            context.Response.Write(sTree);
            context.Response.StatusCode = 200;
        }
        private void ListDirectories(string path, ref string sPath)
        {
            string sFolder = System.Configuration.ConfigurationManager.AppSettings["RootFolder"];
            var directories = Directory.GetDirectories(path);
            if (directories.Any())
            {
                sPath += "<ul>";
                foreach (var directory in directories)
                {
                    var di = new DirectoryInfo(directory);
                    sPath += string.Format("<li><span title=\"{0}\">{1}</span>", di.FullName.Replace(sFolder, "Root").Replace('\\', '/'), di.Name);
                    ListDirectories(directory, ref sPath);
                    sPath += "</li>";
                }
                sPath += "</ul>";
            }
        }

        /********************************************************************/
        private bool AllowCall(HttpContext context)
        {
            string ServerLocal = context.Request.Url.Authority;
            string ServerRefeffer = "";
            if (context.Request.UrlReferrer != null) ServerRefeffer = context.Request.UrlReferrer.Authority;
            return (ServerLocal == ServerRefeffer);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}