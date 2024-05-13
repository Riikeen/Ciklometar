using AutoMapper;
using CiklometarBLL.DTO;
using CiklometarDAL.Models;
using CiklometarDAL.Repository;
using CiklometarDAL.Repositroy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CiklometarBLL.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IRepository<Organization> _organizationRepository;
        private readonly IRepository<Location> _locationRepository;
        private readonly IBasicRepository<Role> _roleRepository;
        private readonly IRepository<Requests> _requestRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserBan> _banRepository;
        private readonly IActivityService _activityService;
        private readonly UserFactory _userFactory;
        private readonly IMapper _mapper;
        private readonly ContextDb _save;

        public OrganizationService(IRepository<Organization> organizationRepository,IRepository<Location> locationRepository,
            IMapper mapper,ContextDb save, IBasicRepository<Role> roleRepository, IRepository<User> userRepository, 
            IRepository<Requests> requestRepository, IActivityService activityService, UserFactory userFactory, IRepository<UserBan> banRepository)
        {
            _organizationRepository = organizationRepository;
            _locationRepository = locationRepository;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _requestRepository = requestRepository;
            _activityService = activityService;
            _banRepository = banRepository;
            _userFactory = userFactory;
            _mapper = mapper;
            _save = save;
        }

        public void AddLocation(LocationDTO locationDTO)
        {
            Location location = _mapper.Map<Location>(locationDTO);
            location.Coordinates = new NetTopologySuite.Geometries.Point(locationDTO.Lng,locationDTO.Lat) { SRID = 4326 };
            _locationRepository.Insert(location);
            _save.SaveChanges();
        }

        public void AddOrganization(AddOrganizationDTO organizationDTO, Guid userId)
        {
            if (_userFactory.IsSuperAdmin() == false)
            {
                throw new UnauthorizedAccessException("Only SuperAdmin can create Organizations");
            }
            Organization organization = _mapper.Map<Organization>(organizationDTO);
            _organizationRepository.Insert(organization);
            _save.SaveChanges();
        }

        public void DeleteOrganization(Guid id)
        {
            if(_userFactory.IsSuperAdmin() == false)
            {
                throw new UnauthorizedAccessException();
            }
            var organization = _organizationRepository.GetById(id);
            OrganizationDTO organizationDTO = _mapper.Map<OrganizationDTO>(organization);
            _organizationRepository.Delete(organization);
            _save.SaveChanges();
        }

        public IEnumerable<OrganizationDTO> GetAllOrganizations()
        {
            IEnumerable<Organization> organizations = _organizationRepository.GetAllAsReadOnly();
            IEnumerable<OrganizationDTO> organizationDTOs = _mapper.Map<IEnumerable<OrganizationDTO>>(organizations);
            return organizationDTOs;
        }

        public IEnumerable<UserCyclistDTO> GetAllUsersInOrganization(Guid orgId)
        {
            var userIds = _roleRepository.GetAll().Where(i => i.OrganizationId == orgId && i.UserType == UserType.Cyclist).Select(u => u.UserId);
            IEnumerable<User> users = _userRepository.GetAll().Where(id => userIds.Contains(id.Id));
            IEnumerable<UserCyclistDTO> userDTOs = _mapper.Map<IEnumerable<UserCyclistDTO>>(users);
            return userDTOs;
        }

        public OrganizationDTO GetById(Guid id)
        {
            OrganizationDTO organizationDTO = _mapper.ProjectTo<OrganizationDTO>(_organizationRepository.GetAll()
                .Where(s => s.Id == id)).FirstOrDefault();
            return organizationDTO; 
        }

        public OrganizationProfileDTO GetProfileData(Guid id, Guid sessionId, DateTime startDate, DateTime endDate)
        {
            OrganizationDTO organizationDTO = _mapper.Map<OrganizationDTO>(_organizationRepository.GetById(id));
            
            IEnumerable<UserCyclistDTO> users = GetAllUsersInOrganization(id);
            List<OrganizationRankingDTO> rankedMembers = users.Select(user => new OrganizationRankingDTO()
            {
                UserData = user
            }).ToList();

            //this is bad and is only a temporary measure
            //this in the future must be handled by claims in session but currently our security stamp
            //of data in the access token doesn't invalidate the JWT token when a new request is sent or
            //a new member is accepted so I'm forced to do this because I don't have time to learn
            //how JWT security stamps work or how to fix them for us
            bool sessionIsMemberOfThisOrganization = users.Where(user => user.Id == sessionId).Any();
            bool sessionSentRequestToThisOrganization = _requestRepository.GetAllAsReadOnly().Where(request => request.OrganizationId == id && request.UserId == sessionId).Any();

            IEnumerable<ActivityOutputDTO> organizationActivities = _activityService.GetActivitiesByOrganizationId(id);
            IEnumerable<RankedRideDTO> rankedRides = organizationActivities.Select(activity => new RankedRideDTO()
            {
                AverageSpeed = activity.AvgSpeed,
                DistanceCycled = activity.Distance,
                TotalTimeCycled = activity.Moving_time,
                AthleteId = activity.AthleteId,
                OrganizationId = activity.OrganizationId,
                Timestamp = activity.Event_time
            });
            IEnumerable<RankedRideDTO> relevantRides = rankedRides.Where(ride => ride.Timestamp > startDate && ride.Timestamp < endDate);

            foreach (var rankedMember in rankedMembers)
            {
                List<RankedRideDTO> filteredRides = relevantRides.Where(ride => ride.AthleteId == rankedMember.UserData.StravaId).ToList();
                if (filteredRides.Any())
                {
                    rankedMember.RankedRidesByThisUser = filteredRides;
                }
            }

            OrganizationProfileDTO organizationRankingDTO = new OrganizationProfileDTO
            {
                OrganizationData = organizationDTO,
                RankedRidesByMembers = rankedMembers,
                UserIsInOrganization = sessionIsMemberOfThisOrganization,
                UserAlreadySentJoinRequest = sessionSentRequestToThisOrganization,
            };
            return organizationRankingDTO;
        }

        public IEnumerable<LocationResponseDTO> GetLocationsByOrganizationId(Guid id)
        {
            if (_userFactory.UserIsAdminInOrganization(id) == true)
            {
                IEnumerable<Location> locations = _locationRepository.GetAll().Where(o => o.OrganizationId == id);
                IEnumerable<LocationResponseDTO> locationsDTO = _mapper.Map<IEnumerable<LocationResponseDTO>>(locations);
                return locationsDTO;
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }

        public void UpdateOrganization(OrganizationDTO organization)
        {
            if (_userFactory.UserIsAdminInOrganization(organization.Id) == false)
            {
                throw new UnauthorizedAccessException("User is not an Admin");
            }
            Organization organizationDTO = _mapper.Map<Organization>(organization);
            _organizationRepository.Update(organizationDTO);
            _save.SaveChanges();
        }

        public void UpdateOrganizationLocations(Guid id, List<LocationDTO> newLocations)
        {
            if (_userFactory.UserIsAdminInOrganization(id))
            {
                var locationsToDelete = _locationRepository.GetAll().Where(i => i.OrganizationId == id).ToList();
                _locationRepository.DeleteRange(locationsToDelete);

                var locationsToAdd = newLocations.Select(l => new Location()
                {
                    OrganizationId = id,
                    Coordinates = new NetTopologySuite.Geometries.Point(l.Lat, l.Lng) { SRID = 4326 }
                });
                _locationRepository.AddRange(locationsToAdd);
                _save.SaveChanges();
            }
            else { throw new UnauthorizedAccessException(); }
        }

        public IEnumerable<OrganizationDTO> GetOrganizationsAvailableToJoin(Guid userId)
        {
            var orgIds = _roleRepository.GetAll().Where(i => i.UserId == userId).Select(u => u.OrganizationId);
            
            IEnumerable<Organization> organizations = _organizationRepository.GetAll().Where(org => !orgIds.Contains(org.Id));
            IEnumerable<OrganizationDTO> organizationDTOs = _mapper.Map<IEnumerable<OrganizationDTO>>(organizations);
            return organizationDTOs;
        }
        public IEnumerable<OrganizationDTO> GetOrganizationsAdminOf(Guid userId)
        {
            User user = _userRepository.GetById(userId);

            if(user.IsSuperAdmin)
            {
                return this.GetAllOrganizations();
            }

            var orgIds = _roleRepository.GetAll().Where(i => i.UserId == userId && i.UserType == UserType.Admin).Select(u => u.OrganizationId);

            IEnumerable<Organization> organizations = _organizationRepository.GetAll().Where(org => orgIds.Contains(org.Id));
            IEnumerable<OrganizationDTO> organizationDTOs = _mapper.Map<IEnumerable<OrganizationDTO>>(organizations);

            return organizationDTOs;
        }

        public void BanUsersFromOrganization(List<Guid> userIds, Guid orgId, long timeInMiliseconds)
        {
            Guid sessionId = _userFactory.GetUserId();

            if(userIds == null)
            {
                throw new ArgumentException("No users to ban provided.");
            }

            Organization organization = _organizationRepository.GetById(orgId);
            if (organization == null)
            {
                throw new ArgumentException("This organization doesn't exist.");
            }

            List<Role> usersInOrganization = _roleRepository.GetAllAsReadOnly()
                                             .Where(role => role.OrganizationId == orgId).ToList();
            List<Guid> adminsOfOrganization = usersInOrganization.Where(role => role.UserType == UserType.Admin)
                                              .Select(role => role.UserId).ToList();

            if(!adminsOfOrganization.Contains(sessionId) && !_userFactory.IsSuperAdmin())
            {
                throw new Exception("You don't have ban privileges for this organization.");
            }

            foreach (var admin in adminsOfOrganization)
            {
                if(userIds.Contains(admin))
                {
                    throw new Exception("Admins of an organization cannot be banned.");
                }
            };

            List<Guid> membersOfOrganization = usersInOrganization.Where(role => role.UserType == UserType.Cyclist).Select(role => role.UserId).ToList();

            foreach (var member in membersOfOrganization)
            {
                if (!userIds.Contains(member))
                {
                    throw new Exception("This user is not a member of this organization or does not exist.");
                }
            };

            DateTime currentTime = DateTime.Now;

            DateTime banExpiration = DateTimeOffset.FromUnixTimeMilliseconds(timeInMiliseconds).UtcDateTime;

            if(currentTime > banExpiration)
            {
                throw new Exception("Ban expiration time has already passed.");
            }

            IEnumerable<User> usersToBan = _userRepository.GetAllAsReadOnly().Where(user => userIds.Contains(user.Id));

            IEnumerable<UserBan> bans = usersToBan.Select(user => new UserBan
            {
                Reason = "No reason provided.",
                Start = currentTime,
                End = banExpiration,
                UserId = user.Id,
                OrganizationId = orgId,
                BannedById = sessionId,
            });
            _banRepository.AddRange(bans);
            _save.SaveChanges();
        }


        //returns activities grouped per athlete 
        public List<ActivitesInOrganizationDTO> GetAtivitiesInOrganization(Guid organizationId)
        {
            var activites = _activityService.GetActivitiesByOrganizationId(organizationId).ToList();
            var orgActivites = new List<ActivitesInOrganizationDTO>();

            var groupedActivities = activites.GroupBy(g => g.AthleteId).Select(x => new ActivityOutputDTO
            {
                AthleteId = x.Key,
                Distance = x.Sum(x => x.Distance),
                AvgSpeed = (long)x.Average(x =>x.AvgSpeed),
                Elapsed_time = x.Sum(x => x.Elapsed_time),
                OrganizationId = x.Select(x => x.OrganizationId).FirstOrDefault(),
                ActivityId = x.Select(x => x.ActivityId).FirstOrDefault(),
                Event_time = x.Select(x => x.Event_time).FirstOrDefault(),
                Lat = x.Select(x => x.Lat).FirstOrDefault(),
                Lng = x.Select(x => x.Lng).FirstOrDefault(),
                Moving_time = x.Sum(x => x.Moving_time),
                Type = x.Select(x => x.Type).FirstOrDefault(),
            }).ToList();
           
            foreach(var activity in groupedActivities)
            {
                var temp = new ActivitesInOrganizationDTO()
                {
                    ActivityOutputDTO = activity,
                    CyclistName = _userRepository.GetAll().Where(id => id.StravaId == activity.AthleteId).Select(n => n.Nickname).FirstOrDefault(),
                    OrganizationName = _organizationRepository.GetById(activity.OrganizationId).Name
                };
                orgActivites.Add(temp);
            }
             return orgActivites;

           

        }
    }
}
