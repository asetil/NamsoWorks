using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Aware.Dependency;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model;
using Aware.Util;
using Aware.Util.Log;
using Aware.Util.Model;
using Aware.ECommerce.Enums;
using Aware.File;
using Aware.File.Model;
using Aware.Util.Enums;

namespace Aware.ECommerce.Manager
{
    public class UploadItemManager : IUploadItemManager
    {
        private readonly IProductService _productService;
        private readonly IStoreItemService _itemService;
        private readonly ILogger _logger;

        public UploadItemManager(IProductService productService, IStoreItemService itemService, ILogger logger)
        {
            _productService = productService;
            _itemService = itemService;
            _logger = logger;
        }

        public Result UploadStoreItems(HttpRequestBase request, int storeID)
        {
            try
            {
                var file = request.Files.Count > 0 ? request.Files[0] : null;
                if (file == null || file.InputStream == null || file.InputStream.Length == 0)
                {
                    return Result.Error(Resource.StoreItem_ImportInvalidFile);
                }

                var extension = Path.GetExtension(file.FileName);
                if (extension == ".txt")
                {
                    return UploadFromTemplate(file, storeID);
                }

                if (extension != ".xls" && extension != ".xlsx")
                {
                    return Result.Error(Resource.StoreItem_ImportInvalidFile);
                }

                var filePath = HttpContext.Current.Server.MapPath(string.Format("~/Resource/{0}", file.FileName));
                file.SaveAs(filePath);

                var connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                String query = "SELECT * FROM [Product_Catalog$]";

                var list = new List<StoreItem>();
                using (OleDbConnection connection = new OleDbConnection(connString))
                {
                    OleDbCommand command = new OleDbCommand(query, connection);
                    connection.Open();

                    OleDbDataReader reader = command.ExecuteReader();
                    while (reader != null && reader.Read())
                    {
                        list.Add(new StoreItem()
                        {
                            ID = reader[0].ToString().Int(),
                            StoreID = reader[1].ToString().Int(),
                            ProductID = reader[2].ToString().Int(),

                            Product = new Product() { ID = reader[2].ToString().Int(), Barcode = reader[5].ToString() },
                            ListPrice = reader[7].ToString().Dec(),
                            SalesPrice = reader[8].ToString().Dec(),
                            Stock = reader[9].ToString().Dec(),
                            Status = (Statuses)(reader[10].ToString().Int()),
                        });
                    }
                    if (reader != null) reader.Close();
                }

                var barcodeList = list.Where(i => i.ProductID == 0 && !string.IsNullOrEmpty(i.Product.Barcode)).Select(i => i.Product.Barcode).ToList();
                var barcodeItems = _productService.GetBarcodeProducts(barcodeList);

                foreach (var item in list)
                {
                    item.StoreID = (item.StoreID > 0 ? item.StoreID : storeID);
                    if (item.StoreID <= 0 || item.Status == Statuses.Rejected) { continue; }

                    if (item.ProductID == 0 && barcodeItems.Any())
                    {
                        var product = barcodeItems.FirstOrDefault(i => i.Barcode == item.Product.Barcode);
                        item.ProductID = product != null ? product.ID : 0;
                    }

                    item.Product = null;
                    _itemService.Save(item);
                }

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                return Result.Success(null, Resource.StoreItem_ImportOperationSuccessfull);
            }
            catch (Exception ex)
            {
                _logger.Error("Product > UploadStoreItems - Failed.", ex);
            }
            return Result.Error(Resource.StoreItem_ImportOperationFailed);
        }

