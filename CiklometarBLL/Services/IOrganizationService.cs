using CiklometarBLL.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace CiklometarBLL.Services
{
   public interface IOrganizationService
    {
        IEnumerable<OrganizationDTO> GetAllOrganizations();
        IEnumerable<LocationResponseDTO> GetLocationsByOrganizationId(Guid id);
        OrganizationDTO GetById(Guid id);
        void AddOrganization(AddOrganizationDTO organizationDTO, Guid userId);
        void AddLocation(LocationDTO locationDTO);
        void DeleteOrganization(Guid id);
        void UpdateOrganization(OrganizationDTO organization);
        IEnumerable<UserCyclistDTO> GetAllUsersInOrganization(Guid orgId);
        void UpdateOrganizationLocations(Guid id, List<LocationDTO> newLocations);
        OrganizationProfileDTO GetProfileData(Guid id, Guid sessionId, DateTime start, DateTime end);
        IEnumerable<OrganizationDTO> GetOrganizationsAvailableToJoin(Guid guid);
        IEnumerable<OrganizationDTO> GetOrganizationsAdminOf(Guid userId);
        void BanUsersFromOrganization(List<Guid> userIds, Guid orgId, long timeInMiliseconds);
        List<ActivitesInOrganizationDTO> GetAtivitiesInOrganization(Guid organizationId);
    }
}
