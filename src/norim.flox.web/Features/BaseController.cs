using Microsoft.AspNetCore.Mvc;
using norim.flox.web.Models;
using norim.flox.web.Extensions;

namespace norim.flox.web.Features
{
    public abstract class BaseController : Controller
    {
        public OkObjectResult OkObject()
        {
            return new OkObjectResult(new OkResponse(HttpContext.GetRequestId()));
        }

        public BadRequestObjectResult BadRequestObject(string msg)
        {
            return new BadRequestObjectResult(
                new ErrorResponse(HttpContext.GetRequestId(), msg));
        }
    }
}