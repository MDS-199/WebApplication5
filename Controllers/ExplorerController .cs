using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApplication5.Models;

namespace WebApplication5.Controllers
{
    public class ExplorerController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        private readonly ExplorerDbContext _context = new ExplorerDbContext();

        public ExplorerController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {

            /*List<FilesExtensions> filesextensions = _context.filesextensions.ToList();

            List<Folders> folders = _context.folders.ToList();

            List<Files> files = _context.files.ToList();

            var dataTuple = (files, folders, filesextensions);
            return View(dataTuple); */


            var viewModel = new ExplorerViewModel
            {
                files = _context.files.ToList(),
                folders = _context.folders.ToList(),
                fileextensions = _context.filesextensions.ToList()
            };

            return View(viewModel);


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