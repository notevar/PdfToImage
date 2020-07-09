using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PdfToImage;
using System.Collections.Generic;

namespace PDFToImage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHost;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHost)
        {
            _logger = logger;
            _webHost = webHost;
        }

        public IActionResult Index()
        {
            var result = ExampleOne.ConvertPDFToImage("git.pdf");
            List<string> path = new List<string>();
            for (int i = 0; i < result.Count; i++)
            {
                var filePath = $"/images/output{i}.png";
                System.IO.File.WriteAllBytes(_webHost.WebRootPath + filePath, result[i]);
                path.Add(filePath);
            }
            ViewBag.Path = path;
            return View();
        }



        [HttpGet("page/{page}")]
        public IActionResult PageDetail(int page=1)
        {
            var result = ExampleOne.ConvertPDFToImageByPage("git.pdf", page);
            var filePath = $"/images/output{page}.png";
            System.IO.File.WriteAllBytes(_webHost.WebRootPath + filePath, result);
            ViewBag.Path = filePath;
            return View();
        }
    }
}
