using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Common;
using Shared.Constants;
using Shared.DTOs;

namespace web2backend.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly IUserService _userService;
        public UserController(IUserService userService) 
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (ModelState.IsValid)
            {
                ResponsePackage<ProfileDTO> response = _userService.LoginUser(loginDTO);

                if (response.Status == ResponseStatus.OK)
                {
                    return Ok(response.Data);
                }
                else
                {
                    return Problem(response.Message, statusCode: (int)response.Status);
                }
            }
            else
            {
                return Problem("Entered values not valid", statusCode: (int)ResponseStatus.BadRequest);
            }
        }

        [HttpPost("signinBuyer")]
        public async Task<IActionResult> SigninBuyer([FromForm] UserDTO UserDTO, IFormFile? file = null)
        {
            if(ModelState.IsValid)
            {
                string filePath;
                if (file != null && file.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string uploadPath = "Avatars";
                    Console.WriteLine(uploadPath);
                    if(!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                else
                {
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "Avatars");
                    filePath = Path.Combine(filePath, "avatar.svg");
                }

                var task = await _userService.SigninUser(UserDTO, SD.Roles.Buyer, filePath);
                if(task.Status == ResponseStatus.OK)
                {
                    return Ok(task.Message);
                }
                else if (task.Status == ResponseStatus.InternalServerError)
                {
                    if(System.IO.File.Exists(filePath) && file != null)
                    {
                        System.IO.File.Delete(filePath);
                    }
                    return Problem(task.Message, statusCode: ((int)task.Status));
                }
                else
                {
                    return Problem(task.Message, statusCode: ((int)task.Status));
                }
            }
            return Problem("Entered values not valid", statusCode: (int)ResponseStatus.BadRequest);
        }

        [HttpPost("signinSeller")]
        public async Task<IActionResult> SigninSeller([FromForm] UserDTO UserDTO, IFormFile? file = null)
        {
            if (ModelState.IsValid)
            {
                string filePath;
                if (file != null && file.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string uploadPath = "Avatars";
                    Console.WriteLine(uploadPath);
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                else
                {
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "Avatars");
                    filePath = Path.Combine(filePath, "avatar.svg");
                }

                var task = await _userService.SigninUser(UserDTO, SD.Roles.Seller, filePath);
                if (task.Status == ResponseStatus.OK)
                {
                    return Ok(task.Message);
                }
                else if (task.Status == ResponseStatus.InternalServerError)
                {
                    if (System.IO.File.Exists(filePath) && file != null)
                    {
                        System.IO.File.Delete(filePath);
                    }
                    return Problem(task.Message, statusCode: ((int)task.Status));
                }
                else
                {
                    return Problem(task.Message, statusCode: ((int)task.Status));
                }
            }
            return Problem("Entered values not valid", statusCode: (int)ResponseStatus.BadRequest);
        }

        [HttpPost("updateUser")]
        public async Task<IActionResult> UpdateUser([FromForm] UserDTO userDTO, IFormFile? file = null)
        {
            if(ModelState.IsValid)
            {
                string filePath;
                string avatar = String.Empty;
                if(file != null && file.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string uploadPath = "Avatars";
                    Console.WriteLine(uploadPath);

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    filePath = Path.Combine(uploadPath, fileName);

                    using(var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                else
                {
                    filePath = String.Empty;
                }

                var task = _userService.UpdateProfile(userDTO, filePath);
                if(task.Status == ResponseStatus.OK)
                {
                    return Ok(task.Data);
                }
                else if(task.Status == ResponseStatus.InternalServerError)
                {
                    if(System.IO.File.Exists(filePath) && file != null) 
                    {
                        System.IO.File.Delete(filePath);
                    }
                    return Problem(task.Message, statusCode: ((int)task.Status));
                }
                else
                {
                    return Problem(task.Message, statusCode: ((int)task.Status));
                }
            }
            return Problem("Entered values not valid", statusCode: (int)ResponseStatus.BadRequest);
        }

        [HttpPost("registerAdmin")]
        public IActionResult RegisterAdmin(UserDTO userDTO)
        {
            if (ModelState.IsValid)
            {
                var task = _userService.RegisterAdmin(userDTO);
                if (task.Status == ResponseStatus.OK)
                    return Ok(task.Message);
                else if (task.Status == ResponseStatus.InvalidEmail)
                    ModelState.AddModelError("email", task.Message);
                else if (task.Status == ResponseStatus.InvalidUsername)
                    ModelState.AddModelError("username", task.Message);
                else if (task.Status == ResponseStatus.InternalServerError)
                    ModelState.AddModelError(String.Empty, task.Message);
                else
                    ModelState.AddModelError(String.Empty, task.Message);
            }
            return Problem();
        }

        [HttpGet("notVerified")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUnverified()
        {
            var result = _userService.GetVerified();
            if (result.Status == ResponseStatus.AllUsersVerified)
                return Ok(result.Message);
            else if (result.Status == ResponseStatus.OK)
                return Ok(result.Data);
            else
                return Problem(result.Message);
        }

        [HttpPost("verify")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Verify([FromBody]VerificationDTO user)
        {
            var result = await _userService.VerifyUser(user);
            if (result.Status == ResponseStatus.OK)
                return Ok(result.Message);
            else
                return Problem(detail: result.Message, statusCode: (int)result.Status);
        }

        [HttpPost("deny")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Deny([FromBody]VerificationDTO user)
        {
            var result = await _userService.DenyUser(user);
            if (result.Status == ResponseStatus.OK)
                return Ok(result.Message);
            else
                return Problem(detail: result.Message, statusCode: (int)result.Status);

        }
 
    }
}
