using BLL.Services.Interfaces;
using Shared;
using Shared.Common;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations
{
    public class UserService : IUserService
    {
        public Task<ResponsePackage<bool>> SigninUser(UserDTO userDTO, SD.Roles role, string file)
        {
            throw new NotImplementedException();
        }
    }
}
