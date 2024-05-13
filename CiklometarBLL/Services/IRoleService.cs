using CiklometarBLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CiklometarBLL.Services
{
   public interface IRoleService
   {
        public void AddRelationship(RoleDTO roleDTO);
        IEnumerable<RoleDTO> GetAllRelationships();
        void UpdateRelationships(Guid id, List<Guid> userIds);
        IEnumerable<RoleDTO> GetById(Guid id);
        void RemoveMembers(Guid orgId, List<Guid> userIds);
        IEnumerable<RoleDTO> GetRelevantRelationships(Guid userId);
    }
}
