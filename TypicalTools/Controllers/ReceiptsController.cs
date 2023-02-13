using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TypicalTools.Models;
using TypicalTools.Services;
using DataAccess.Models;
using Microsoft.AspNetCore.Hosting;
using static TypicalTools.Controllers.AccountsController;
using Microsoft.AspNetCore.Authorization;

namespace TypicalTools.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReceiptsController : Controller
    {
        private readonly ILogger<ReceiptsController> _logger;
        private readonly FileLoaderService _loader;
        
        public ReceiptsController(ILogger<ReceiptsController> logger, FileLoaderService loader)
        {
            _logger = logger;
            _loader = loader;
        }
        public IActionResult Index()
        {

            string[] filePaths = Directory.GetFiles("./wwwroot/Uploads");
            //Copy File names to Model collection.
            List <FileModel> files = new List<FileModel>();
            foreach (string filePath in filePaths)
            {
                files.Add(new FileModel { FileName = Path.GetFileName(filePath) });
            }
            return View(files);
        }

        [HttpPost]
        public async Task<IActionResult> ImageUpload(IFormFile file)
        {
            await _loader.SaveFile(file);
            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(string filename)
        {
            byte[] fileBytes = await _loader.LoadEncryptedFile(filename);
            if (fileBytes == null || fileBytes.Length == 0)
            {

                return RedirectToAction(nameof(Index));
            }

            return File(fileBytes, "application/octet-stream", filename);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
