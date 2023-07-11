using System.Linq;
using Aware.Util;
using System.Collections.Generic;
using Aware.Dependency;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Search;

namespace Aware.ECommerce.Util
{
    public static class SqlHelper
    {
        public static string GetOpportunityItems(int regionID, int topInfo)
        {
            return GetCallScript("GetOpportunityItems", "{0},{1}", regionID, topInfo);
        }

        public static string GetRegionStores(int regionID)
        {
            return GetCallScript("GetStores", "{0}", regionID);
        }

        public static string ClearBasket(int userID, int basketID, bool deleteBasket)
        {
            return GetCallScript("ClearBasket", "{0},{1},{2}", userID, basketID, deleteBasket ? 1 : 0);
        }

        public static string ClearFavorites(int userID, int storeID)
        {
            return GetCallScript("ClearFavorites", "{0},{1}", userID, storeID);
        }

        public static string GetUserStore(int userID, int storeID)
        {
            return GetCallScript("GetUserStore", "{0},{1}", userID, storeID);
        }

        public static string GetCommentStats(int ownerID, int relationID, int relationType)
        {
            return GetCallScript("GetCommentStats", "{0},{1},{2}", relationID, relationType, ownerID);
        }

        public static string RefreshProductProperties(int productID = 0)
        {
            return GetCallScript("RefreshProductProperties", "{0} ", productID);
        }

        public static string RefreshProductImages(int productID = 0)
        {
            return GetCallScript("RefreshProductImages", "{0} ", productID);
        }

        public static string GetPropertyValues(bool justDefinitions, int propertyID)
        {
            var definition = justDefinitions ? 1 : 0;
            return GetCallScript("GetPropertyValues", "{0},{1}", definition, propertyID);
        }
        
        public static string GetStoreStatistics(int customerID)
        {
            return GetCallScript("GetStoreStatistics", "{0}", customerID);
        }

        public static string GetHomeCategoryItems(int regionID)
        {
            return GetCallScript("GetHomeCategoryItems", "{0}", regionID);
        }

        public static string SetDefaultLanguage(int languageID)
        {
            return GetCallScript("SetDefaultLanguage", "{0}", languageID);
        }

        public static string RefreshProductBrand(string newName, string oldName)
        {
            return GetCallScript("RefreshProductBrand", "'{0}','{1}'", newName, oldName);
        }

        public static string RefreshFileRelationInfo(int relationID, int relationType, string fileInfo = "")
        {
            return GetCallScript("RefreshFileRelationInfo", "{0},{1},'{2}'", relationID, relationType, fileInfo);
        }

        private static string GetCallScript(string spName, string paramString, params object[] param)
        {
            var callScript = string.Empty;
            switch (Config.DatabaseType)
            {
                case DatabaseType.MsSQL:
                    callScript = string.Format("exec {0} {1}", spName, paramString); break;
                case DatabaseType.MySQL:
                    callScript = string.Format("call {0}({1})", spName, paramString); break;
            }
            return string.Format(callScript, param);
        }

