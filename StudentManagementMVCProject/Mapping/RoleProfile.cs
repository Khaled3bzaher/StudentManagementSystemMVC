using AutoMapper;
using Microsoft.AspNetCore.Identity;
using StudentManagementMVCProject.ViewModels.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagementMVCProject.Mapping
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<IdentityRole, GetRoleViewModel>();
            CreateMap<IdentityRole, GetRoleViewModel>().ReverseMap();
        }
    }
}
