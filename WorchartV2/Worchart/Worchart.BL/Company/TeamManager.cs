using System;
using Worchart.BL.Constants;
using Worchart.BL.Log;
using Worchart.BL.Manager;
using Worchart.BL.Model;
using Worchart.Data;

namespace Worchart.BL.Company
{
    public class TeamManager : BaseManager<Team>, ITeamManager
    {
        public TeamManager(IRepository<Team> repository, ILogger logger) : base(repository, logger)
        {
        }

        protected override OperationResult OnBeforeCreate(ref Team model)
        {
            if (model != null && model.IsValid())
            {
                model.Status = Enum.StatusType.Active;
                model.CreateDate = DateTime.Now;
                return Success();
            }
            return Failed(ErrorConstants.CheckParameters);
        }

        protected override OperationResult OnBeforeUpdate(ref Team existing, Team model)
        {
            if (existing != null && model != null && model.IsValid())
            {
                existing.Name = model.Name;
                existing.ShortName = model.ShortName;
                existing.Description = model.Description;
                existing.Logo = model.Logo;
                existing.Status = model.Status;
                return Success();
            }
            return Failed(ErrorConstants.CheckParameters);
        }
    }
}
