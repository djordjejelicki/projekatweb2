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
using System.Net.Http.Json;
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
                newUser.IsVerified = false;
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

        public async Task<ResponsePackage<bool>> VerifyUser(VerificationDTO verificationDTO)
        {
            User u = _uow.User.GetFirstOrDefault(u => u.UserName == verificationDTO.UserName);
            if(u == null)
            {
                return new ResponsePackage<bool>(false, ResponseStatus.NotFound);
            }
            u.IsVerified = true;
            string emailContent = $"<p>Zdravo {u.FirstName} {u.LastName},</p>";
            emailContent += $"<p>Vas nalog je verifikovan. Zahvaljujemo vam se na strpljenju.</p>";

            try
            {
                _uow.User.Update(u);
                _uow.Save();

                var success = await _emailService.SendMailAsync(new EmailData()
                {
                    To = u.Email,
                    Content = emailContent,
                    IsContentHtml = true,
                    Subject = "Verifikacija naloga"

                });

                if (success)
                {
                    return new ResponsePackage<bool>(true, ResponseStatus.OK, "User verified succesfully");
                }
                else
                {
                    return new ResponsePackage<bool>(false, ResponseStatus.InternalServerError, "There was an error while veryfying user");
                }
            }
            catch(Exception ex)
            {
                return new ResponsePackage<bool>(false, ResponseStatus.InternalServerError, ex.Message);
            }
        }

        public async Task<ResponsePackage<bool>> DenyUser(VerificationDTO verificationDTO)
        {

            User u = _uow.User.GetFirstOrDefault(u => u.UserName == verificationDTO.UserName);
            if (u == null)
            {
                return new ResponsePackage<bool>(false, ResponseStatus.NotFound);
            }

            string emailContent = $"<p>Zdravo {u.FirstName} {u.LastName},</p>";
            emailContent += $"<p>Vas nalog je odbijen iz razloga {verificationDTO.Reason}. </p>";

            try
            {
                _uow.User.Remove(u);
                _uow.Save();

                var success = await _emailService.SendMailAsync(new EmailData()
                {
                    To = u.Email,
                    Content = emailContent,
                    IsContentHtml = true,
                    Subject = "Verifikacija naloga"
                });

                if (success)
                    return new ResponsePackage<bool>(true, ResponseStatus.OK, "User denied");
                else
                    return new ResponsePackage<bool>(false, ResponseStatus.InternalServerError, "There was an error");
            }
            catch (Exception ex)
            {
                return new ResponsePackage<bool>(false, ResponseStatus.InternalServerError, ex.Message);
            }
        }

        public ResponsePackage<List<ProfileDTO>> GetVerified()
        {
            List<User> notVerified = _uow.User.GetAll(u => !u.IsVerified).ToList();
            if (notVerified.Count == 0)
                return new ResponsePackage<List<ProfileDTO>>(null, ResponseStatus.AllUsersVerified, "All users are verified");
            else
            {
                List<ProfileDTO> response = new List<ProfileDTO>();
                foreach (var elem in notVerified)
                {
                    ProfileDTO retUser = _mapper.Map<ProfileDTO>(elem);
                    byte[] imageBytes = System.IO.File.ReadAllBytes(elem.ProfileUrl);
                    retUser.Avatar = Convert.ToBase64String(imageBytes);
                    response.Add(retUser);
                }
                return new ResponsePackage<List<ProfileDTO>>(response, ResponseStatus.OK);
            }
        }

        public ResponsePackage<bool> RegisterAdmin(UserDTO userDTO)
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
            newUser.Role = SD.Roles.Admin;
            newUser.IsVerified = true;

            newUser.ProfileUrl = Path.Combine(Directory.GetCurrentDirectory(), "Avatars");
            newUser.ProfileUrl = Path.Combine(newUser.ProfileUrl, "avatar.svg");
            try
            {
                _uow.User.Add(newUser);
                _uow.Save();
                return new ResponsePackage<bool>(true, ResponseStatus.OK, "User registered succesfully");
            }
            catch (Exception ex)
            {
                return new ResponsePackage<bool>(false, ResponseStatus.InternalServerError, ex.Message);
            }
        }

        public async Task<ResponsePackage<bool>> GoogleRegister(string accessToken, SD.Roles Role)
        {
            var httpClient = new HttpClient();
            var requestUrl = $"https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={accessToken}";
            try
            {
                var response = await httpClient.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Google login verification failed.");
                }
                var tokenInfo = await response.Content.ReadFromJsonAsync<GoogleTokenInfo>();

                if (MailExists(tokenInfo.Email))
                {
                    return new ResponsePackage<bool>(false, ResponseStatus.InvalidEmail, "Email already exists");
                }
                User newUser = new User
                {
                    Address = " ",
                    BirthDate = DateTime.UtcNow.AddYears(-18),
                    UserName = "user" + Guid.NewGuid(),
                    Email = tokenInfo.Email,
                    FirstName = tokenInfo.GivenName,
                    LastName = tokenInfo.FamilyName,
                };
                if ((tokenInfo.GivenName == null || tokenInfo.FamilyName == null) && tokenInfo.Name.Contains(' '))
                {
                    newUser.FirstName = tokenInfo.Name.Split(' ')[0];
                    newUser.LastName = tokenInfo.Name.Split(' ')[1];
                }
                else
                {
                    newUser.FirstName = tokenInfo.Name;
                    newUser.LastName = tokenInfo.Name;
                }

                byte[] salt = PasswordHasher.GenerateSalt();
                newUser.Salt = salt;
                newUser.Password = PasswordHasher.GenerateSaltedHash(Encoding.ASCII.GetBytes(Guid.NewGuid().ToString()), salt);

                string pictureUrl = tokenInfo.Picture;

                // Fetch the profile picture
                var pictureResponse = await httpClient.GetAsync(pictureUrl);

                if (pictureResponse.IsSuccessStatusCode)
                {
                    var pictureBytes = await pictureResponse.Content.ReadAsByteArrayAsync();
                    string extension = GetFileExtensionFromContentType(pictureResponse.Content.Headers.ContentType?.MediaType);

                    string fileName = Guid.NewGuid().ToString() + extension;
                    string filePath = Path.Combine("Avatars\\", fileName);

                    // Save the picture bytes to a file
                    File.WriteAllBytes(filePath, pictureBytes);

                    newUser.ProfileUrl = filePath;
                }
                else
                {
                    newUser.ProfileUrl = "\\Avatars\\avatar.svg";
                }

                string emailContent;
                if (Role == SD.Roles.Buyer)
                {
                    newUser.IsVerified = true;
                    newUser.Role = SD.Roles.Buyer;
                    emailContent = $"<p>Zdravo {newUser.FirstName} {newUser.LastName},</p>";
                    emailContent += $"<p>Vas nalog je uspešno napravljen. Zelimo vam srecnu kupovinu.</p>";
                }
                else
                {
                    newUser.Role = SD.Roles.Seller;
                    newUser.IsVerified = false;
                    emailContent = $"<p>Zdravo {newUser.FirstName} {newUser.LastName},</p>";
                    emailContent += $"<p>Vas nalog je uspešno napravljen. Molimo vas da sačekate da neko od naših administratora pregleda i odobri vaš profil.</p>";
                    emailContent += $"<p>Dobićete email obaveštenja kada nalog bude pregledan.</p>";
                }

                try
                {
                    var success = await _emailService.SendMailAsync(new EmailData()
                    {
                        To = newUser.Email,
                        Content = emailContent,
                        IsContentHtml = true,
                        Subject = "Aktivacija naloga"
                    });

                    if (success)
                    {
                        _uow.User.Add(newUser);
                        _uow.Save();
                        return new ResponsePackage<bool>(true, ResponseStatus.OK, "User registered succesfully");
                    }
                    else
                        return new ResponsePackage<bool>(false, ResponseStatus.InternalServerError, "There was an error while registering new user");
                }
                catch (Exception ex)
                {
                    return new ResponsePackage<bool>(false, ResponseStatus.InternalServerError, ex.Message);
                }
            }
            catch (Exception ex)
            {
                return new ResponsePackage<bool>(false, ResponseStatus.InternalServerError, ex.Message);
            }
        }

        public async Task<ResponsePackage<ProfileDTO>> GoogleLogin(string accessToken)
        {
            var httpClient = new HttpClient();
            var requestUrl = $"https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={accessToken}";

            try
            {
                var response = await httpClient.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Google login verification failed.");
                }
                var tokenInfo = await response.Content.ReadFromJsonAsync<GoogleTokenInfo>();

                User u = _uow.User.GetFirstOrDefault(u => u.Email == tokenInfo.Email);

                if (u != null)
                {
                    List<Claim> claims = new List<Claim>();
                    if (u.Role == SD.Roles.Buyer)
                        claims.Add(new Claim(ClaimTypes.Role, "Buyer"));
                    if (u.Role == SD.Roles.Seller)
                        claims.Add(new Claim(ClaimTypes.Role, "Seller"));
                    if (u.Role == SD.Roles.Admin)
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));

                    SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey.Value));
                    var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                    var tokeOptions = new JwtSecurityToken(
                        issuer: "http://localhost:5000", //url servera koji je izdao token
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(20),
                        signingCredentials: signinCredentials
                    );
                    string tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

                    ProfileDTO p = _mapper.Map<ProfileDTO>(u);
                    p.Token = tokenString;
                    p.Role = u.Role;

                    byte[] imageBytes = System.IO.File.ReadAllBytes(u.ProfileUrl);
                    p.Avatar = Convert.ToBase64String(imageBytes);

                    return new ResponsePackage<ProfileDTO>(p, ResponseStatus.OK, "Login successful");
                }
                else
                    throw new KeyNotFoundException("This user does not exist");

            }
            catch (KeyNotFoundException ex)
            {
                return new ResponsePackage<ProfileDTO>(null, ResponseStatus.NotFound, ex.Message);
            }
            catch (Exception ex)
            {
                return new ResponsePackage<ProfileDTO>(null, ResponseStatus.InternalServerError, ex.Message);
            }
        }

        private string GetFileExtensionFromContentType(string contentType)
        {
            switch (contentType)
            {
                case "image/jpeg":
                    return ".jpg";
                case "image/png":
                    return ".png";
                case "image/gif":
                    return ".gif";
                // Add more cases for other supported image formats if needed
                default:
                    return ".jpg"; // Default extension if the content type is unknown or not provided
            }
        }
    }
}
