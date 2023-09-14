using AutoMapper;
using BLL.Services.Interfaces;
using DAL.Model;
using DAL.Repository.IRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared;
using Shared.Common;
using Shared.Constants;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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

        public UserService(IUnitOfWork uow, IConfiguration configuration, IMapper mapper, IEmailService emailService)
        {
            _uow = uow;
            _configuration = configuration;
            _mapper = mapper;
            _secretKey = _configuration.GetSection("SecretKey");
            _emailService = emailService;
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
                if (u.Password.SequenceEqual(PasswordHasher.GenerateSaltedHash(Encoding.ASCII.GetBytes(loginDTO.Password), u.Salt)))
                {
                    List<Claim> claims = new List<Claim>();
                    if(u.Role == SD.Roles.Buyer)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "Buyer"));
                    }

                    if(u.Role == SD.Roles.Seller)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "Seller"));
                    }

                    if(u.Role == SD.Roles.Admin)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                    }

                    SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey.Value));
                    var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                    var tokenOptions = new JwtSecurityToken(
                        issuer: "http://localhost:5000",
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(20),
                        signingCredentials: signinCredentials);

                    
                   

                    string tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                   
                    ProfileDTO p = _mapper.Map<ProfileDTO>(u);
                    p.Token = tokenString;
                    p.Role = u.Role;
                 
                    byte[] imageBytes = System.IO.File.ReadAllBytes(u.ProfileUrl);
                    p.Avatar = Convert.ToBase64String(imageBytes);

                    return new ResponsePackage<ProfileDTO>(p, ResponseStatus.OK, "Login successfull");
                }
                else
                {
                    return new ResponsePackage<ProfileDTO>(null, ResponseStatus.NotFound, "Wrong password");
                }
            }
            else
            {
                return new ResponsePackage<ProfileDTO>(null, ResponseStatus.NotFound, "This user does not exist");
            }
        }

        public ResponsePackage<ProfileDTO> UpdateProfile(UserDTO userDTO, string file)
        {
            User u = _uow.User.GetFirstOrDefault(u => u.Email == userDTO.Email);
            string avatar = String.Empty;
            if (file != String.Empty)
            {
                avatar = u.ProfileUrl;
                u.ProfileUrl = file;
            }
            u.FirstName = userDTO.FirstName;
            u.LastName = userDTO.LastName;
            u.BirthDate = userDTO.BirthDate;
            u.Address = userDTO.Address;

            try
            {
                _uow.User.Update(u);
                _uow.Save();

                ProfileDTO p = _mapper.Map<ProfileDTO>(u);
                p.Role = u.Role;
                byte[] imageBytes = System.IO.File.ReadAllBytes(u.ProfileUrl);
                p.Avatar = Convert.ToBase64String(imageBytes); 
                if(avatar != String.Empty)
                {
                    if (avatar.Split('\\')[1] != "avatar.svg")
                    {
                        System.IO.File.Delete(avatar);
                    }
                }
                return new ResponsePackage<ProfileDTO>(p, ResponseStatus.OK, "Profile changed");
            }
            catch(Exception ex)
            {
                return new ResponsePackage<ProfileDTO>(null, ResponseStatus.InternalServerError, "There was an error");
            }

        }
    }
}
