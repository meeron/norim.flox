using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using norim.flox.domain;

namespace norim.flox.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFileRepository _repository;

        public HomeController(IFileRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            return Content(string.Empty);
        }

        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetFile(string container, string resourceKey)
        {
            if (string.IsNullOrWhiteSpace(resourceKey))
                return NotFound();

            var isHeadRequest = Request.Method == "HEAD";
                
            var fileData = await _repository.GetAsync(container, resourceKey, isHeadRequest);
            if (fileData == null)
                return NotFound();

            foreach (var item in fileData.Metadata)
            {
                Response.Headers.Add(item.Key, item.Value);
            }

            if (isHeadRequest)
            {
                Response.ContentLength = fileData.Length;
                return Ok();
            }

            return File(fileData.Content, fileData.Metadata["Content-Type"]);
        }
    }
}