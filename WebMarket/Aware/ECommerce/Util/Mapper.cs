using System;
using System.Collections.Generic;
using System.Linq;
using Aware.Authenticate.Model;
using Aware.Authority.Model;
using Aware.ECommerce.Enums;
using Aware.ECommerce.Model;
using Aware.File.Model;
using Aware.Mail;
using Aware.Payment.Model;
using Aware.Search.ElasticSearch.Model;

namespace Aware.Util
{
    public static class Mapper
    {
        public static void Map(ref User userToRefresh, User user, bool isAdmin)
        {
            if (userToRefresh == null || user == null) { return; }
            userToRefresh.Name = user.Name;
            userToRefresh.Email = user.Email;
            userToRefresh.DateModified = DateTime.Now;

            if (!isAdmin)
            {
                userToRefresh.Permissions = user.Permissions;
            }
            else if (userToRefresh.Role == UserRole.AdminUser)
            {
                userToRefresh.TitleID = user.TitleID;
            }
        }

        public static void Map(ref StoreItem modelToUpdate, StoreItem modelToMap)
        {
            if (modelToUpdate == null || modelToMap == null) { return; }
            modelToUpdate.StoreID = modelToMap.StoreID;
            modelToUpdate.ProductID = modelToMap.ProductID;
            modelToUpdate.SalesPrice = modelToMap.SalesPrice;
            modelToUpdate.ListPrice = modelToMap.ListPrice;
            modelToUpdate.Stock = modelToMap.Stock;
            modelToUpdate.Status = modelToMap.Status;
            modelToUpdate.IsForSale = modelToMap.IsForSale;
            modelToUpdate.HasVariant = modelToMap.HasVariant;
            modelToUpdate.DateModified = DateTime.Now;
        }

        public static void Map(ref Campaign modelToUpdate, Campaign modelToMap)
        {
            if (modelToUpdate == null || modelToMap == null) { return; }

            modelToUpdate.Name = modelToMap.Name;
            modelToUpdate.Description = modelToMap.Description;
            modelToUpdate.Scope = modelToMap.Scope;
            modelToUpdate.DiscountType = modelToMap.DiscountType;
            modelToUpdate.Discount = modelToMap.Discount;
            modelToUpdate.MinimumAmount = modelToMap.MinimumAmount;
            modelToUpdate.MinimumQuantity = modelToMap.MinimumQuantity;
            modelToUpdate.ExpireDays = modelToMap.ExpireDays;
            modelToUpdate.PublishDate = modelToMap.PublishDate;

            var filterInfo = !string.IsNullOrEmpty(modelToMap.FilterInfo) ? modelToMap.FilterInfo.Trim('&') : string.Empty;
            modelToUpdate.FilterInfo = filterInfo;
            modelToUpdate.ItemScope = modelToMap.ItemScope;
            modelToUpdate.Status = modelToMap.Status;
        }

        public static void Map(ref Order modelToUpdate, Order modelToMap)
        {
            if (modelToUpdate == null || modelToMap == null) { return; }
            modelToUpdate.ShippingAddressID = modelToMap.ShippingAddressID;
            modelToUpdate.BillingAddressID = modelToMap.BillingAddressID;
            modelToUpdate.ShippingMethodID = modelToMap.ShippingMethodID;
            modelToUpdate.PaymentType = modelToMap.PaymentType;
            modelToUpdate.Note = modelToMap.Note;
            modelToUpdate.DateModified = DateTime.Now;
        }

        public static void MapOrderTotals(ref Order order, Basket basket, decimal shippingPrice, decimal paymentCost)
        {
            if (order == null || basket == null) { return; }
            order.BasketTotal = basket.Total;
            order.DiscountTotal = basket.DiscountTotal;
            order.ShipmentCost = shippingPrice;
            order.PaymentCost = paymentCost;
            order.GrossTotal = basket.GrossTotal + shippingPrice + paymentCost;
        }

        public static void Map(ref BankInfo modelToUpdate, BankInfo modelToMap)
        {
            if (modelToUpdate == null || modelToMap == null) { return; }
            modelToUpdate.Name = modelToMap.Name;
            modelToUpdate.BranchName = modelToMap.BranchName;
            modelToUpdate.IBAN = modelToMap.IBAN;
            modelToUpdate.AccountNumber = modelToMap.AccountNumber;
            modelToUpdate.ImageUrl = modelToMap.ImageUrl;
            modelToMap.Status = Enums.Statuses.Active; //TODO# : Bunu yönetmeyecek miyiz?
        }

        public static void Map(ref InstallmentInfo modelToUpdate, InstallmentInfo modelToMap)
        {
            if (modelToUpdate == null || modelToMap == null) { return; }
            modelToUpdate.Name = modelToMap.Name;
            modelToUpdate.PosID = modelToMap.PosID;
            modelToUpdate.Count = modelToMap.Count;
            modelToUpdate.Commission = modelToMap.Commission;
            modelToMap.Status = Enums.Statuses.Active; //TODO# : Bunu yönetmeyecek miyiz?
        }

        public static void Map(ref Category modelToUpdate, Category modelToMap)
        {
            if (modelToUpdate == null || modelToMap == null) { return; }
            modelToUpdate.Name = modelToMap.Name;
            modelToUpdate.ParentID = modelToMap.ParentID;
            modelToUpdate.Status = modelToMap.Status;
        }

