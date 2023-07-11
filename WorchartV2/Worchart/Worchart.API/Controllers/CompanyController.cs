using Microsoft.AspNetCore.Mvc;
using Worchart.API.Models;
using Worchart.BL.Company;
using Worchart.BL.Model;
using Worchart.BL.Project;
using Worchart.BL.User;
using Microsoft.AspNetCore.Authorization;
using Worchart.BL.Lookup;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Diagnostics;

namespace Worchart.API.Controllers
{
    [Authorize(Policy = "Company")]
    public class CompanyController : BaseController
    {
        private readonly ICompanyManager _companyManager;
        private readonly IProjectManager _projectManager;
        private readonly IUserManager _userManager;
        private readonly ITeamManager _teamManager;
        private readonly ITeamUserManager _teamUserManager;
        private readonly ILookupManager _lookupManager;

        public CompanyController(ICompanyManager companyManager, IProjectManager projectManager, IUserManager userManager, ITeamManager teamManager, ITeamUserManager teamUserManager, ILookupManager lookupManager)
        {
            _companyManager = companyManager;
            _projectManager = projectManager;
            _userManager = userManager;
            _teamManager = teamManager;
            _teamUserManager = teamUserManager;
            _lookupManager = lookupManager;
        }

        [HttpGet("detail/{companyID}")]
        public ActionResult<OperationResult> Detail(int companyID)
        {
            var company = _companyManager.Get(companyID);
            var model = new CompanyViewModel();

            if (company != null)
            {
                model.Projects = _projectManager.SearchBy(i => i.FirmID == companyID)?.ToList();
                model.CrewList = _userManager.SearchBy(i => i.CompanyID == companyID)?.ToList().Select(crew => new CompanyTeamUserView()
                {
                    ID = crew.ID,
                    Name = crew.Name,
                    Email = crew.Email,
                }).ToList();

                var teamList = _teamManager.SearchBy(i => i.CompanyID == companyID)?.ToList();
                var teamIds = teamList.Select(t => t.ID);
                var teamUsers = _teamUserManager.SearchBy(i => teamIds.Contains(i.TeamID))?.ToList();
                var teamRoleList = _lookupManager.GetLookupItems(LookupType.TeamRoles, UserLanguage);

                model.Initialize(company, teamList, teamUsers, teamRoleList);
            }
            return Success(model);
        }

        [HttpPost("team")]
        public ActionResult<OperationResult> SaveTeam(Team team, IFormFile logo)
        {
            if (team != null)
            {
                var oldPath = team.Logo;
                team.CreatedBy = CustomerID;
                team.Logo = GetFilePath(logo, "team", team.Logo);

                var result = SaveFile(logo, team.Logo);
                if (result.Success)
                {
                    result = _teamManager.Save(team);
                    if (result.Success && team.Logo != oldPath)
                    {
                        RemoveFile(oldPath);
                    }
                }
                return result;
            }
            return Failed();
        }

        [HttpPost("teamuser")]
        public ActionResult<OperationResult> SaveTeamUser(TeamUser model)
        {
            if (model != null)
            {
                model.CreatedBy = CustomerID;
                return _teamUserManager.Save(model);
            }
            return Failed();
        }
    }
}