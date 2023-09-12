using AutoMapper;
using BLL.Services.Interfaces;
using DAL.Model;
using DAL.Repository.IRepository;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.Common;
using Shared.Constants;
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
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _configuration;
        private readonly IConfigurationSection _secretKey;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public UserService(IUnitOfWork uow, IConfiguration configuration, IMapper mapper)
        {
            _uow = uow;
            _configuration = configuration;
            _mapper = mapper;
            _secretKey = _configuration.GetSection("SecretKey");
        }
        
        private bool MailExists(string email)
        {
            User u = _uow.User.GetFirstOrDefault(u => u.Email == email);
            if(u != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool UsernameExists(string username)
        {
            User u = _uow.User.GetFirstOrDefault(u => u.UserName == username);
            if(u != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<ResponsePackage<bool>> SigninUser(UserDTO userDTO, SD.Roles role, string file)
        {
            if (MailExists(userDTO.Email))
            {
                return new ResponsePackage<bool>(false, ResponseStatus.InvalidEmail, "Email already exists");
            }

            if (UsernameExists(userDTO.UserName))
            {
                return new ResponsePackage<bool>(false, ResponseStatus.InvalidUsername, "Username already exists");
            }

            User newUser = _mapper.Map<User>(userDTO);

            byte[] salt = PasswordHasher.GenerateSalt();
            newUser.Salt = salt;
            newUser.Password = PasswordHasher.GenerateSaltedHash(Encoding.ASCII.GetBytes(userDTO.Password), salt);
            newUser.ProfileUrl = file;
            string emailContent;
            if(role == SD.Roles.Buyer)
            {
                newUser.IsVerified = true;
                newUser.Role = SD.Roles.Buyer;
                emailContent = $"<p>Hi! {newUser.FirstName} {newUser.LastName} ,</p>";
                emailContent += $"<p>Your account is succsesfully created! have a nice time </p>";

            }
            else
            {
                newUser.IsVerified = true;
                newUser.Role = SD.Roles.Seller;
                emailContent = $"<p>Hi! {newUser.FirstName} {newUser.LastName} ,</p>";
                emailContent += $"<p>Your account is succsesfully created! please wait until our administrators check and approve your profile.</p>";
                emailContent += $"<p>You will receive email when your account is checked</p>";
            }

            try
            {
                var success = await _emailService.SendMailAsync(new EmailData()
                {
                    To = newUser.Email,
                    Content = emailContent,
                    IsContentHtml = true,
                    Subject = "Account activation"
                });

                if (success)
                {
                    _uow.User.Add(newUser);
                    _uow.Save();
                    return new ResponsePackage<bool>(true, ResponseStatus.OK, "User signin succesfully");
                }
                else
                {
                    return new ResponsePackage<bool>(false, ResponseStatus.InternalServerError, "There was an error while registiring new user");
                }
            }
            catch (Exception ex) 
            {
                return new ResponsePackage<bool>(false, ResponseStatus.InternalServerError, ex.Message);
            }
        }

        public ResponsePackage<ProfileDTO> LoginUser(LoginDTO loginDTO)
        {
            User u = _uow.User.GetFirstOrDefault(u => u.Email == loginDTO.Email);

            if(u != null)
            {

            }
        }
    }
}
