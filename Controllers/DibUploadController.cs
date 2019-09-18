using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace web2.Controllers
{
    [Route("api/[controller]")]
    public class DibUploadController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            foreach (IFormFile formFile in files)
                handleSingleFileUpload(formFile);

            return Ok(new { count = files.Count, size });
        }

        void handleSingleFileUpload(IFormFile formFile)
        {
            if (formFile.Length == 0)
                return;
            
            string filePath = Path.GetFileNameWithoutExtension(formFile.FileName) + "/" + formFile.FileName;
            string directoryPath = Path.GetDirectoryName(filePath);

            createDirectoryIfDoesntExist(directoryPath);
            clearDirectory(directoryPath);
            saveUploadedRepoZip(formFile, filePath);
            extractRepoZip(filePath, directoryPath);
            removeCompressedFile(filePath);
        }

        void createDirectoryIfDoesntExist(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
        }

        void clearDirectory(string consideredDirectoryPath)
        {
            foreach (string filePath in Directory.GetFiles(consideredDirectoryPath))
                System.IO.File.Delete(filePath);
            foreach (string directoryPath in Directory.GetDirectories(consideredDirectoryPath))
                Directory.Delete(directoryPath, true);
        }

        void saveUploadedRepoZip(IFormFile formFile, string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
                formFile.CopyTo(stream);
        }

        void extractRepoZip(string filePath, string directoryPath)
        {
            string extractPath = directoryPath;
            ZipFile.ExtractToDirectory(filePath, extractPath);
        }

        void removeCompressedFile(string filePath)
        {
            System.IO.File.Delete(filePath);
        }
    }
}
