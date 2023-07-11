using Aware.Data;
using Aware.Model;
using Aware.Util.Constants;
using Aware.Util.Log;
using System;
using Aware.BL.Model;
using Aware.Util.Enum;
using Aware.Util.Cache;

namespace Aware.Manager
{
    public class ApplicationManager : BaseManager<ApplicationModel>, IApplicationManager
    {
        public ApplicationManager(IRepository<ApplicationModel> repository, IAwareLogger logger, IAwareCacher cacher) : base(repository, logger, cacher)
        {
        }

        protected override OperationResult<ApplicationModel> OnBeforeUpdate(ref ApplicationModel existing, ApplicationModel model)
        {
            if (existing != null && model != null && model.IsValid())
            {
                existing.Name = model.Name;
                existing.AllowedIps = model.AllowedIps;
                existing.Status = model.Status;
                existing.DateModified = DateTime.Now;
                return OperationResult<ApplicationModel>.Success(existing);
            }
            return OperationResult<ApplicationModel>.Error(ResultCodes.Error.CheckParameters);
        }

        protected override ManagerCacheMode CacheMode => ManagerCacheMode.UseResponsiveCache;
    }
}
