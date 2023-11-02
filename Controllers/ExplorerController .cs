using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql.BackendMessages;
using System.Diagnostics;
using System.Net.Mime;
using System.Text;
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
                byte[] bytes = Convert.FromBase64String(file.filecontent);
                string decodedString = Encoding.UTF8.GetString(bytes);
                return Content(decodedString);
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

            bool DoesFolderNameExist(string folderName)
            {
                return _context.folders.Any(folder => folder.foldername == folderName);
            }

            if (DoesFolderNameExist(newName))
            {
                TempData["ErrorMessage"] = "Папка с таким именем уже существует.";
                return RedirectToAction("Index");
            }

            var folder = _context.folders.FirstOrDefault(f => f.id == id);
            if (folder != null)
            {
                folder.foldername = newName;
                _context.SaveChanges(); // Сохрание изменения в базе данных
                return RedirectToAction("Index"); // Перенаправяет пользователя обратно на страницу списка папок
            }
            else
            {
                // Если папка с указанным ID не найдена
                return RedirectToAction("Error"); // Перенаправляет пользователя на страницу с ошибкой
            }
        }

        // Функция загрузки файла
        [HttpPost]
        public IActionResult UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                // Обработка случая, когда файл не выбран
                return RedirectToAction("Index");
            }

            if (file != null && file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())

                {
                file.CopyTo(memoryStream);
                var fileName = file.FileName;
                var fileExtension = Path.GetExtension(fileName);

                // Получаем id последнего файла
                var lastFile = _context.files.OrderByDescending(f => f.id).FirstOrDefault();

                // Переменная времени для добавления в описание
                var currentDate = DateTime.Now;

                // Сохранение файла в базу данных:
                var fileData = new Files
                    {
                        id = lastFile != null ? lastFile.id + 1 : 1,
                        filename = fileName,
                        filedescription = "Дата добавления: " + currentDate.ToString("dd/MM/yyyy HH:mm:ss"),
                        filetypecode = Path.GetExtension(fileName),
                        foldercode = 1,
                        filecontent = Convert.ToBase64String(memoryStream.ToArray()) // Конвертируем в Base64 строку
                    };


                    _context.files.Add(fileData);
                    _context.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return View("Error");
        }

        // Функция удаления файла
        [HttpPost]
        public IActionResult DeleteFile(int id)
        {
            _context.files.Where(f => f.id == id).ExecuteDelete();

            return RedirectToAction("Index");
        }

        // Функция выгрузки файла
        public IActionResult DownloadFile(int id)
        {
            var file = _context.files.FirstOrDefault(f => f.id == id);

            if (file != null)
            {
                var contentDisposition = new ContentDisposition
                {
                    FileName = file.filename + file.filetypecode,
                    Inline = false,
                };
                Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

                var contentType = "application/octet-stream";


                return File(Encoding.UTF8.GetBytes(file.filecontent), contentType, file.filename + file.filetypecode);
            }

            else
            {
                return NotFound(); // Если файл с указанным id не найден
            }
        }

        // Переименование файла
        [HttpPost]
        public IActionResult RenameFile(int id, string newName)
        {
            bool DoesFileNameExist(string fileName)
            {
                return _context.files.Any(file => file.filename == fileName);
            }

            if (DoesFileNameExist(newName))
            {
                TempData["ErrorMessage"] = "Файл с таким именем уже существует.";
                return RedirectToAction("Index");
            }

            var file = _context.files.FirstOrDefault(f => f.id == id);
            if (file != null)
            {
                file.filename = newName;
                _context.SaveChanges(); // Сохранение изменения в базе данных
                return RedirectToAction("Index"); // Перенаправяет пользователя обратно на страницу списка папок
            }
            else
            {
                // Если папка с указанным ID не найдена
                return RedirectToAction("Error"); // Перенаправляет пользователя на страницу с ошибкой
            }
        }

        // Отображение расширений
        public IActionResult ShowExtensions()
        {
            var viewModel = new ExplorerViewModel
            {
                files = _context.files.ToList(),
                folders = _context.folders.ToList(),
                fileextensions = _context.filesextensions.ToList()
            };

            return View(viewModel);
        }

        // Создание расширения
        [HttpPost]
        public IActionResult CreateExtension(ExplorerViewModel viewModel)
        {
            // Получаем имя новой папки из модели
            string extensionName = viewModel.fileextensions[0].filetype;

            // Получаем id последнего расширения
            var lastExtension = _context.filesextensions.OrderByDescending(f => f.id).FirstOrDefault();

            // Создаем новую папку
            var newExtension = new FilesExtensions
            {
                filetype = extensionName,
                // Устанавливаем свойство id на 1 больше последнего значения
                id = lastExtension != null ? lastExtension.id + 1 : 1,
                fileicon = "1"
            };

            _context.filesextensions.Add(newExtension);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Удаление расширения
        [HttpPost]
        public IActionResult DeleteExtension(int id)
        {

            _context.filesextensions.Where(f => f.id == id).ExecuteDelete();

            return RedirectToAction("ShowExtensions");
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