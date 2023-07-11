using System;
using System.Collections.Generic;
using System.Linq;
using Worchart.BL.Constants;
using Worchart.BL.Log;
using Worchart.BL.Manager;
using Worchart.BL.Model;
using Worchart.Data;

namespace Worchart.BL.Project
{
    public class ProjectManager : BaseManager<ProjectModel>, IProjectManager
    {
        private readonly IRepository<ProjectRequirementModel> _requirementRepository;
        public ProjectManager(IRepository<ProjectModel> repository, IRepository<ProjectRequirementModel> requirementRepository, ILogger logger) : base(repository, logger)
        {
            _requirementRepository = requirementRepository;
        }

        public OperationResult CreateNewProject(ProjectModel model, List<ProjectRequirementModel> requirements)
        {
            try
            {
                var result = Save(model);
                if (result.Success && requirements != null && requirements.Any())
                {
                    var projectID = result.ValueAs<ProjectModel>().ID;
                    foreach (var requirement in requirements)
                    {
                        requirement.ProjectID = projectID;
                        requirement.DateCreated = DateTime.Now;
                        requirement.DateModified = DateTime.Now;
                        requirement.Status = Enum.ProjectRequirementStatus.Created;
                        _requirementRepository.Add(requirement);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("ProjectManager|CreateNewProject", ex);
            }
            return Failed();
        }

        protected override OperationResult OnBeforeCreate(ref ProjectModel model)
        {
            if (model != null && model.IsValid())
            {
                model.DateCreated = DateTime.Now;
                model.DateModified = DateTime.Now;
                model.Reference = string.Format("REF-{0}{1}", model.CreatedBy.ToString().PadLeft(5, '0'), model.DateCreated.ToString("YYYYMMDDHH"));
                model.Status = Enum.StatusType.Active;
                return Success();
            }
            return Failed(ErrorConstants.CheckParameters);
        }

        protected override OperationResult OnBeforeUpdate(ref ProjectModel existing, ProjectModel model)
        {
            if (existing != null && model != null && model.IsValid())
            {
                existing.Name = model.Name;
                existing.Description = model.Description;
                existing.Status = model.Status;
                existing.DateModified = DateTime.Now;
                return Success();
            }
            return Failed(ErrorConstants.CheckParameters);
        }
    }
}