        public static void Map(ref Product modelToUpdate, Product modelToMap)
        {
            if (modelToUpdate == null || modelToMap == null) { return; }
            modelToUpdate.Name = modelToMap.Name;
            modelToUpdate.ShortDescription = modelToMap.ShortDescription;
            modelToUpdate.Description = modelToMap.Description;
            modelToUpdate.Brand = modelToMap.Brand;
            modelToUpdate.CategoryID = modelToMap.CategoryID;
            modelToUpdate.Barcode = modelToMap.Barcode;
            modelToUpdate.Unit = modelToMap.Unit;
            modelToUpdate.Status = modelToMap.Status;
            modelToUpdate.DateModified = DateTime.Now;
        }

        public static void Map(ref PropertyValue modelToUpdate, PropertyValue modelToMap)
        {
            if (modelToUpdate == null || modelToMap == null) { return; }
            modelToUpdate.Name = modelToMap.Name;
            modelToUpdate.Type = modelToMap.Type;
            modelToUpdate.ParentID = modelToMap.ParentID;
            modelToUpdate.Status = modelToMap.Status;
        }

        public static void Map(ref VariantProperty modelToUpdate, VariantProperty modelToMap)
        {
            if (modelToUpdate == null || modelToMap == null) { return; }
            modelToUpdate.Name = modelToMap.Name;
            modelToUpdate.DisplayName = modelToMap.DisplayName;
            modelToUpdate.IsRequired = modelToMap.IsRequired;
            modelToUpdate.TrackStock = modelToMap.TrackStock;
            modelToUpdate.SortOrder = modelToMap.SortOrder;
            modelToUpdate.MaxSelection = modelToMap.MaxSelection;
            modelToUpdate.DisplayMode = modelToMap.DisplayMode;
            modelToUpdate.Status = modelToMap.Status;
        }

        public static void Map(ref FileRelation modelToUpdate, FileRelation modelToMap)
        {
            if (modelToUpdate == null || modelToMap == null) { return; }
            modelToUpdate.Name = modelToMap.Name;
            modelToUpdate.SortOrder = modelToMap.SortOrder;
            modelToUpdate.Status = modelToMap.Status;
        }

        public static void Map(ref SimpleItem modelToUpdate, SimpleItem modelToMap)
        {
            if (modelToUpdate == null || modelToMap == null) { return; }
            modelToUpdate.Title = modelToMap.Title;
            modelToUpdate.Value = modelToMap.Value;
            modelToUpdate.Url = modelToMap.Url;
            modelToUpdate.SortOrder = modelToMap.SortOrder;
            modelToUpdate.SubType = modelToMap.SubType;
            modelToUpdate.Cost = modelToMap.Cost;
            modelToUpdate.Status = modelToMap.Status;
        }

        public static void Map(ref AuthorityDefinition modelToUpdate, AuthorityDefinition modelToMap)
        {
            if (modelToUpdate == null || modelToMap == null) { return; }
            modelToUpdate.Title = modelToMap.Title;
            modelToUpdate.Type = modelToMap.Type;
            modelToUpdate.Mode = modelToMap.Mode;
        }

        public static void Map(ref PosDefinition modelToUpdate, PosDefinition modelToMap)
        {
            if (modelToUpdate == null || modelToMap == null) { return; }
            modelToUpdate.Name = modelToMap.Name;
            modelToUpdate.TerminalID = modelToMap.TerminalID;
            modelToUpdate.UserID = modelToMap.UserID;
            modelToUpdate.Password = modelToMap.Password;
            modelToUpdate.MerchantID = modelToMap.MerchantID;
            modelToUpdate.SuccessUrl = modelToMap.SuccessUrl;
            modelToUpdate.ErrorUrl = modelToMap.ErrorUrl;
            modelToUpdate.RefundPassword = modelToMap.RefundPassword;
            modelToUpdate.RefundUserID = modelToMap.RefundUserID;
            modelToUpdate.IsTest = modelToMap.IsTest;
            modelToUpdate.PaymentMethod = modelToMap.PaymentMethod;
            modelToUpdate.StoreKey = modelToMap.StoreKey;
            modelToUpdate.PosType = modelToMap.PosType;
            modelToUpdate.PostUrl = modelToMap.PostUrl;
            modelToUpdate.XmlUrl = modelToMap.XmlUrl;
            modelToUpdate.ImageUrl = modelToMap.ImageUrl;
        }

        public static IEnumerable<Product> MapToProduct(IEnumerable<ElasticProduct> elasticProducts, IEnumerable<Store> stores)
        {
            if (elasticProducts != null && stores != null)
            {
                var products = elasticProducts.Select(i => new Product()
                {
                    ID = i.ID,
                    Name = i.Name,
                    ShortDescription = i.ShortDescription,
                    Description = i.Description,
                    CategoryID = i.CategoryID,
                    Barcode = i.Barcode,
                    Brand = i.Brand,
                    PropertyInfo = i.PropertyInfo,
                    ImageInfo = i.ImageInfo,
                    Status = i.Status,
                    DateModified = i.DateModified,
                    CommentInfo = string.Format("{0};{1}", i.Rating, i.CommentCount),
                    Items = i.Items.Select(item => new StoreItem()
                    {
                        ID = item.ID,
                        StoreID = item.StoreID,
                        ProductID = i.ID,
                        IsForSale = item.IsForSale,
                        HasVariant = item.HasVariant,
                        SalesPrice = item.SalesPrice,
                        ListPrice = item.ListPrice,
                        Stock = item.Stock,
                        Status = item.Status,
                        DateModified = item.DateModified,
                        Store = stores.FirstOrDefault(s => s.ID == item.StoreID)
                    })
                });
                return products;
            }
            return new List<Product>();
        }
    }
}