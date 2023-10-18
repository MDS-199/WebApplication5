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
            var viewModel = new ExplorerViewModel
            {
                files = _context.files.ToList(),
                folders = _context.folders.ToList(),
                fileextensions = _context.filesextensions.ToList()
            };

            return View(viewModel);


        }

        [HttpGet]
        public IActionResult GetFileContent(int id)
        {
            // Здесь ваш код для получения содержимого файла из базы данных по id
            // Например, используйте Entity Framework для доступа к базе данных
            var file = _context.files.FirstOrDefault(e => e.id == id);

            if (file != null)
            {
                string Filecontent = file.filecontent; // Здесь подставьте имя свойства, которое вы хотите получить
                return Content(Filecontent);
            }

            return NotFound(); // Если файл с заданным id не найден
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