        public static string GetProductSearchSQL(ItemSearchParams searchParams)
        {
            if (searchParams != null)
            {
                var subSql = GetProductSearchFilterSql(searchParams);
                if (subSql != null)
                {
                    var sql = new System.Text.StringBuilder();
                    if (Config.DatabaseType == DatabaseType.MsSQL)
                    {
                        var orderByColumn = string.Empty;
                        var orderByColumn2 = string.Empty;
                        if (searchParams.SortList != null && searchParams.SortList.Any())
                        {
                            var c = 0;
                            foreach (var sorter in searchParams.SortList)
                            {
                                var field = string.Format("MIN({0})", sorter.DynamicOnField.ToString()
                                    .Replace("i => i.Product.", "P.").Replace("i => i.", "SI."));

                                orderByColumn += string.Format("{0} AS SR_{1}, ", field, c);
                                orderByColumn2 = string.Format("FPI.SR_{0} {1}, ", c, sorter.Descending ? "DESC" : "ASC");
                                c++;
                            }
                        }
                        else
                        {
                            orderByColumn = "MIN(SI.SalesPrice) AS SORT_ROW";
                            orderByColumn2 = "FPI.SORT_ROW ASC";
                        }

                        sql.AppendLine("WITH FilteredProductInfo AS ( ");
                        sql.AppendFormat(" SELECT P.ID, {0} ", orderByColumn.Trim().Trim(','));
                        sql.AppendFormat("{0} ", subSql);
                        sql.AppendLine("GROUP BY P.ID) ");

                        sql.AppendFormat("SELECT TOP {0} PP.* FROM (", searchParams.Size);
                        sql.AppendFormat("  SELECT FPI.ID, ROW_NUMBER() OVER (ORDER BY {0}) AS ROW_NUM ", orderByColumn2.Trim().Trim(','));
                        sql.AppendLine("  FROM FilteredProductInfo AS FPI	 ");
                        sql.AppendFormat(") AS TT INNER JOIN Product (NOLOCK) PP ON PP.ID=TT.ID WHERE TT.ROW_NUM>{0}", searchParams.Size * searchParams.Page);
                    }
                    else if (Config.DatabaseType == DatabaseType.MySQL)
                    {
                        sql.AppendFormat("call SearchProducts(\"{0}\",{1},{2});", subSql.ToString(), searchParams.Size, searchParams.Size * searchParams.Page);
                    }
                    return sql.ToString();
                }
            }
            return string.Empty;
        }

        public static string GetProductSearchCountSQL(ItemSearchParams searchParams)
        {
            if (searchParams != null)
            {
                var topInfo = searchParams.Size * (searchParams.Page + 1);
                var subSQL = GetProductSearchFilterSql(searchParams);
                if (subSQL != null)
                {
                    var sql = new System.Text.StringBuilder();
                    sql.AppendFormat("SELECT COUNT(DISTINCT P.ID) {0}", subSQL.ToString());
                    return sql.ToString();
                }
            }
            return string.Empty;
        }

        public static string GetItemSearchSql(ItemSearchParams searchParams, List<int> productIDList)
        {
            if (searchParams != null)
            {
                var subSql = GetProductSearchFilterSql(searchParams);
                if (subSql != null)
                {
                    var sql = new System.Text.StringBuilder();
                    sql.AppendFormat("SELECT SI.* {0}", subSql);

                    if (productIDList != null && productIDList.Any())
                    {
                        var productIDs = string.Join(",", productIDList);
                        sql.AppendFormat("AND SI.ProductID IN ({0}) ", productIDs);
                    }
                    return sql.ToString();
                }
            }
            return string.Empty;
        }

        private static System.Text.StringBuilder GetProductSearchFilterSql(ItemSearchParams searchParams)
        {
            if (searchParams != null)
            {
                var sql = new System.Text.StringBuilder();
                sql.AppendLine("FROM Product (NOLOCK) P INNER JOIN StoreItem (NOLOCK)  SI ON SI.ProductID=P.ID ");
                sql.AppendLine("WHERE P.Status=1 AND SI.Status=1 AND SI.IsForSale=1 ");

                if (!string.IsNullOrEmpty(searchParams.IDsString))
                {
                    sql.AppendFormat("AND P.ID IN ({0}) ", searchParams.IDsString.Trim(','));
                }

                var storeIDs = searchParams.StoreIDs ?? searchParams.RegionStoreIDs;
                if (storeIDs != null && storeIDs.Any())
                {
                    sql.AppendFormat("AND SI.StoreID IN ({0}) ", string.Join(",", storeIDs));
                }

                if (searchParams.CategoryIDs != null && searchParams.CategoryIDs.Any())
                {
                    var categoryService = WindsorBootstrapper.Resolve<ICategoryService>();
                    var categoryIDs = string.Join(",", categoryService.GetRelatedCategoryIDs(searchParams.CategoryIDs.ToList()));
                    sql.AppendFormat("AND P.CategoryID IN ({0}) ", categoryIDs);
                }

                if (!string.IsNullOrEmpty(searchParams.Keyword))
                {
                    sql.AppendFormat("AND (P.Name LIKE '%{0}%' OR P.PropertyInfo LIKE '%{0}%') ", searchParams.Keyword.Trim());
                }
                return sql;
            }
            return null;
        }
    }
}