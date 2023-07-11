using System;
using Worchart.BL.Constants;
using Worchart.BL.Log;
using Worchart.BL.Manager;
using Worchart.BL.Model;
using Worchart.Data;

namespace Worchart.BL.Company
{
    public class TeamUserManager : BaseManager<TeamUser>, ITeamUserManager
    {
        public TeamUserManager(IRepository<TeamUser> repository, ILogger logger) : base(repository, logger)
        {
        }

        protected override OperationResult OnBeforeCreate(ref TeamUser model)
        {
            if (model != null && model.IsValid())
            {
                model.Status = Enum.StatusType.Active;
                model.CreateDate = DateTime.Now;
                return Success();
            }
            return Failed(ErrorConstants.CheckParameters);
        }

        protected override OperationResult OnBeforeUpdate(ref TeamUser existing, TeamUser model)
        {
            if (existing != null && model != null && model.IsValid())
            {
                existing.UserID = model.UserID;
                existing.TeamID = model.TeamID;
                existing.RoleID = model.RoleID;
                existing.Status = model.Status;
                return Success();
            }
            return Failed(ErrorConstants.CheckParameters);
        }
    }
}
