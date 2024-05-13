using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CiklometarBLL.DTO;
using CiklometarBLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CiklometarAPI.Controllers
{   // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _service;

        public RoleController(IRoleService service)
        {
            _service = service;
        }
        [Authorize(Roles = "Admin, Cyclist")]
        [HttpGet]
        [Route("")]
        public IEnumerable<RoleDTO> GetAllRelationships()
        {
            return _service.GetAllRelationships().ToList();
        }

        [HttpPost]
        [Route("")]
        public void AddRelationship([FromBody] RoleDTO role)
        {
            _service.AddRelationship(role);
        }

        [HttpPut]
        [Route("")]
        public IActionResult UpdateRelationships(Guid Orgid, [FromBody] List<Guid> userIds)
        {
            _service.UpdateRelationships(Orgid, userIds);
            return Ok();
        }

        [HttpGet]
        [Route("{id}")]
        public IEnumerable<RoleDTO> GetRelationshipsById(Guid id)
        {
            return _service.GetById(id);
        }

        [HttpDelete]
        [Route("Members/{id}")]
        public IActionResult RemoveMembers(Guid id, [FromBody]List<Guid> userIds)
        {
            _service.RemoveMembers(id, userIds);
            return Ok();
        }
    }
}
