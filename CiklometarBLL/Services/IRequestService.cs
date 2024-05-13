using CiklometarBLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CiklometarBLL.Services
{
    public interface IRequestService
    {
        IEnumerable<RequestsGetDTO> GetAllRequests();
        IEnumerable<RequestsGetDTO> GetRelevantRequests(Guid id);
        public void AddRequest(Guid Userid, List<Guid> orgIds);
        public void HandleRequest(List<RequestIdDTO> request);
    }
}
