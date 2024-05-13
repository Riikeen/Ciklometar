using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CiklometarBLL.DTO;
using CiklometarBLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace CiklometarAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        
        private readonly IStravaService _webhookService;

        public WebhookController( IStravaService webhookService)
        {
            
            _webhookService = webhookService;
        }

        [HttpGet]
        public ChallengeDTO VerifySubscription([Bind(Prefix = "hub.verify_token")] string verify_token, [Bind(Prefix = "hub.challenge")] string challenge, [Bind(Prefix = "hub.mode")] string mode)
        {
           return _webhookService.VerifySubscription(challenge);
        }

        [HttpGet("Create")]
        public void CreateWebhook()
        {
            _webhookService.CreateWebhook();
        }

        [HttpPost]
        public async Task<IActionResult> HandleWebhookEvent ([FromBody] StravaWebhookDTO stravaWebhookDTO)
        {
            try
            {
                var response = await _webhookService.HandleWebhookEvent(stravaWebhookDTO);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
          
        }
        [HttpGet("Delete")]
        public async Task<IActionResult> DeleteWebhook()
        {
             await _webhookService.DeleteWebhook();
            return Ok();
        }

        [HttpGet("SyncWithStrava/{time}")]
        public async Task<IActionResult> SyncWithStrava(long time)
        {
           await _webhookService.SyncActivitesWithStrava(time);
            return Ok() ;
        }

    }
}
