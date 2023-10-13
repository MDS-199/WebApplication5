using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Models
{
    public class Folders
    {
        public int id { get; set; }

        [Key]
        public string foldername { get; set; }
        public string parentfoldername { get; set; }

    }

    public class Files
    {
        [Key]
        public int id { get; set; }

        public string filename { get; set; }

        public string filedescription { get; set; }

        public string filetypecode { get; set; }

        public int foldercode { get; set; }

        public string filecontent { get; set; }

    }


    public class FilesExtensions
    {
        [Key]
        public int id { get; set; }

        public string filetype { get; set; }

        public string fileicon { get; set; }
    }


    public class ExplorerDbContext : DbContext
    {
        public DbSet<Folders> folders { get; set; }

        public DbSet<Files> files { get; set; }

        public DbSet<FilesExtensions> filesextensions { get; set; }


        public ExplorerDbContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=WebAppExplorer2;Username=postgres;Password=_17_AquaLifer_16");
        }
    }
}