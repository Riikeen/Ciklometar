using AutoMapper;
using CiklometarBLL.DTO;
using CiklometarDAL.Models;
using CiklometarDAL.Repository;
using CiklometarDAL.Repositroy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CiklometarBLL.Services
{
   public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IBasicRepository<Role> _roleRepository;
        private readonly IRepository<Organization> _organizationRepository;
        private readonly IActivityService _activityService;
        private readonly IStravaService _stravaService;
        private readonly UserFactory _userFactory;
        private readonly IMapper _mapper;
        private readonly ContextDb _save; //_context.SaveChanges() izvađen u zasebnu klasu.

        public UserService(IRepository<User> userRepository,IMapper mapper,ContextDb save,
            IBasicRepository<Role> roleRepository, IRepository<Organization> organizationRepository,
            IActivityService activityService, UserFactory userFactory, IStravaService stravaService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _organizationRepository = organizationRepository;
            _activityService = activityService;
            _stravaService = stravaService;
            _userFactory = userFactory;
            _mapper = mapper;
            _save = save;
           
        }

        public void AddUser(UserDTO userDTO)
        {
            User user = _mapper.Map<User>(userDTO);
            _userRepository.Insert(user);
            _save.SaveChanges();
        }

        public void DeleteUser(Guid id)
        {
            if(_userFactory.IsSuperAdmin()== true)
            {
                var user = _userRepository.GetById(id);
                _userRepository.Delete(user);
                _save.SaveChanges();
            }
            else { throw new UnauthorizedAccessException(); }
        }

        public IEnumerable<OrganizationDTO> GetAllOrgsByUserId(Guid userId)
        {
            var OrgIds = _roleRepository.GetAll().Where(i => i.UserId == userId).Select(id => id.OrganizationId);
            IEnumerable<Organization> orgs = _organizationRepository.GetAll().Where(id => OrgIds.Contains(id.Id));
            IEnumerable<OrganizationDTO> organizationDTOs = _mapper.Map<IEnumerable<OrganizationDTO>>(orgs);
            return organizationDTOs;
        }

        public IQueryable<UserDTO> GetAllUsers()
        {
            IQueryable<User> users = _userRepository.GetAllAsReadOnly();
            IQueryable<UserDTO> userDTOs = _mapper.ProjectTo<UserDTO>(users);
            return userDTOs;
        }

        public IQueryable<UserCyclistDTO> GetAllCyclists()
        {
            IQueryable<User> users = _userRepository.GetAllAsReadOnly().Where(user => user.StravaId != null);
            IQueryable<UserCyclistDTO> userDTOs = _mapper.ProjectTo<UserCyclistDTO>(users);
            return userDTOs;
        }

        public UserDTO GetById(Guid id)
        {
            var user = _userRepository.GetById(id);
            UserDTO userDTO = _mapper.Map<UserDTO>(user);
            return userDTO;
        }

        public UserProfileDTO GetProfileData(Guid profileId, DateTime startDate, DateTime endDate)
        {
            User profileUserData = _userRepository.GetById(profileId);
            bool profileUserIsNotCyclist = profileUserData.StravaId == null;

            UserDTO profileUserDataDTO = _mapper.Map<UserDTO>(profileUserData);

            IEnumerable <ActivityOutputDTO> profileUserActivities = _activityService.GetActivitiesByUser(profileUserData.StravaId);
            IEnumerable<RankedRideDTO> profileRankedRides = profileUserActivities.Select(activity => new RankedRideDTO()
            {
                AverageSpeed = activity.AvgSpeed,
                DistanceCycled = activity.Distance,
                TotalTimeCycled = activity.Moving_time,
                AthleteId = activity.AthleteId,
                OrganizationId = activity.OrganizationId,
                OrganizationData = _organizationRepository.GetById(activity.OrganizationId).Name,
                Timestamp = activity.Event_time
            });
            IEnumerable<RankedRideDTO> profileRelevantRides = profileRankedRides.Where(ride => ride.Timestamp > startDate && ride.Timestamp < endDate);

            UserProfileDTO userProfileDTO = new UserProfileDTO
            {
                UserData = profileUserDataDTO,
                Rides = profileRelevantRides.ToList(),
                Organizations = GetAllOrgsByUserId(profileId).ToList(),
                ProfileUserIsNotCyclist = profileUserIsNotCyclist,
            };

            return userProfileDTO;
        }

        public void UpdateUser(Guid id, UserUpdateDTO userUpdate)
        {

            User userDB = _userRepository.GetById(id);

            if(userDB == null)
            {
                throw new Exception("This user does not exist.");
            }

            if (BLLFunctions.CreateHash(userUpdate.CurrentPassword, userDB.Salt) != userDB.Password)
            {
                throw new Exception("Incorrect password.");
            }

            if(userUpdate.Password != null)
            {
                var salt = BLLFunctions.CreateSalt();
                var pwHash = BLLFunctions.CreateHash(userUpdate.Password, salt);
                userDB.Salt = salt;
                userDB.Password = pwHash;
            }

            if (userUpdate.Email != null)
            {
                userDB.Email = userUpdate.Email;
            }

            if (userUpdate.Nickname != null)
            {
                userDB.Nickname = userUpdate.Nickname;
            }

            if (userUpdate.FirstName != null)
            {
                userDB.FirstName = userUpdate.FirstName;
            }

            if (userUpdate.LastName != null)
            {
                userDB.LastName = userUpdate.LastName;
            }

            if (userUpdate.StravaCode != null)
            {
                _stravaService.ConnectWithStrava(userUpdate.StravaCode, id);
            }

            _userRepository.Update(userDB);
            _save.SaveChanges();
        }
    }
}
