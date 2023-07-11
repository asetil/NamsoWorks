using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Aware.Util.Enums;

namespace WebMarket.Admin.Models
{
    public class FileBrowserModel
    {

        public string BasePath { get; set; }
        public string CurrentPath { get; set; }
        public List<FileInfo> FileList { get; set; }
        public List<DirectoryInfo> DirectoryList { get; set; }

        public bool CanCreateDirectory
        {
            get
            {
                var path = CurrentPath.ToLowerInvariant().Replace(BasePath, "");
                return string.IsNullOrEmpty(path) || !excludeList.Contains(path);
            }
        }

        public bool CanDeleteDirectory
        {
            get
            {
                var path = CurrentPath.ToLowerInvariant().Replace(BasePath, "");
                return CurrentPath.IndexOf(BasePath) > -1 && !string.IsNullOrEmpty(path) && !excludeList.Contains(path);
            }
        }

        private readonly List<string> excludeList = new List<string>() {
            "/product","/app","/bank","/brand","/campaign","/category",
            "/customer","/icons","/lang","/slider","/store"
        };
    }
}
