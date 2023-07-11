using System;
using Worchart.BL.Constants;
using Worchart.BL.Log;
using Worchart.BL.Manager;
using Worchart.BL.Model;
using Worchart.Data;

namespace Worchart.BL.Token
{
    public class ApplicationManager : BaseManager<ApplicationModel>, IApplicationManager
    {
        public ApplicationManager(IRepository<ApplicationModel> repository, ILogger logger) : base(repository, logger)
        {

        }

        protected override OperationResult OnBeforeUpdate(ref ApplicationModel existing, ApplicationModel model)
        {
            if (existing != null && model != null && model.IsValid())
            {
                existing.Name = model.Name;
                existing.AllowedIps = model.AllowedIps;
                existing.Status = model.Status;
                existing.DateModified = DateTime.Now;
                return Success();
            }
            return Failed(ErrorConstants.CheckParameters);
        }
    }
}
