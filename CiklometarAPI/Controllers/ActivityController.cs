using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CiklometarBLL.DTO;
using CiklometarBLL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CiklometarAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _activityService;
        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        //we don't need any of these to be exposed to anyone anymore
    }
}
