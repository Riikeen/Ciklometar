using CiklometarBLL.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace CiklometarBLL.Services
{
    public interface IActivityService
    {
        IEnumerable<ActivityOutputDTO> GetActivitiesByOrganizationId(Guid organizationId);
        IEnumerable<ActivityOutputDTO> GetActivitiesByUser(string id);
        IEnumerable<ActivityOutputDTO> GetActivitiesByDate(DateTime startDate, DateTime endDate);
    }
}
