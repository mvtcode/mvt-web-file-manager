using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MVT.Core;

namespace WebFileManager
{
    public partial class _Default : System.Web.UI.Page
    {
        private string sFolder = @"E:\music\";
        protected static string sTree = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();

                Load_Treeview();

                //TreeNode node=new TreeNode("Root",sFolder);
                //node.Expanded = true;
                //ListDirectories(sFolder, ref node);
                //TreeView1.Nodes.Clear();
                //TreeView1.Nodes.Add(node);
                ////TreeView1.CollapseAll();
            }
        }

        private void Load_Treeview()
        {
            //if(sTree!="") return;
            //sTree = "<li><span title=\"Root\" class=\"folder\">Root</span>";
            sTree = "<li><span title=\"Root\" class=\"open\">Root</span>";
            ListDirectories(sFolder, ref sTree);
            sTree += "</li>";
            //Server.MapPath()
        }

        private void LoadData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("FullName");
            dt.Columns.Add("name");
            dt.Columns.Add("length");
            dt.Columns.Add("DateCreate");
            dt.Columns.Add("DateEdit");

            //System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(Server.MapPath(sFolder));
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(sFolder);
            foreach (System.IO.FileInfo f in dir.GetFiles("*.*"))
            {
                DataRow dr = dt.Rows.Add();
                dr["FullName"] = f.FullName;
                dr["name"] = f.Name;
                dr["length"] = UntilityFunction.ShowCappacityFile(f.Length);
                dr["DateCreate"] = f.CreationTime;
                dr["DateEdit"] = f.LastWriteTime;
            }
            GV_File.DataSource = dt;
            GV_File.DataBind();
        }

        //private void ListDirectories(string path, ref TreeNode node)
        //{
        //    var directories = Directory.GetDirectories(path);
        //    if (directories.Any())
        //    {
        //        foreach (var directory in directories)
        //        {
        //            var di = new DirectoryInfo(directory);
        //            TreeNode sub = new TreeNode(di.Name,di.FullName);
        //            sub.Expanded = false;
        //            node.ChildNodes.Add(sub);
        //            ListDirectories(directory, ref sub);
        //        }
        //    }
        //}

        private void ListDirectories(string path, ref string sPath)
        {
            var directories = Directory.GetDirectories(path);
            if (directories.Any())
            {
                sPath += "<ul>";
                foreach (var directory in directories)
                {
                    var di = new DirectoryInfo(directory);
                    //sPath += string.Format("<li><span title=\"{0}\" class=\"folder\">{1}</span>", di.FullName.Replace(sFolder, "Root\\"), di.Name);
                    sPath += string.Format("<li><span title=\"{0}\">{1}</span>", di.FullName.Replace(sFolder, "Root\\"), di.Name);
                    ListDirectories(directory, ref sPath);
                    sPath += "</li>";
                }
                sPath += "</ul>";
            }
        }

        protected void GV_File_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

        }

        protected void GV_File_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        protected void GV_File_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //int iRow = e.Row.DataItemIndex;

            if (e.Row.DataItemIndex != -1)
            {
                string sFullName = GV_File.DataKeys[e.Row.RowIndex].Value.ToString();
                string sFile = sFullName;
                ImageButton btDelete = (ImageButton)e.Row.Cells[5].FindControl("imgbtDelete");
                //if (btDelete != null) btDelete.Attributes.Add("onclick", "javascript:return SetDelete('" + sFile + "')");
                if (btDelete != null) btDelete.Attributes.Add("onclick", "SetDelete('xzzz');return false;");
            }
        }
    }
}
