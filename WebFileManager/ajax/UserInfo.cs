using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebFileManager.ajax
{
    [Serializable]
    public class UserInfo
    {
        public string id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public int codeError { get; set; }
        public string msg { get; set; }
    }
}