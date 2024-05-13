using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CiklometarBLL;
using CiklometarBLL.DTO;
using CiklometarBLL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CiklometarAPI.Controllers
{   // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly IRequestService _service;
        private readonly IUserManager _checkUserService;

        public RequestController (IRequestService service, IUserManager checkUserService)
        {
            _service = service;
            _checkUserService = checkUserService;
        }

        [HttpGet]
        [Route("")]
        public IEnumerable<RequestsGetDTO> GetAllRequests()
        {
            //var userIsAdminInOrg = _checkUserService.UserIsSuperadmin(HttpContext.GetUserId());
            //if (!userIsAdminInOrg)
            //{
            //    throw new Exception("Not a superadmin.");
            //}
            return _service.GetAllRequests();
        }
        [HttpGet]
        [Route("{id}")]
        public IEnumerable<RequestsGetDTO> GetRelevantRequests(Guid id)
        {
            if (!_checkUserService.UserCanGetRelevantRequests(id, HttpContext.GetUserId()))
            {
                throw new Exception("Not authorized.");
            }

            return _service.GetRelevantRequests(id);
        }

        [HttpPost]
        [Route("{userId}")]
        public void AddRequest(Guid userId, [FromBody]List<Guid>orgIds)
        {
            //if(userId != HttpContext.GetUserId())
            //{
            //    throw new Exception("You cannot add requests for other users.");
            //}
            _service.AddRequest(userId, orgIds);
        }

        [HttpPost]
        [Route("Resolve")]
        public void HandleRequest(List<RequestIdDTO> requests)
        {
            _service.HandleRequest(requests);
        }
    }
}
