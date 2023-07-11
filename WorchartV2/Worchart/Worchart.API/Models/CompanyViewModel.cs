using System;
using System.Collections.Generic;
using System.Linq;
using Worchart.BL.Company;
using Worchart.BL.Enum;
using Worchart.BL.Model;
using Worchart.BL.Project;

namespace Worchart.API.Models
{
    public class CompanyViewModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string CompanyCode { get; set; }

        public string Logo { get; set; }

        public string PhoneNumber { get; set; }

        public string WebAddress { get; set; }

        public string Address { get; set; }

        public DateTime DateCreated { get; set; }

        public StatusType Status { get; set; }

        public List<ProjectModel> Projects { get; set; }

        public List<CompanyTeamUserView> CrewList { get; set; }

        public List<CompanyTeamView> TeamList { get; set; }

        public void Initialize(CompanyModel company, List<Team> teamList, List<TeamUser> teamUsers, List<Item> teamRoleList)
        {
            if (company != null)
            {
                ID = company.ID;
                Address = company.Address;
                CompanyCode = company.CompanyCode;
                DateCreated = company.DateCreated;
                Logo = company.LogoPath;
                Name = company.Name;
                PhoneNumber = company.PhoneNumber;
                Status = company.Status;
                WebAddress = company.WebAddress;

                if (teamList != null && teamUsers != null && teamRoleList != null && CrewList != null)
                {
                    TeamList = teamList.Select(i => new CompanyTeamView()
                    {
                        ID = i.ID,
                        Name = i.Name,
                        CompanyID = i.CompanyID,
                        CreateDate = i.CreateDate,
                        CreatedBy = i.CreatedBy,
                        Description = i.Description,
                        Logo = i.Logo,
                        ShortName = i.ShortName,
                        Status = i.Status,
                        UserList = teamUsers.Where(tu => tu.TeamID == i.ID).Select(tu =>
                        {
                            var crew = CrewList?.FirstOrDefault(u => u.ID == tu.UserID);
                            var teamRole = teamRoleList.FirstOrDefault(tr => tr.ID == tu.RoleID);
                            var user = crew != null ? new CompanyTeamUserView()
                            {
                                ID = crew.ID,
                                Name = crew.Name,
                                Email = crew.Email,
                                Role = teamRole,
                                Status = tu.Status
                            } : null;
                            return user;
                        }).Where(tu => tu != null).ToList()
                    }).ToList();
                }
            }
        }
    }

    public class CompanyTeamView
    {
        public int ID { get; set; }

        public int CompanyID { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public string Description { get; set; }

        public string Logo { get; set; }

        public DateTime CreateDate { get; set; }

        public int CreatedBy { get; set; }

        public StatusType Status { get; set; }

        public List<CompanyTeamUserView> UserList { get; set; }
    }

    public class CompanyTeamUserView
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public Item Role { get; set; }

        public StatusType Status { get; set; }
    }
}
