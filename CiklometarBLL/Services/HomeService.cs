using AutoMapper;
using CiklometarBLL.DTO;
using CiklometarDAL.Models;
using CiklometarDAL.Repository;
using CiklometarDAL.Repositroy;
using System;
using System.Collections.Generic;
using System.Text;

namespace CiklometarBLL.Services
{
    public class HomeService : IHomeService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IBasicRepository<Role> _roleRepository;
        private readonly IActivityService _activityService;
        private readonly IRepository<Organization> _organizationRepository;
        private readonly UserFactory _userFactory;
        private readonly IMapper _mapper;
        private readonly ContextDb _save;

        public HomeService(IRepository<User> userRepository, IMapper mapper, ContextDb save,
            IBasicRepository<Role> roleRepository, IRepository<Organization> organizationRepository,
            IActivityService activityService, UserFactory userFactory, IStravaService stravaService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _activityService = activityService;
            _organizationRepository = organizationRepository;
            _userFactory = userFactory;
            _mapper = mapper;
            _save = save;
        }

        public HomepageDTO GetHomepageData()
        {
            return new HomepageDTO();
        }

    }
}
