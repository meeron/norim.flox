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

        [HttpGet("{container}/{*resourceKey}")]
        public async Task<IActionResult> Index(string container, string resourceKey)
        {
            var fileData = await _repository.GetAsync(container, resourceKey);
            if (fileData == null)
                return NotFound();

            foreach (var item in fileData.Metadata)
            {
                Response.Headers.Add(item.Key, item.Value);
            }

            return File(fileData.Content, fileData.Metadata["Content-Type"]);
        }
    }
}