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
            var file = _context.files.FirstOrDefault(e => e.id == id);

            if (file != null)
            {
                string Filecontent = file.filecontent;
                return Content(Filecontent);
            }

            return NotFound(); // Если файл с заданным id не найден
        }


        // Действие для обработки создания папки
        [HttpPost]
        public IActionResult CreateFolder(ExplorerViewModel viewModel)
        {
            // Получаем имя новой папки из модели
            string folderName = viewModel.folders[0].foldername;

            // Получаем последний элемент
            var lastFolder = _context.folders.OrderByDescending(f => f.id).FirstOrDefault();

            // Создаем новую папку
            var newFolder = new Folders
            {
                foldername = folderName,
                // Устанавливаем свойство id на 1 больше последнего значения
                id = lastFolder != null ? lastFolder.id + 1 : 1,
                parentfolderid = 1
            };

            _context.folders.Add(newFolder);
            _context.SaveChanges();

            return RedirectToAction("Index");
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