using CiklometarBLL.DTO;
using CiklometarDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CiklometarBLL.Services
{
   public interface IUserService
    {
        IQueryable<UserDTO> GetAllUsers();
        UserDTO GetById(Guid id);
        void AddUser(UserDTO userDTO);
        void DeleteUser(Guid id);
        void UpdateUser(Guid id, UserUpdateDTO user);
        IEnumerable<OrganizationDTO> GetAllOrgsByUserId(Guid useId);
        UserProfileDTO GetProfileData(Guid profileId, DateTime startDate, DateTime endDate);
        IQueryable<UserCyclistDTO> GetAllCyclists();
    }
}
