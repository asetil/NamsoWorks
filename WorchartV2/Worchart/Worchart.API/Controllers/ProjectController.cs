using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Worchart.API.Models;
using Worchart.BL.Model;
using Worchart.BL.Project;
using Worchart.BL.Search;

namespace Worchart.API.Controllers
{
    [Authorize(Policy = "Customer")]
    public class ProjectController : BaseController
    {
        private readonly IProjectManager _projectManager;
        public ProjectController(IProjectManager projectManager)
        {
            _projectManager = projectManager;
        }

        [HttpPost("search")]
        public ActionResult<SearchResult<ProjectModel>> GetProjects()
        {
            var searchParams = new SearchParams<ProjectModel>();
            searchParams.AddFilter(f => f.CreatedBy == CustomerID)
                .SortBy(o => o.DateCreated, true)
                .SetPaging(1, 20)
                .WithCount();

            return _projectManager.Search(searchParams);
        }

        [HttpPost("detail")]
        public ActionResult<ProjectModel> GetDetail(int projectID)
        {
            return _projectManager.Get(projectID);
        }

        [HttpPost("create")]
        public ActionResult<OperationResult> Create(ProjectCreateModel model)
        {
            var result = Validate();
            if (result.Success)
            {
                if (model != null && model.Project != null)
                {
                    model.Project.CreatedBy = CustomerID;
                    model.Project.FirmID = Customer.CompanyID;
                }
                result = _projectManager.CreateNewProject(model.Project, model.Requirements);
            }
            return result;
        }
    }
}
