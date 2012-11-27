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
using Ionic.Zip;

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
                    delete(context);
                    break;
                case "move":
                    move(context);
                    break;
                case "upload":
                    uploads(context);
                    break;
                case "CheckExistFile":
                    CheckExistFile(context);
                    break;
                case "editText":
                    editText(context);
                    break;
                case "SaveText":
                    SaveText(context);
                    break;
                case "setproperty":

                    break;
                case "ExtractHere":
                    ExtractHere(context);
                    break;
                case "ZipFolder":
                    ZipFolder(context);
                    break;
                case "ViewZip":
                    ViewZip(context);
                    break;
                case "treeview":
                    Load_Treeview(context);
                    break;
                case "download":
                    DownloadFile(context);
                    break;
                case "newFolder":
                    createFolder(context);
                    break;
            }
        }

        private void CheckExistFile(HttpContext context)
        {
            
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
                string sOld = IdToFile(context.Request["id"], context);
                string sNew = context.Request["newname"];
                bool isFile = (context.Request["isFile"] == "true");

                if (!checkNameNotUse(sNew, isFile))
                {
                    oItem.error = isFile ? "canot use file name" : "canot use folder name";
                    return;
                }

                //string stype = context.Request["type"];
                if (isFile)
                {
                    sNew = Path.GetDirectoryName(sOld) + "\\" + sNew + Path.GetExtension(sOld);
                    if (!File.Exists(sOld))
                    {
                        oItem.error = "file not exist";
                    }
                    else
                    {
                        if (sOld == sNew) return;
                        if (File.Exists(sNew))
                        {
                            oItem.error = "canot rename file because exist";
                        }
                        else
                        {
                            File.Move(sOld, sNew);
                        }
                    }
                }
                else
                {
                    sNew = Path.GetDirectoryName(sOld) + "\\" + sNew;
                    if (!Directory.Exists(sOld))
                    {
                        oItem.error = "folder not exist";
                    }
                    else
                    {
                        if (sOld == sNew) return;
                        if (Directory.Exists(sNew))
                        {
                            oItem.error = "canot rename folder because exist";
                        }
                        else
                        {
                            System.IO.Directory.Move(sOld, sNew);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oItem.error = ex.Message;
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                string sContent = Newtonsoft.Json.JsonConvert.SerializeObject(oItem, new JavaScriptDateTimeConverter());
                context.Response.Clear();
                context.Response.Write(context.Request["jsoncallback"] + "(" + sContent + ")");
                context.Response.End();
            }
        }

        private void move(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            FileInfo oItem = new FileInfo();
            try
            {
                string sName = IdToFile(context.Request["id"], context);
                bool isFile = (context.Request["isFile"] == "true");
                string sCurrentFolder = getFullPath(context.Request.QueryString["fd"], context);
                string sNewFolder = getFullPath(context.Request.QueryString["nfd"], context);

                if (sCurrentFolder == sNewFolder) return;

                if (isFile)
                {
                    if (!File.Exists(sName))
                    {
                        oItem.error = "file not exist";
                    }
                    else
                    {
                        string newTarget = sNewFolder + "\\" + Path.GetFileName(sName);
                        if (File.Exists(newTarget))
                        {
                            oItem.error = "file target already exist";
                        }
                        else
                        {
                            File.Move(sName, newTarget);
                        }
                    }
                }
                else
                {
                    if (!Directory.Exists(sName))
                    {
                        oItem.error = "folder not exist";
                    }
                    else
                    {
                        sNewFolder = sNewFolder + "\\" + Path.GetFileNameWithoutExtension(sName);
                        if (Directory.Exists(sNewFolder))
                        {
                            oItem.error = "folder target already exist";
                        }
                        else
                        {
                            if (sNewFolder.StartsWith(sName))
                            {
                                oItem.error = "canot move to sub this folder";
                            }
                            else
                            {
                                System.IO.Directory.Move(sName, sNewFolder);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oItem.error = ex.Message;
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                string sContent = Newtonsoft.Json.JsonConvert.SerializeObject(oItem, new JavaScriptDateTimeConverter());
                context.Response.Clear();
                context.Response.Write(context.Request["jsoncallback"] + "(" + sContent + ")");
                context.Response.End();
            }
        }

        private void delete(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            FileInfo oItem = new FileInfo();
            try
            {
                string sName = IdToFile(context.Request["id"], context);
                bool isFile = (context.Request["isFile"] == "true");
                if (isFile)
                {
                    if (!File.Exists(sName))
                    {
                        oItem.error = "file not exist";
                    }
                    else
                    {
                        File.Delete(sName);
                    }
                }
                else
                {
                    if (!Directory.Exists(sName))
                    {
                        oItem.error = "folder not exist";
                    }
                    else
                    {
                        System.IO.Directory.Delete(sName, true);
                    }
                }
            }
            catch (Exception ex)
            {
                oItem.error = ex.Message;
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                string sContent = Newtonsoft.Json.JsonConvert.SerializeObject(oItem, new JavaScriptDateTimeConverter());
                context.Response.Clear();
                context.Response.Write(context.Request["jsoncallback"] + "(" + sContent + ")");
                context.Response.End();
            }
        }

        //edit
        private void editText(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string sContent = "";
            try
            {
                string sPath = IdToFile(context.Request["id"], context);
                StreamReader stream = new StreamReader(sPath);
                sContent = stream.ReadToEnd();
                if (sContent == null) sContent = " ";
                stream.Close();
                stream.Dispose();
                context.Response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 404;
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                context.Response.Write(sContent);
            }
        }

        private void SaveText(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            FileInfo oItem = new FileInfo();
            try
            {
                string sFile = IdToFile(context.Request["id"], context);
                string sContent = context.Request["Content"];
                if(sContent!=null) sContent = sContent.Replace("\\n","\r\n");
                if (!File.Exists(sFile))
                {
                    oItem.error = "file not exist";
                }
                else
                {
                    //char[] b = sContent.ToCharArray();
                    StreamWriter stream = new StreamWriter(sFile, false);
                    stream.Write(sContent);//stream.Write(b);
                    stream.Close();
                    stream.Dispose();
                }
            }
            catch (Exception ex)
            {
                oItem.error = ex.Message;
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                string sContent = Newtonsoft.Json.JsonConvert.SerializeObject(oItem, new JavaScriptDateTimeConverter());
                context.Response.Clear();
                context.Response.Write(context.Request["jsoncallback"] + "(" + sContent + ")");
                context.Response.End();
            }
        }
        //setproperty

        private void ExtractHere(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            FileInfo oItem = new FileInfo();
            try
            {
                string sFile = IdToFile(context.Request["id"], context);
                bool bOver = (context.Request["Overwrite"] == "true");
                if (!File.Exists(sFile))
                {
                    oItem.error = "file not exist";
                    return;
                }

                if (!sFile.ToLower().EndsWith(".zip"))
                {
                    oItem.error = "file not type .zip";
                    return;
                }

                using (ZipFile zip = ZipFile.Read(sFile))
                {
                    foreach (ZipEntry zipEntry in zip)
                    {
                        if (bOver)
                            zipEntry.Extract(Path.GetDirectoryName(sFile), ExtractExistingFileAction.OverwriteSilently);  // overwrite == true 
                        else
                            zipEntry.Extract(Path.GetDirectoryName(sFile), ExtractExistingFileAction.DoNotOverwrite);  // overwrite == false 
                    }
                }
            }
            catch (Exception ex)
            {
                oItem.error = ex.Message;
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                string sContent = Newtonsoft.Json.JsonConvert.SerializeObject(oItem, new JavaScriptDateTimeConverter());
                context.Response.Clear();
                context.Response.Write(context.Request["jsoncallback"] + "(" + sContent + ")");
                context.Response.End();
            }
        }

        private void ViewZip(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            FileInfo oItem = new FileInfo();
            string sName = IdToFile(context.Request["id"], context);
            bool isFile = (context.Request["isFile"] == "true");
            if (isFile)
            {
                if (!File.Exists(sName))
                {
                    oItem.error = "file not exist";
                }
                else
                    if (!sName.ToLower().EndsWith(".zip"))
                    {
                        oItem.error = "file not type .zip";
                    }
                    else
                    {
                        List<FileInfo> oList = new List<FileInfo>();
                        try
                        {
                            using (ZipFile zip = ZipFile.Read(sName))
                            {
                                foreach (ZipEntry zipEntry in zip)
                                {
                                    //e.Extract(TargetDirectory, true);  // overwrite == true  
                                    //listBox1.Items.Add(zipEntry.FileName);
                                    oItem = new FileInfo
                                                {
                                                    name = zipEntry.FileName,
                                                    url = zipEntry.Info,
                                                    DateEdit = zipEntry.LastModified,
                                                    DateCreate = zipEntry.CreationTime,
                                                    isFile = zipEntry.IsDirectory == false
                                                };
                                    oList.Add(oItem);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            oItem.error = ex.Message;
                            throw new ApplicationException(ex.Message);
                        }
                        finally
                        {
                            string sContent = Newtonsoft.Json.JsonConvert.SerializeObject(oList, new JavaScriptDateTimeConverter());
                            context.Response.Clear();
                            context.Response.Write(context.Request["jsoncallback"] + "(" + sContent + ")");
                            context.Response.End();
                        }
                        return;
                    }
            }

            string sContentx = Newtonsoft.Json.JsonConvert.SerializeObject(oItem, new JavaScriptDateTimeConverter());
            context.Response.Clear();
            context.Response.Write(context.Request["jsoncallback"] + "(" + sContentx + ")");
            context.Response.End();
        }

        private void ZipFolder(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            FileInfo oItem = new FileInfo();
            try
            {
                string sPath = IdToFile(context.Request["id"], context);
                string sName = context.Request["newname"];
                bool isFile = (context.Request["isFile"] == "true");

                if (isFile)
                {
                    oItem.error = "you must select folder";
                    return;
                }

                if (!checkNameNotUse(sName, false))
                {
                    oItem.error = "canot use file name" + sName;
                    return;
                }

                string sFile = Path.GetDirectoryName(sPath) + "\\" + sName + ".zip";
                if (!Directory.Exists(sPath))
                {
                    oItem.error = "folder not exist";
                }
                else
                {
                    if (File.Exists(sFile))
                    {
                        oItem.error = sName + ".zip exist";
                    }
                    else
                    {
                        using (ZipFile zip = new ZipFile())
                        {
                            zip.AddDirectory(sPath, Path.GetFileNameWithoutExtension(sPath));
                            zip.Save(Path.GetDirectoryName(sPath) + "\\" + sName + ".zip");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oItem.error = ex.Message;
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                string sContent = Newtonsoft.Json.JsonConvert.SerializeObject(oItem, new JavaScriptDateTimeConverter());
                context.Response.Clear();
                context.Response.Write(context.Request["jsoncallback"] + "(" + sContent + ")");
                context.Response.End();
            }
        }
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
                        string sFile = IdToFile(arr[0], context);
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
                            return;
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

        public static string getFullPath(string sname, HttpContext context)
        {
            string sRoot = System.Configuration.ConfigurationManager.AppSettings["RootFolder"];
            if (sRoot.IndexOf(@":\") < 0) sRoot = context.Server.MapPath(sRoot);
            if (!sRoot.EndsWith(@"\")) sRoot += @"\";

            if (sname != null && sname.Length > 0)
            {
                if (sname.StartsWith("Root/"))
                    sname = (sRoot + sname.Substring(5, sname.Length - 5)).Replace('/', '\\');
                else if (sname.StartsWith("Root"))
                    sname = (sRoot + sname.Substring(4, sname.Length - 4)).Replace('/', '\\');
            }
            return sname;
        }

        /********************************************************************/
        private bool checkexistFile(string sName, HttpContext context)
        {
            return false;
        }

        private void createFolder(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            FileInfo oItem = new FileInfo();

            try
            {
                string sName = context.Request.QueryString["newname"];
                string sFolder = context.Request.QueryString["fd"];
                sFolder = getFullPath(sFolder, context);

                if (!checkNameNotUse(sName, false))
                {
                    oItem.error = "canot use folder name";
                    return;
                }

                if (!Directory.Exists(sFolder))
                {
                    oItem.error = "current folder not exist";
                }
                else
                {
                    sName = sFolder.EndsWith("\\") ? sFolder + sName : sFolder + @"\" + sName;
                    if (Directory.Exists(sName))
                    {
                        oItem.error = "canot create folder because exist";
                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(sName);
                    }
                }
            }
            catch (Exception ex)
            {
                oItem.error = ex.Message;
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                string sContent = Newtonsoft.Json.JsonConvert.SerializeObject(oItem, new JavaScriptDateTimeConverter());
                context.Response.Clear();
                context.Response.Write(context.Request["jsoncallback"] + "(" + sContent + ")");
                context.Response.End();
            }
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

        private bool checkNameNotUse(string sName, bool isFile)
        {
            string[] notName = { "CON", "PRN", "NUL", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9" };

            if (isFile)
            {
                sName = Path.GetFileNameWithoutExtension(sName);
            }

            foreach (string s in notName)
            {
                if (sName.ToUpper() == s)
                    return false;
            }
            return true;
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