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
    public class ActivityService : IActivityService
    {
        private readonly IBasicRepository<Activity> _activityRepository;
        private readonly IMapper _mapper;

        public ActivityService(IBasicRepository<Activity> activityRepository, IMapper mapper)
        {
            _activityRepository = activityRepository;
            _mapper = mapper;
        }

        public IEnumerable<ActivityOutputDTO> GetActivitiesByDate(DateTime startDate, DateTime endDate)
        {
            IQueryable<Activity> activities = _activityRepository.GetAllAsReadOnly().Where(i => i.Event_time > startDate && i.Event_time < endDate);
            IEnumerable<ActivityOutputDTO> activityOutputDTOs = _mapper.ProjectTo<ActivityOutputDTO>(activities);
            return activityOutputDTOs;
        }


        public IEnumerable<ActivityOutputDTO> GetActivitiesByOrganizationId(Guid organizationId)
        {
            IQueryable<Activity> activities = _activityRepository.GetAllAsReadOnly().Where(i => i.OrganizationId == organizationId);
            IEnumerable<ActivityOutputDTO> activityOutputDTOs = _mapper.ProjectTo<ActivityOutputDTO>(activities);
            return activityOutputDTOs;
        }

        public IEnumerable<ActivityOutputDTO> GetActivitiesByUser(string id)
        {
            IQueryable<Activity> activities = _activityRepository.GetAllAsReadOnly().Where(i => i.AthleteId == id);
            IEnumerable<ActivityOutputDTO> activityOutputDTOs = _mapper.ProjectTo<ActivityOutputDTO>(activities);
            return activityOutputDTOs;
        }
    }
}
