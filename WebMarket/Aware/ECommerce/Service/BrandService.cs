using System;
using System.Collections.Generic;
using Aware.Data;
using Aware.ECommerce.Interface;
using Aware.ECommerce.Model;
using Aware.ECommerce.Util;
using Aware.Util;
using Aware.Util.Enums;
using Aware.Util.Log;

namespace Aware.ECommerce.Service
{
    public class BrandService : BaseService<Brand>, IBrandService
    {
        public BrandService(IRepository<Brand> repository,ILogger logger):base(repository,logger)
        {
        }

        public List<Brand> GetBrands(int page, int pageSize = 25)
        {
            try
            {
                return Repository.Where(i => i.ID > 0).SetPaging(page, pageSize).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error("BrandService > GetBrands - failed", ex);
            }
            return new List<Brand>();
        }

        public void RefreshProductBrand(string newName, string oldName)
        {
            try
            {
                if (!string.IsNullOrEmpty(oldName))
                {
                    var refreshSp = SqlHelper.RefreshProductBrand(newName,oldName);
                    Repository.ExecuteSp(refreshSp);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("BrandService > RefreshProductBrand - Failed for newName:{0}, oldName:{1}", ex, newName,oldName);
            }
        }

        protected override void OnBeforeUpdate(ref Brand existing, Brand model)
        {
            if (existing != null && model != null)
            {
                existing.Name = model.Name;
                existing.ImagePath = model.ImagePath;
                existing.Description = model.Description;
                existing.Status = Statuses.Active;
            }
        }

       
    }
}