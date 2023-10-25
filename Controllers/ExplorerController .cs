using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApplication5.Models;

namespace WebApplication5.Controllers
{
    public class ExplorerController : Controller
    {



        private readonly ExplorerDbContext _context = new();


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


        //Просмотр содержимого файла
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


        // Создание папки
        [HttpPost]
        public IActionResult CreateFolder(ExplorerViewModel viewModel)
        {
            // Получаем имя новой папки из модели
            string folderName = viewModel.folders[0].foldername;

            // Получаем id последней папки
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

        // Удаление папки
        [HttpPost]
        public IActionResult DeleteFolder(int id)
        {

            _context.folders.Where(f => f.id == id).ExecuteDelete();

            return RedirectToAction("Index");
        }

        // Переименование папки
        [HttpPost]
        public IActionResult RenameFolder(int id, string newName)
        {
            var folder = _context.folders.FirstOrDefault(f => f.id == id);
            if (folder != null)
            {
                folder.foldername = newName;
                _context.SaveChanges(); // Сохраните изменения в базе данных
                return RedirectToAction("Index"); // Перенаправяет пользователя обратно на страницу списка папок
            }
            else
            {
                // Если папка с указанным ID не найдена
                return RedirectToAction("Error"); // Перенаправляет пользователя на страницу с ошибкой
            }
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