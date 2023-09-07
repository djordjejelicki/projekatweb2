using Microsoft.AspNetCore.Mvc;

namespace web2backend.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
