using System.Collections.Generic;
using Worchart.BL.Manager;
using Worchart.BL.Model;

namespace Worchart.BL.Project
{
    public interface IProjectManager : IBaseManager<ProjectModel>
    {
        OperationResult CreateNewProject(ProjectModel model, List<ProjectRequirementModel> requirements);
    }
}
