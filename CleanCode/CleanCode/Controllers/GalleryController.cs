using System;
using System.Linq;
using Aware.File;
using Aware.Util.View;
using System.IO;
using System.Web.Mvc;
using CleanCode.Helper;
using CleanCode.Helper.Model;
using Aware.Util.Model;
using System.Collections.Generic;

namespace CleanCode.Controllers
{
    [Aware.Util.Filter.Authorize(Aware.Util.Enums.AuthorizeLevel.SuperUser)]
    public class GalleryController : AwareController
    {
        private readonly string BASE_PATH = "/res/img";
        private readonly IFileService _fileService;

        public GalleryController(IFileService fileService)
        {
            _fileService = fileService;
        }

        public ActionResult Index()
        {
            ViewBag.CurrentPath = BASE_PATH;
            return View();
        }

        [HttpPost]
        public JsonResult LoadPath(string path)
        {
            var directoryExists = string.IsNullOrEmpty(path) ? false : Directory.Exists(Server.MapPath(path));
            if (!directoryExists)
            {
                return Json(new { success = 0, message = string.Format("{0} dizini mevcut değil!", path) }, JsonRequestBehavior.DenyGet);
            }

            var directory = Server.MapPath(path);
            var directoryPathList = Directory.GetDirectories(directory).ToList();
            var filePathList = Directory.GetFiles(directory).ToList();

            var model = new FileBrowserModel
            {
                BasePath = BASE_PATH,
                CurrentPath = path,
                FileList = filePathList.Select(i => new FileInfo(i)).ToList(),
                DirectoryList = directoryPathList.Select(i => new DirectoryInfo(i)).ToList(),
            };

            var html = this.RenderPartialView("_GalleryBrowser", model);
            return Json(new { html, success = 1 }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult AddDirectory(string path, string name)
        {
            var result = Result.Error();
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(name))
            {
                var directoryPath = string.Format("{0}/{1}", path, name.Replace(" ", "_"));
                if (Directory.Exists(Server.MapPath(directoryPath)))
                {
                    return Json(new { success = 0, message = string.Format("{0} dizini zaten mevcut!", directoryPath) }, JsonRequestBehavior.DenyGet);
                }

                var info = Directory.CreateDirectory(Server.MapPath(directoryPath));
                if (info != null)
                {
                    result = Result.Success();
                }
            }
            return ResultValue(result);
        }

        [HttpPost]
        public JsonResult RemoveDirectory(string path)
        {
            var result = Result.Error();
            if (!string.IsNullOrEmpty(path))
            {
                if (!Directory.Exists(Server.MapPath(path)))
                {
                    return Json(new { success = 0, message = string.Format("{0} dizini mevcut değil!", path) }, JsonRequestBehavior.DenyGet);
                }

                Directory.Delete(Server.MapPath(path), true);
                var pathList = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                path = string.Format("/{0}", string.Join("/", pathList.Take(pathList.Count() - 1)));
                result = Result.Success(path);
            }
            return Json(new { success = result.IsSuccess, message = result.Message, value = result.Value }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult AjaxFileUpload(string path)
        {
            var postedFiles = GetPostedFiles();
            var result = _fileService.SaveFilesToDirectory(path, postedFiles);
            return ResultValue(result);
        }

        [HttpPost]
        public JsonResult RemoveFile(string path)
        {
            var result = _fileService.DeletePhysicalFile(path);
            return ResultValue(result);
        }

        private static List<string> GetDirectories(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (searchOption == SearchOption.TopDirectoryOnly)
                return Directory.GetFiles(path, searchPattern).ToList();

            var directories = new List<string>(GetDirectories(path, searchPattern));
            for (var i = 0; i < directories.Count; i++)
                directories.AddRange(GetDirectories(directories[i], searchPattern));

            return directories;
        }

        private static List<string> GetDirectories(string path, string searchPattern)
        {
            try
            {
                return Directory.GetDirectories(path, searchPattern).ToList();
            }
            catch (UnauthorizedAccessException)
            {
                return new List<string>();
            }
        }
    }
}