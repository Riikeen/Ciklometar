using CiklometarBLL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CiklometarAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IUserManager _checkUserService;

        public HomeController(IUserManager checkUserService)
        {
            _checkUserService = checkUserService;
        }


    }
}
