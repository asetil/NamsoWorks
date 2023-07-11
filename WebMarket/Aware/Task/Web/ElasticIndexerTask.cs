using System;
using System.Collections.Generic;
using System.Linq;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model;
using Aware.Search.ElasticSearch;
using Aware.Search.ElasticSearch.Model;
using Aware.Util;
using Aware.Util.Model;
using Aware.ECommerce.Enums;
using Aware.Util.Enums;

namespace Aware.Task.Web
{
    public class ElasticIndexerTask : BaseTask
    {
        private readonly IElasticService _elasticService;
        private readonly IProductService _productService;
        private readonly IStoreItemService _storeItemService;

        public ElasticIndexerTask(IProductService productService, IElasticService elasticService, IStoreItemService storeItemService)
        {
            _productService = productService;
            _elasticService = elasticService;
            _storeItemService = storeItemService;
        }

        public override Result Execute()
        {
            try
            {
                var lastIndexDate = DateTime.Now.AddMinutes(-10);
                var result = RefreshElasticItems(lastIndexDate, false);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("ElasticIndexerTask > Execute - Failed", ex);
                return Result.Error(ex.Message);
            }
        }

        public override TaskType Type
        {
            get { return TaskType.ElasticIndexer; }
        }

        private Result RefreshElasticItems(DateTime dateTime, bool refreshAll = false)
        {
            try
            {
                Logger.Info("ElasticSearchManager > RefreshElasticItems > Started");
                if (_elasticService.Status != ElasticStatus.Active)
                {
                    return Result.Error("Elastic search is not available!");
                }

                //Delete  all indexed
                _elasticService.DeleteIndex(string.Empty, refreshAll);

                //Create
                var indexName = Config.ElasticIndex;

                var indexExist = _elasticService.IndexExist(indexName);
                if (!indexExist)
                {
                    _elasticService.CreateIndex(indexName);
                }

                var start = refreshAll ? DateTime.MinValue : dateTime;
                var items = _storeItemService.GetModifiedItems(start);
                var products = items.Select(i => i.Product).Distinct().ToList();
                products = products.Select(i =>
                {
                    i.Items = items.Where(it => it.ProductID == i.ID);
                    return i;
                }).ToList();

                _elasticService.InsertMany(MapElastic(products), indexName, 1000, !refreshAll);

                Logger.Info("ElasticSearchManager > RefreshElasticItems > Completed");
                return Result.Success();
            }
            catch (Exception ex)
            {
                Logger.Error("ElasticSearchManager > RefreshElasticItems - Failed", ex);
                return Result.Error(string.Format("İndex güncelleme işlemi başarısız! {0}", ex.Message));
            }
        }

        private List<ElasticProduct> MapElastic(List<Product> products)
        {
            if (products != null && products.Any())
            {
                var aa = products.Where(i => i.Properties.Any());
                return products.Select(i => new ElasticProduct()
                {
                    ID = i.ID,
                    Name = i.Name,
                    ShortDescription = i.ShortDescription,
                    Description = i.Description,
                    Brand = i.Brand,
                    CategoryID = i.CategoryID,
                    PropertyInfo = i.PropertyInfo,
                    ImageInfo = i.ImageInfo,
                    Unit = i.Unit,
                    Status = i.Status,
                    DateModified = i.DateModified,
                    Barcode = i.Barcode,
                    Rating = i.CommentRating,
                    CommentCount = i.CommentCount,
                    Properties = i.Properties.Where(it => it.Type == PropertyType.Selection).Select(it => new IDNamePair()
                    {
                        ID = it.ID,
                        Value = it.Value
                    }).ToList(),
                    Items = i.Items.Select(it => new ElasticItem()
                    {
                        ID = it.ID,
                        StoreID = it.StoreID,
                        Status = it.Status,
                        Stock = it.Stock,
                        SalesPrice = it.SalesPrice,
                        ListPrice = it.ListPrice,
                        DateModified = it.DateModified,
                        IsForSale = it.IsForSale,
                        HasVariant = it.HasVariant
                    }).ToList()
                }).ToList();
            }
            return null;
        }
    }
}
