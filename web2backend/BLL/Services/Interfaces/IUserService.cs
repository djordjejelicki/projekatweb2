using Shared;
using Shared.Common;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IUserService
    {
        Task<ResponsePackage<bool>> SigninUser(UserDTO userDTO,SD.Roles role, string file);
        ResponsePackage<ProfileDTO> LoginUser(LoginDTO loginDTO);
        ResponsePackage<ProfileDTO> UpdateProfile(UserDTO userDTO, string file);
        Task<ResponsePackage<bool>> VerifyUser(VerificationDTO verificationDTO);
        Task<ResponsePackage<bool>> DenyUser(VerificationDTO verificationDTO);
        ResponsePackage<List<ProfileDTO>> GetVerified();
        ResponsePackage<bool> RegisterAdmin(UserDTO userDTO);
        Task<ResponsePackage<bool>> GoogleRegister(string accessToken, SD.Roles Role);
        Task<ResponsePackage<ProfileDTO>> GoogleLogin(string accessToken);
    }
}
