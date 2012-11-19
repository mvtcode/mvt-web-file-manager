using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebFileManager.ajax
{
    [Serializable]
    public class FileInfo
    {
        public string id { get; set; }
        public string path { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public bool isFile { get; set; }
        public string length { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateEdit { get; set; }
        public bool isReadOnly { get; set; }
        public bool isHidden { get; set; }
        public bool isSystem { get; set; }
        public string error { get; set; }
        public string url { get; set; }
    }
}