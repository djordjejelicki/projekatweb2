using DAL.Model;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Mapping
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile() 
        { 
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, ProfileDTO>().ForMember(x => x.Token, opt => opt.Ignore()).ForMember(x => x.Role, opt => opt.Ignore()).ForMember(x => x.Avatar, opt => opt.Ignore()).ReverseMap();
        }
    }
}