        private Result UploadFromTemplate(HttpPostedFileBase file, int storeID)
        {
            try
            {
                if (file == null || file.InputStream == null || file.InputStream.Length == 0)
                {
                    return Result.Error(Resource.StoreItem_ImportInvalidFile);
                }

                var stream = file.InputStream;
                byte[] bytes = new byte[stream.Length];
                stream.Position = 0;
                stream.Read(bytes, 0, (int)stream.Length);

                string data = Encoding.UTF8.GetString(bytes);
                var list = data.DeSerialize<IEnumerable<StoreItem>>();

                return UploadJsonItems(list, storeID);

                var barcodeList = list.Where(i => i.ProductID == 0 && !string.IsNullOrEmpty(i.Product.Barcode)).Select(i => i.Product.Barcode).ToList();
                var barcodeItems = _productService.GetBarcodeProducts(barcodeList);

                foreach (var item in list)
                {
                    item.StoreID = (item.StoreID > 0 ? item.StoreID : storeID);
                    if (item.StoreID <= 0 || item.Status == Statuses.Rejected) { continue; }

                    if (item.ProductID == 0 && barcodeItems != null && barcodeItems.Any())
                    {
                        var product = barcodeItems.FirstOrDefault(i => i.Barcode == item.Product.Barcode);
                        item.ProductID = product != null ? product.ID : 0;
                    }

                    item.ID = 0;
                    item.Product = null;
                    _itemService.Save(item);
                }
                return Result.Success(null, Resource.StoreItem_ImportOperationSuccessfull);
            }
            catch (Exception ex)
            {
                _logger.Error("Product > UploadFromTemplate - Failed.", ex);
            }
            return Result.Error(Resource.StoreItem_ImportOperationFailed);
        }

        private Result UploadJsonItems(IEnumerable<StoreItem> itemList, int storeID)
        {
            try
            {
                var fileService = WindsorBootstrapper.Resolve<IFileService>();
                var imageUrl = ""; //"https://img-carrefour.mncdn.com/mnresize/700/700/faces/Picture/";
                var imageSaveDirectory = "D:\\WM\\WebMarket\\WebMarket.Admin\\resource\\img\\Product\\";

                foreach (var item in itemList)
                {
                    item.ID = 0;
                    item.StoreID = (item.StoreID > 0 ? item.StoreID : storeID);
                    item.ListPrice = item.ListPrice > 0 ? item.ListPrice : item.SalesPrice * 1.1M;
                    item.Status = Statuses.Active;
                    item.Stock = new Random().Next(0,30);
                    item.IsForSale = true;

                    var product = item.Product;
                    var imageInfo = string.Format("{0}{1}", imageUrl, product.ImageInfo);
                    

                    var existing = _productService.GetProductWithName(item.Product.Name.Trim());
                    if (existing == null)
                    {
                        existing = item.Product;
                        existing.ID = 0;
                        existing.Status = Statuses.Active;
                        _productService.SaveProduct(existing);

                        item.ProductID = existing.ID;
                        _itemService.Save(item);
                    }
                    else
                    {
                        var existingItem = _itemService.Get(item.StoreID, existing.ID);
                        if (existingItem != null)
                        {
                            existingItem.SalesPrice = item.SalesPrice;
                            existingItem.ListPrice = item.ListPrice;
                            _itemService.Save(existingItem);
                        }
                        else
                        {
                            item.ProductID = existing.ID;
                            item.Product = null;
                            _itemService.Save(item);
                            item.Product = existing;
                        }
                    }

                    var path = string.Format("{0}.{1}",existing.ID, item.Product.ImageInfo.Split('.').LastOrDefault());
                    var savePath = string.Format("{0}{1}", imageSaveDirectory, path);
                    WebRequester.DownloadFile(imageInfo, savePath);
                    
                    var file = new FileRelation()
                    {
                        RelationID = existing.ID,
                        RelationType = (int)RelationTypes.Product,
                        Name = item.Product.Name.Short(12),
                        Path = path,
                        Status = Statuses.Active,
                        SortOrder = "0",
                        Size = 0
                    };

                    fileService.SaveGallery(file,null);
                }
                return Result.Success(null, Resource.StoreItem_ImportOperationSuccessfull);
            }
            catch (Exception ex)
            {
                _logger.Error("Product > UploadFromTemplate - Failed.", ex);
            }
            return Result.Error(Resource.StoreItem_ImportOperationFailed);
        }
    }
}
