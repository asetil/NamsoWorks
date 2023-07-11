using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model;
using Aware.ECommerce.Search;
using Aware.Search;
using WebMarket.Admin.Helper;
using Aware.File;
using Aware.File.Model;

namespace WebMarket.Admin.Controllers
{
    public class FileController : BaseController
    {
        private readonly IFileService _fileService;
        private readonly IStoreItemService _storeItemService;

        public FileController(IFileService fileService, IStoreItemService storeItemService)
        {
            _fileService = fileService;
            _storeItemService = storeItemService;
        }

        [HttpPost]
        public JsonResult GetItemsForExport(int storeID)
        {
            var result = SearchItems(storeID, 1);
            var html = this.RenderPartialView("~/Views/StoreItem/_ExportItems.cshtml", result);
            return Json(new { success = 1, html });
        }

        [HttpPost]
        public JsonResult GetFileDetail(int fileID, int relationID, int relationType)
        {
            var file = _fileService.GetFile(fileID, relationID, relationType);
            return Json(new { success = 1, file });
        }

        [HttpPost]
        public JsonResult RefreshGallery(int relationID, int relationType, int size)
        {
            var html = string.Empty;
            var gallery = _fileService.GetGallery(relationID, relationType, size);
            if (gallery != null)
            {
                gallery.ViewMode = GetViewMode();
                html = this.RenderPartialView("_FileGallery", gallery);
            }
            return Json(new { html }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult AjaxFileUpload(FileRelation model)
        {
            var postedFiles = GetPostedFiles();
            var result = _fileService.SaveGallery(model, postedFiles);
            return ResultValue(result);
        }

        [HttpPost]
        public JsonResult DeleteFile(int fileID, int relationID, int relationType)
        {
            var result = _fileService.Delete(fileID, relationID, relationType, true);
            return ResultValue(result);
        }

        public void ExportToExcell(int storeID)
        {
            var result = SearchItems(storeID, 1);
            GridView gv = new GridView();
            gv.DataSource = result.Results;
            gv.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/ms-excel";
            Response.AddHeader("content-disposition", "attachment;filename=StoreItemList.xls");
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            Response.Charset = "";

            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            gv.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
        }

        public void ExportToTemplate(int storeID)
        {
            var result = SearchItems(storeID, 1);
            var exportList = result.Results.Select(i => new
            {
                i.ID,
                i.StoreID,
                i.ProductID,
                i.ListPrice,
                i.SalesPrice,
                i.Stock,
                i.Status
            });

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "text/plain";
            Response.AddHeader("content-disposition", "attachment;filename=data.txt");
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            Response.Charset = "";

            StringBuilder sb = new StringBuilder();
            string output = Aware.Util.Common.Serialize(exportList);
            sb.Append(output);
            sb.Append("\r\n");
            Response.Write(sb.ToString());

            Response.Flush();
            Response.End();
        }

        public FilePathResult SampleFormat()
        {
            string filePath = Server.MapPath("~/Resource/sampleFormat.xlsx");
            return File(filePath, "application/x-ms-excel", "SampleFormat.xlsx");
        }

        private SearchResult<StoreItem> SearchItems(int storeID, int page)
        {
            var searchParams = new ItemSearchParams(string.Empty, page, 0).WithStore(storeID);
            searchParams.Fields = "Product";

            var result = _storeItemService.GetItems(searchParams, CurrentUser.CustomerID);
            return result != null ? result.SearchResult : null;
        }
    }
}