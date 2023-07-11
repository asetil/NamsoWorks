using System.Collections.Generic;
using Worchart.BL.Project;

namespace Worchart.API.Models
{
    public class ProjectCreateModel
    {
        public ProjectModel Project { get; set; }
        public List<ProjectRequirementModel> Requirements { get; set; }
    }
}
