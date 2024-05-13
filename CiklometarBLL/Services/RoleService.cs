using AutoMapper;
using CiklometarBLL.DTO;
using CiklometarDAL.Models;
using CiklometarDAL.Repositroy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CiklometarBLL.Services
{ 
    public class RoleService : IRoleService
    {
        private readonly IMapper _mapper;
        private readonly IBasicRepository<Role> _roleRepository;
        private readonly ContextDb _save;
        public RoleService(IMapper mapper, IBasicRepository<Role> roleRepository,ContextDb save)
        {
            _mapper = mapper;
            _roleRepository = roleRepository;
            _save = save;
        }
        public void AddRelationship(RoleDTO roleDTO)
        {
            Role role = _mapper.Map<RoleDTO, Role>(roleDTO);
            _roleRepository.Insert(role);
            _save.SaveChanges();

        }
        public IEnumerable<RoleDTO> GetAllRelationships()
        {
            IEnumerable<Role> roles = _roleRepository.GetAllAsReadOnly();
            IEnumerable<RoleDTO> roleDTOs = _mapper.Map<IEnumerable<Role>, IEnumerable<RoleDTO>>(roles);
            return roleDTOs;
        }
        public IEnumerable<RoleDTO> GetRelevantRelationships(Guid id)
        {
            IEnumerable<Role> roles = _roleRepository.GetAllAsReadOnly().Where(i => i.UserId == id || i.OrganizationId == id);
            IEnumerable<RoleDTO> roleDTOs = _mapper.Map<IEnumerable<Role>, IEnumerable<RoleDTO>>(roles);
            return roleDTOs;
        }

        public IEnumerable<RoleDTO> GetById(Guid id)
        {
            var role = _roleRepository.GetAllAsReadOnly().Where(i => i.UserId == id | i.OrganizationId == id).ToList();
            var roleDTO = _mapper.Map<IEnumerable<Role>, IEnumerable<RoleDTO>>(role);
            return roleDTO;   
        }

        public void UpdateRelationships(Guid orgId, List<Guid> newUserId)
        {
           
            var orgIds = _roleRepository.GetAll().Where(u => u.OrganizationId == orgId)
                .Select(o => o.UserId).ToList();
            //add selected roles
            var rolesToAdd = newUserId.Where(userId => !orgIds.Contains(userId))
                .Select(uId => new Role()
                {
                    OrganizationId = orgId,
                    UserId = uId,
                    UserType = UserType.Admin
                }).ToList();
            _roleRepository.AddRange(rolesToAdd);

            //delete unselected roles
            var roleIdsToDelete = orgIds.Where(userId => !newUserId.Contains(userId)).ToList();
            var rolesToDelete = _roleRepository.GetAll().Where(r => roleIdsToDelete.Contains(r.UserId)).ToList();  
             _roleRepository.DeleteRange(rolesToDelete);

            AddAsCyclist(orgId, newUserId);
            _save.SaveChanges();            
        }

        private void AddAsCyclist(Guid orgId, List<Guid> newUserIds)
        {
            var orgIds = _roleRepository.GetAll().Where(u => u.OrganizationId == orgId)
               .Select(o => o.UserId).ToList();

            var rolesToAdd = newUserIds.Where(userId => !orgIds.Contains(userId))
                .Select(oId => new Role()
                {
                    OrganizationId = orgId,
                    UserId = oId,
                    UserType = UserType.Cyclist
                }).ToList();

            foreach (var role in rolesToAdd)
            {
                _roleRepository.Insert(role);
            }
        }

        public void RemoveMembers(Guid orgId, List<Guid> userIds)
        {
            var allCurrentMembers = _roleRepository.GetAll()
                .Where(u => u.OrganizationId == orgId)
                .ToList();

            var membersToRemove = allCurrentMembers
                .Where(u => userIds
                .Contains(u.UserId))
                .ToList();

            foreach(var member in membersToRemove)
            {
                _roleRepository.Delete(member);
            }

            _save.SaveChanges();
        }
    }
}
