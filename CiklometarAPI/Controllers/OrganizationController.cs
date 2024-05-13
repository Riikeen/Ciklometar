using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CiklometarBLL;
using CiklometarBLL.DTO;
using CiklometarBLL.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;

namespace CiklometarAPI.Controllers
{   
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _service;
        private readonly IUserManager _checkUserService;
        private readonly UserFactory _userFactory;

        public OrganizationController(IOrganizationService service, UserFactory userFactory ,IUserManager checkUserService)
        {
            _service = service;
            _userFactory = userFactory;
            _checkUserService = checkUserService;
        }
        
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public IEnumerable<OrganizationDTO> GetAllOrganizations()
        {
            return _service.GetAllOrganizations().ToList();
        }

        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public ActionResult<OrganizationDTO> GetOrganizationById(Guid id)
        {
            return _service.GetById(id);
        }

        [HttpGet]
        [Route("{id}/Profile/{start}/{end}")]
        [AllowAnonymous]
        public ActionResult<OrganizationProfileDTO> GetOrganizationProfile(Guid id, double start, double end)
        {
            var startDate = (new DateTime(1970, 1, 1)).AddMilliseconds(start);
            var endDate = (new DateTime(1970, 1, 1)).AddMilliseconds(end);
            return _service.GetProfileData(id, HttpContext.GetUserId(), startDate, endDate);
        }

        [HttpGet]
        [Route("{id}/Users")]
        [AllowAnonymous]
        public IEnumerable<UserCyclistDTO> GetUsersInOrganization(Guid id)
        {
            return _service.GetAllUsersInOrganization(id);
        }

        [HttpGet]
        [Route("AvailableToJoin/")]
        public IEnumerable<OrganizationDTO> GetOrganizationsAvailableToJoin()
        {
            return _service.GetOrganizationsAvailableToJoin(_userFactory.GetUserId());
        }

        [HttpGet]
        [Route("AdminOf/")]
        public IEnumerable<OrganizationDTO> GetOrganizationsUserIsAdminOf()
        {
            return _service.GetOrganizationsAdminOf(_userFactory.GetUserId());
        }

        [HttpGet]
        [Route("{id}/Location")]
        public IEnumerable<LocationResponseDTO> GetLocationByOrganizationId(Guid id)
        {
            //var userIsAdminInOrg = _checkUserService.UserIsAdminInOrganization(id, HttpContext.GetUserId());
            //if (!userIsAdminInOrg)
            //{
            //    throw new Exception("Not an admin: " + HttpContext.GetUserId() + " of: " + id);
            //}
            return _service.GetLocationsByOrganizationId(id);
        }

        [HttpPost]
        [Route("")]
        public void AddOrganization([FromBody]AddOrganizationDTO organization)
        {
            //var userIsSuperadmin = _checkUserService.UserIsSuperadmin(HttpContext.GetUserId());
            //if (!userIsSuperadmin)
            //{
            //    throw new Exception("Not a superadmin.");
            //}
            _service.AddOrganization(organization, _userFactory.GetUserId());
        }

        [HttpDelete]
        [Route("{id}")]
        public void DeleteOrganization(Guid id)
        {
            //var userIsSuperadmin = _checkUserService.UserIsSuperadmin(HttpContext.GetUserId());
            //if (!userIsSuperadmin)
            //{
            //    throw new Exception("Not a superadmin.");
            //}
            _service.DeleteOrganization(id);
        }

        [HttpPut]
        [Route("")]
        public void UpdateOrganization([FromBody]OrganizationDTO organization)
        {
            //var userIsAdminInOrg = _checkUserService.UserIsAdminInOrganization(organization.Id, HttpContext.GetUserId());
            //if (!userIsAdminInOrg)
            //{
            //    throw new Exception("Not an admin.");
            //}
            _service.UpdateOrganization(organization);
        }

        [HttpPost]
        [Route("{id}/Location")]
        public void AddListOfLocations(Guid id, List<LocationDTO>locations)
        {
            //var userIsAdminInOrg = _checkUserService.UserIsAdminInOrganization(id, HttpContext.GetUserId());
            //if (!userIsAdminInOrg)
            //{
            //    throw new Exception("Not an admin.");
            //}
            _service.UpdateOrganizationLocations(id, locations);
        }

        [HttpPost]
        [Route("{orgId}/Ban/{timeInMiliseconds}")]
        public void AddOrganization(Guid orgId, long timeInMiliseconds, [FromBody] List<Guid> userIds)
        {
            _service.BanUsersFromOrganization(userIds, orgId, timeInMiliseconds);
        }

        [HttpGet]
        [Route("{organizationId}/activites")]
        public List<ActivitesInOrganizationDTO> GetActivitiesInOrganization(Guid organizationId)
        {
            return _service.GetAtivitiesInOrganization(organizationId);
        }
    }
}
