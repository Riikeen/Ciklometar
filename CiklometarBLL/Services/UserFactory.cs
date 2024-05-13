using CiklometarDAL.Models;
using CiklometarDAL.Repository;
using CiklometarDAL.Repositroy;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace CiklometarBLL.Services
{
    public class UserFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBasicRepository<Role> _roleRepository;
        private readonly IRepository<UserBan> _userBanRepository;

       public UserFactory(IHttpContextAccessor httpContextAccessor, IBasicRepository<Role> roleRepository, IRepository<UserBan> userBanRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _roleRepository = roleRepository;
            _userBanRepository = userBanRepository;
        }

        public Guid GetUserId()
        {
            if(_httpContextAccessor.HttpContext.User == null)
            {
                return Guid.Empty;
            }
           return Guid.Parse(_httpContextAccessor.HttpContext.User.Claims.Single(x => x.Type == "id").Value);
        }

        public bool UserIsAdminInOrganization(Guid organizationId)
        {
            var userId = GetUserId();
            if (Convert.ToBoolean( _httpContextAccessor.HttpContext.User.Claims.Single(x => x.Type == "isSuperAdmin").Value) == true)
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

        public bool IsSuperAdmin()
        {
            if(Convert.ToBoolean( _httpContextAccessor.HttpContext.User.Claims
                .Single(x => x.Type == "isSuperAdmin").Value) == true)
            {
                return true;
            }
            else { return false; }
        }
        public DateTime? UsersLastBan(int athleteId, Guid organizationId)
        {
            var usersLastBan = _userBanRepository.GetAllAsReadOnly().Where(i => i.User.StravaId == athleteId.ToString() && i.OrganizationId == organizationId).OrderBy(d => d.End).LastOrDefault();
            if(usersLastBan == null)
            {
                return null;
            }
            
            return usersLastBan.End;
        }

    }
}
