using AutoMapper;
using CiklometarBLL.DTO;
using CiklometarDAL.Models;
using CiklometarDAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CiklometarBLL.Services
{
    public class RequestService : IRequestService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Requests> _requestRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Organization> _organizationRepository;
        private readonly IRoleService _roleService;
        private readonly UserFactory _userFactory;
        private readonly ContextDb _save;

        public RequestService(IMapper mapper, IRepository<Requests> requestRepository, 
            ContextDb save, IRoleService roleService, IRepository<User> userRepository, 
            IRepository<Organization> organizationRepository,UserFactory userFactory)
        {
            _mapper = mapper;
            _save = save;
            _requestRepository = requestRepository;
            _userRepository = userRepository;
            _organizationRepository = organizationRepository;
            _roleService = roleService;
            _userFactory = userFactory;
        } 
        public void AddRequest(Guid userId, List<Guid> orgIds)
        {
            if(_userFactory.GetUserId()!= userId)
            {
                throw new ArgumentException("You cannot add requests for other users");
            }
            var userRoles = _roleService.GetAllRelationships();
            IEnumerable<Requests> requestsToCheck = orgIds.Select(oid => new Requests()
            {
                OrganizationId = oid,
                UserId = userId,
            });

            List<Requests> filteredRequests = new List<Requests>();

            foreach (Requests request in requestsToCheck)
            {
                if (!userRoles.Any(user => 
                    user.UserId == request.UserId && 
                    user.OrganizationId == request.OrganizationId &&
                    user.UserType == UserType.Cyclist
                ))
                {
                    if (!_requestRepository.GetAllAsReadOnly().Any(user =>
                        user.UserId == request.UserId &&
                        user.OrganizationId == request.OrganizationId
                    ))
                    {
                        filteredRequests.Add(request);
                    }
                }
            }

            IEnumerable<Requests> requestsToAdd = filteredRequests.AsEnumerable<Requests>();

            _requestRepository.AddRange(requestsToAdd);
            _save.SaveChanges();
        }

        public IEnumerable<RequestsGetDTO> GetAllRequests()
        {
            if (_userFactory.IsSuperAdmin())
            {
                IEnumerable<Requests> requests = _requestRepository.GetAllAsReadOnly();
                IEnumerable<RequestsGetDTO> requestsDTO = _mapper.Map<IEnumerable<RequestsGetDTO>>(requests);
                return requestsDTO;
            }
            else { throw new UnauthorizedAccessException(); }
        }

        public IEnumerable<RequestsGetDTO> GetRelevantRequests(Guid id)
        {
            var requestIds = _requestRepository.GetAllAsReadOnly().Where(i => i.OrganizationId == id || i.UserId == id).Select(u => u.Id);

            IEnumerable<Requests> requests = _requestRepository.GetAllAsReadOnly().Where(id => requestIds.Contains(id.Id));
            IEnumerable<RequestsGetDTO> requestsDTO = _mapper.Map<IEnumerable<RequestsGetDTO>>(requests);

            if (requests.Select(i => i.OrganizationId == id).Any())
            {
                var userIds = _requestRepository.GetAllAsReadOnly().Where(i => requestIds.Contains(i.Id)).Select(u => u.UserId);
                IEnumerable<User> users = _userRepository.GetAllAsReadOnly().Where(id => userIds.Contains(id.Id));
                IEnumerable<UserDTO> userDTOs = _mapper.Map<IEnumerable<UserDTO>>(users);

                foreach (var request in requestsDTO)
                {
                    request.User = userDTOs.TakeWhile(user => user.Id == request.UserId).FirstOrDefault();
                }
            } else
            {
                var organizationIds = _requestRepository.GetAllAsReadOnly().Where(i => requestIds.Contains(i.Id)).Select(u => u.OrganizationId);
                IEnumerable<Organization> organizations = _organizationRepository.GetAllAsReadOnly().Where(id => organizationIds.Contains(id.Id));
                IEnumerable<OrganizationDTO> organizationDTOs = _mapper.Map<IEnumerable<OrganizationDTO>>(organizations);

                foreach (var request in requestsDTO)
                {
                    request.Organization = organizationDTOs.TakeWhile(organization => organization.Id == request.OrganizationId).FirstOrDefault();
                }
            }
            return requestsDTO;
        }

        public IEnumerable<RequestsDTO> GetById(Guid id)
        {
            var request = _requestRepository.GetAllAsReadOnly().Where(i => i.OrganizationId == id).ToList();
            var requestDTO = _mapper.Map<IEnumerable<RequestsDTO>>(request);
            return requestDTO;
        }

        public void HandleRequest(List<RequestIdDTO> requestDTOList)
        {
            for (int i = 0; i < requestDTOList.Count; i++)
            {
                RequestIdDTO requestDTO = requestDTOList[i];
                Requests request = _requestRepository.GetById(requestDTO.Id);
                if (requestDTO.Request == true)
                {
                    RoleDTO r = new RoleDTO()
                    {
                        OrganizationId = request.OrganizationId,
                        UserId = request.UserId,
                        UserType = UserType.Cyclist
                    };
                    _roleService.AddRelationship(r);
                    _requestRepository.Delete(request);
                }
                else
                {
                    _requestRepository.Delete(request);
                }
            }
            _save.SaveChanges();
        }
    }
}
