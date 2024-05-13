 using System;
using System.Collections.Generic;
using System.Linq;
using CiklometarBLL;
using CiklometarBLL.DTO;
using CiklometarBLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CiklometarAPI.Controllers
{   [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IUserManager _checkUserService;

        public UserController(IUserService service, IUserManager checkUserService)
        {
            _service = service;
            _checkUserService = checkUserService;
        }

        [HttpGet]
        [Route("")]
        public IEnumerable<UserDTO> GetAllUsers()
        {
            var userIsAdminInOrg = _checkUserService.IsUserAdminInAnyOrganization(HttpContext.GetUserId());
            if (!userIsAdminInOrg)
            {
                throw new Exception("Not an admin.");
            }
            return _service.GetAllUsers().ToList();
        }

        [HttpGet]
        [Route("Cyclists")]
        [AllowAnonymous]
        public IEnumerable<UserCyclistDTO> GetCyclists()
        {
            return _service.GetAllCyclists().ToList();
        }

        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public ActionResult<UserDTO> GetUserById(Guid id)
        {
            return _service.GetById(id);
        }

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        public void AddUser([FromBody] UserDTO user)
        {
            _service.AddUser(user);
        }

        [HttpGet]
        [Route("{id}/Profile/{start}/{end}")]
        public ActionResult<UserProfileDTO> GetUserProfile(Guid id, long start, long end)
        {
            var startDate = (new DateTime(1970, 1, 1)).AddMilliseconds(start);
            var endDate = (new DateTime(1970, 1, 1)).AddMilliseconds(end);
            return _service.GetProfileData(id, startDate, endDate);
        }

        [HttpDelete]
        [Route("{id}")]
        public void DeleteUser(Guid id)
        {
            //var userIsSuperadmin = _checkUserService.UserIsSuperadmin(HttpContext.GetUserId());
            //if (!userIsSuperadmin)
            //{
            //    throw new Exception("Not a superadmin.");
            //}
            _service.DeleteUser(id);
        }
        [HttpPut]
        [Route("")]
        public void UpdateUser([FromBody]UserUpdateDTO user)
        {
            _service.UpdateUser(HttpContext.GetUserId(), user);
        }

        [HttpPut]
        [Route("{id}")]
        public void UpdateAnotherUser(Guid id, [FromBody] UserUpdateDTO user)
        {
            if (!_checkUserService.CanAccessUserInfo(id, HttpContext.GetUserId()))
            {
                throw new Exception("Not authorized.");
            }

            _service.UpdateUser(id, user);
        }

        [HttpGet]
        [Route("{id}/Organizations")]
        public IEnumerable<OrganizationDTO> GetAllOrganizationsByUserId(Guid id)
        {
            if (!_checkUserService.CanAccessUserInfo(id, HttpContext.GetUserId()))
            {
                throw new Exception("Not authorized.");
            }

            return _service.GetAllOrgsByUserId(id);
        }
    }
}
