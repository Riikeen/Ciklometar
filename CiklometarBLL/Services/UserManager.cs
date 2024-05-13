using CiklometarBLL.DTO;
using CiklometarDAL.Models;
using CiklometarDAL.Repository;
using CiklometarDAL.Repositroy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CiklometarBLL.Services
{
    public interface IUserManager
    {
        public bool UserIsAdminInOrganization(Guid organizationId, Guid userId);
        public bool IsUserAdminInAnyOrganization(Guid userId);
        public bool IsUserMemberOfAnyOrganization(Guid userId);
        public bool IsConnectedWithStrava(Guid userId);
        public bool UserIsSuperadmin(Guid userId);
        bool UserCanResolveRequest(Guid requestId, Guid userId);
        bool UserCanResolveListOfRequests(List<RequestIdDTO> requestId, Guid userId);
        bool UserCanGetRelevantRequests(Guid id, Guid userId);
        bool CanAccessUserInfo(Guid user, Guid session);
    }

   public class UserManager : IUserManager
   {
        private readonly IBasicRepository<Role> _roleRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Requests> _requestRepository;

        public UserManager(IBasicRepository<Role> roleRepository, IRepository<User> userRepository, IRepository<Requests> requestRepository)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _requestRepository = requestRepository;
        }

        public bool IsConnectedWithStrava(Guid userId)
        {
            if (_userRepository.GetById(userId).StravaId != null)
            {
                return true;
            }
            return false;
        }

        public bool IsUserAdminInAnyOrganization(Guid userId)
        {
            if(UserIsSuperadmin(userId))
            {
                return true;
            }

            if(_roleRepository.GetAllAsReadOnly().Where(i => i.UserId == userId && i.UserType == UserType.Admin).Any())
            {
                return true;
            }
            return false;
        }

        public bool IsUserMemberOfAnyOrganization(Guid userId)
        {
            if (_roleRepository.GetAllAsReadOnly().Where(i => i.UserId == userId && i.UserType == UserType.Cyclist).Any())
            {
                return true;
            }
            return false;
        }

        public bool UserIsAdminInOrganization(Guid organizationId, Guid userId)
        {
            if (UserIsSuperadmin(userId))
            {
                return true;
            }
            return _roleRepository.GetAll()
                .Where(role => 
                role.OrganizationId == organizationId 
                && role.UserId == userId 
                && role.UserType == UserType.Admin)
                .Any();
        }

        public bool CanAccessUserInfo(Guid user, Guid session)
        {
            if (UserIsSuperadmin(session))
            {
                return true;
            }

            if (user == session)
            {
                return true;
            }

            return false;
        }

        public bool UserCanGetRelevantRequests(Guid id, Guid userId)
        {
            if (UserIsSuperadmin(userId))
            {
                return true;
            }

            if (id == userId)
            {
                return true;
            }

            if(_roleRepository.GetAllAsReadOnly().Where(i => i.OrganizationId == id && i.UserId == userId && i.UserType == UserType.Admin).Any())
            {
                return true;
            }

            return false;
        }

        public bool UserCanResolveListOfRequests(List<RequestIdDTO> requestId, Guid userId)
        {
            if (UserIsSuperadmin(userId))
            {
                return true;
            }

            foreach (RequestIdDTO request in requestId)
            {
                if (!UserCanResolveRequest(request.Id, userId))
                {
                    return false;
                }
            }
            return true;
        }

        public bool UserCanResolveRequest(Guid requestId, Guid userId)
        {
            Requests request = _requestRepository.GetById(requestId);
            return _roleRepository.GetAll().Where(i => i.OrganizationId == request.OrganizationId && i.UserId == request.UserId && i.UserType == UserType.Admin).Any();
        }

        public bool UserIsSuperadmin(Guid userId)
        {
            User user = _userRepository.GetById(userId);

            return user.IsSuperAdmin;
        }
    }
}
