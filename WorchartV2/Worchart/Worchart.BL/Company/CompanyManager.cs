using System;
using Worchart.BL.Constants;
using Worchart.BL.Log;
using Worchart.BL.Manager;
using Worchart.BL.Model;
using Worchart.Data;

namespace Worchart.BL.Company
{
    public class CompanyManager : BaseManager<CompanyModel>, ICompanyManager
    {
        public CompanyManager(IRepository<CompanyModel> repository, ILogger logger) : base(repository, logger)
        {
        }

        protected override OperationResult OnBeforeUpdate(ref CompanyModel existing, CompanyModel model)
        {
            if (existing != null && model != null && model.IsValid())
            {
                existing.Name = model.Name;
                existing.WebAddress = model.WebAddress;
                existing.Status = model.Status;
                existing.DateCreated = DateTime.Now;
                return Success();
            }
            return Failed(ErrorConstants.CheckParameters);
        }
    }
}
