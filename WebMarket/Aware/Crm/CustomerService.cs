using System;
using System.Linq;
using System.Web.Mvc;
using Aware.Authenticate;
using Aware.Crm.Model;
using Aware.Data;
using Aware.Dependency;
using Aware.ECommerce.Interface;
using Aware.Util;
using Aware.Util.Log;
using Aware.Util.Model;

namespace Aware.Crm
{
    public class CustomerService : BaseService<Customer>, ICustomerService
    {
        public CustomerService(IRepository<Customer> repository, ILogger logger) : base(repository, logger)
        {
        }

        protected override void OnBeforeUpdate(ref Customer existing, Customer model)
        {
            if (existing != null && model != null)
            {
                existing.Name = model.Name;
                existing.Status = model.Status;
            }
        }

        protected override void OnBeforeCreate(ref Customer model)
        {
            if (model != null)
            {
                model.DateCreated = DateTime.Now;
            }
        }

        protected override Result OnBeforeDelete(int id)
        {
            if (id > 0)
            {
                var userService = WindsorBootstrapper.Resolve<IUserService>();
                var userList = userService.GetCustomerUsers(id);
                if (userList != null && userList.Any())
                {
                    return Result.Error("Firmanın tanımlı kullanıcıları olduğu için firma silinemez!");
                }

                var storeService = WindsorBootstrapper.Resolve<IStoreService>();
                var storeList = storeService.GetCustomerStores(id);
                if (storeList != null && storeList.Any())
                {
                    return Result.Error("Firmanın tanımlı marketleri olduğu için firma silinemez!");
                }
                return Result.Success();
            }
            return Result.Error("Firma ID bilgisi geçersiz!");
        }
    }
}
