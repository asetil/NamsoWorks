using System.Collections.Generic;
using System.Linq;
using Aware.Dependency;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model;
using Aware.Search;
using Aware.Search.ElasticSearch.Model;
using Aware.Util;

namespace Aware.ECommerce.Search
{
    public class ProductSearchResult : SearchResult<Product>
    {
        public ProductSearchResult(ItemSearchParams searchParams)
        {
            SearchParams = searchParams;
        }

        public new ItemSearchParams SearchParams { get; set; }
        public List<Category> Categories { get; set; }
        public List<Store> Stores { get; set; }

        public override bool HasMore
        {
            get
            {
                return HasResult && SearchParams != null && TotalSize > ((SearchParams.Page+1) * SearchParams.Size);
            }
        }
        
        public void ArrangeAggregations()
        {
            var arrangedAggs = new List<AggregationResult>();
            var aggs = Aggregations ?? new List<AggregationResult>();

            foreach (var aggregation in aggs)
            {
                if (aggregation.MapType == AgregationMapType.Category)
                {
                    var allCategories = Categories;
                    var categories = allCategories.Where(i => i.Level == 1);
                    var categoryIDs = SearchParams.CategoryIDs;
                    if (categoryIDs != null && categoryIDs.Any())
                    {
                        categories = allCategories.Where(i => categoryIDs.Contains(i.ParentID));
                    }

                    var aggregationItems = GetAggregationItems(aggs, aggregation.MapType);
                    var aggItems = categories.Select(c =>
                    {
                        var subItems = allCategories.Where(sc => sc.ID == c.ID || sc.SortOrder.StartsWith(c.SortOrder)).Select(i => i.ID).ToList();
                        var count = aggregationItems.Where(a => subItems.Contains(a.Term.Int())).Sum(s => s.Count);

                        return new AggregationItem()
                        {
                            Text = c.Name,
                            Term = c.ID.ToString(),
                            IsActive = categoryIDs.Any(id => id == c.ID),
                            Count = count
                        };
                    }).OrderByDescending(o => o.Count).ToList();

                    arrangedAggs.Add(new AggregationResult
                    {
                        Name = aggregation.Name,
                        SearchName = aggregation.SearchName,
                        MapType = AgregationMapType.Category,
                        Items = aggItems
                    });
                }
                else if (aggregation.MapType == AgregationMapType.Store)
                {
                    var aa = GetAggregationItems(aggs, aggregation.MapType);
                    var aggItems = Stores.Select(i => new AggregationItem()
                    {
                        Text = i.Name,
                        Term = i.ID.ToString(),
                        IsActive = SearchParams.StoreIDs.Any(id => id == i.ID),
                        Count = GetAggregationCount(aa, i.ID.ToString())
                    }).ToList();

                    arrangedAggs.Add(new AggregationResult()
                    {
                        Name = aggregation.Name,
                        SearchName = aggregation.SearchName,
                        MapType = AgregationMapType.Store,
                        Items = aggItems
                    });
                }
                else if (aggregation.MapType == AgregationMapType.Price && !string.IsNullOrEmpty(SearchParams.Price))
                {
                    var price = SearchParams.Price.S();
                    aggregation.Items = aggregation.Items.Select(i =>
                    {
                        i.IsActive = price.IndexOf(i.Term.S()) > -1;
                        return i;
                    }).ToList();
                    arrangedAggs.Add(aggregation);
                }
                else if (aggregation.MapType == AgregationMapType.Stock && !string.IsNullOrEmpty(SearchParams.Stock))
                {
                    var stock = SearchParams.Stock.S();
                    aggregation.Items = aggregation.Items.Select(i =>
                    {
                        i.IsActive = stock.IndexOf(i.Term.S()) > -1;
                        return i;
                    }).ToList();
                    arrangedAggs.Add(aggregation);
                }
                else if (aggregation.MapType == AgregationMapType.CommentRating)
                {
                    var rating = !string.IsNullOrEmpty(SearchParams.Rating) ? SearchParams.Rating.S() : string.Empty;
                    aggregation.Items = aggregation.Items.Select(i =>
                    {
                        i.IsActive = rating.IndexOf(i.Term.S()) > -1;
                        return i;
                    }).OrderByDescending(o => o.Term).ToList();
                    arrangedAggs.Add(aggregation);
                }
                else if (aggregation.MapType == AgregationMapType.Property)
                {
                    var propertyIDs = SearchParams.PropertyIDs ?? new List<int>();
                    var propertyService = WindsorBootstrapper.Resolve<IPropertyService>();
                    var properties = propertyService.GetAllCachedProperties();

                    aggregation.Items = aggregation.Items.Select(i =>
                    {
                        var prop = properties.FirstOrDefault(p => p.ID == i.Term.Int());
                        i.Text = prop.Name;
                        i.Childs = i.Childs.Select(c =>
                        {
                            //var childs = properties.Where(p => p.ParentID == prop.ID).Select(sub =>
                            //{
                            //    var xx = new AggregationItem();
                            //    xx.Term = sub.ID.ToString();
                            //    xx.Text = sub.Name;
                            //    xx.IsActive = propertyIDs.Any(id => id == sub.ID);

                            //    var tt = c.Items.FirstOrDefault(it => it.Term == xx.Term);
                            //    if (tt != null)
                            //    {
                            //        xx.Count = tt.Count;
                            //    }
                            //    return xx;
                            //});
                            //c.Items = childs.OrderByDescending(o=>o.Count).ToList();
                            //return c;

                            c.Items = c.Items.Select(it =>
                            {
                                var property = properties.FirstOrDefault(p => p.ParentID == i.Term.Int() && p.ID == it.Term.Int());
                                it.Text = property != null ? property.Name : it.Text;
                                it.IsActive = propertyIDs.Any(id => id.ToString() == it.Term);
                                return it;
                            }).OrderByDescending(o => o.Count).ToList();
                            return c;
                        }).ToList();
                        return i;
                    }).ToList();

                    arrangedAggs.Add(aggregation);
                }
                else
                {
                    arrangedAggs.Add(aggregation);
                }
            }
            Aggregations = arrangedAggs;
        }

        public string FilterString
        {
            get
            {
                var result = string.Empty;
                if (SearchParams != null)
                {
                    if (!string.IsNullOrEmpty(SearchParams.Keyword)) { result=string.Format("{0}&q={1}",result, SearchParams.Keyword);}
                    if (!string.IsNullOrEmpty(SearchParams.Barcode)) { result=string.Format("{0}&b={1}",result, SearchParams.Barcode);}
                    if (!string.IsNullOrEmpty(SearchParams.Price)) { result=string.Format("{0}&price={1}",result, SearchParams.Price);}
                    if (!string.IsNullOrEmpty(SearchParams.Stock)) { result=string.Format("{0}&stock={1}",result, SearchParams.Stock);}
                    if (SearchParams.OrderBy>0) { result=string.Format("{0}&sirala={1}",result, SearchParams.OrderBy);}
                    if (SearchParams.Page>0) { result=string.Format("{0}&page={1}",result, SearchParams.Page);}
                    if (SearchParams.StoreIDs!=null && SearchParams.StoreIDs.Any()) { result=string.Format("{0}&sid={1}",result, string.Join(",", SearchParams.StoreIDs));}
                    if (SearchParams.CategoryIDs!=null && SearchParams.CategoryIDs.Any()) { result=string.Format("{0}&cid={1}",result, string.Join(",", SearchParams.CategoryIDs));}
                }
                return result.Trim().Trim('&');
            }
        }
    }
}