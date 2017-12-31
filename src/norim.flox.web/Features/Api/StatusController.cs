using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace norim.flox.web.Features.Api
{
    public class StatusController : Controller
    {
        [HttpGet]
        public object Get()
        {
            return new { version = "1.0.0", status = "OK" };
        }
    }
}
