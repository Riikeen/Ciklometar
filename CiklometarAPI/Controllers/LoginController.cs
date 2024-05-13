using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CiklometarBLL;
using CiklometarBLL.DTO;
using CiklometarBLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace CiklometarAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _service;
        private readonly IStravaService _stravaService;
        public LoginController(ILoginService service, IStravaService stravaService)
        {
            _service = service;
            _stravaService = stravaService;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] UserLoginDTO login)
        {
            IActionResult response = Unauthorized();
            var user = _service.AuthenticateUser(login);

            if (user != null)
            {
                var tokenString = _service.GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Refresh/{refreshToken}")]
        public IActionResult RefreshAccessToken(string refreshToken)
        {
            IActionResult response = Unauthorized();
            if (refreshToken != null)
            {
                var tokenString = _service.RefreshAccessToken(refreshToken);
                response = Ok(new { token = tokenString });
            }
            return response;
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register([FromBody] UserRegisterDTO user)
        {
            try
            {
                _service.CreateUser(user);
                return Ok();
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("WithStrava/Register/{code}")]
        public async Task<IActionResult> RegisterWithStrava(string code)
        {
            IActionResult response = Unauthorized();
            var user = await _stravaService.RegisterWithStrava(code);

            if (user != null)
            {
                var tokenString = _service.GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        [AllowAnonymous]
        [HttpPost("WithStrava/Login/{code}")]
        public async Task<IActionResult> LoginWithStrava(string code)
        {
            IActionResult response = Unauthorized();
            var user = await _stravaService.LoginWithStrava(code);

            if (user != null)
            {
                var tokenString = _service.GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        [HttpPost("WithStrava/Connect/{code}/{userId}")]
        public async Task<IActionResult> ConnectWithStrava(string code, Guid userId)
        {
            if (userId != HttpContext.GetUserId())
            {
                throw new Exception("You cannot add requests for other users.");
            }

            IActionResult response = Unauthorized();
            var user = await this._stravaService.ConnectWithStrava(code, userId);

            if (user != null)
            {
                var tokenString = _service.GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        [HttpPost("Credentials")]
        public IActionResult ChangeCredentials([FromBody] UserSetCredentialsDTO user)
        {
            try
            {
                _service.SetCredentials(user, HttpContext.GetUserId());
                return Ok();
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
