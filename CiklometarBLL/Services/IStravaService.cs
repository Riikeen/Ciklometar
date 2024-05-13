using CiklometarBLL.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CiklometarBLL.Services
{
    public interface IStravaService
    {
        public Task CreateWebhook();
        ChallengeDTO VerifySubscription(string link);
        public Task<AccessTokenDataDTO> RegisterWithStrava(string code);
        public Task<AccessTokenDataDTO> LoginWithStrava(string code);
        public Task<AccessTokenDataDTO> ConnectWithStrava(string code, Guid userId);
        Task<bool> HandleWebhookEvent(StravaWebhookDTO stravaWebhookDTO);
        public Task DeleteWebhook();
        public Task SyncActivitesWithStrava(long afterTime);

    }
}
