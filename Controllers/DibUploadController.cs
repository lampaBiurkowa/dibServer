using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using web2.Data;

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
            
            string appName = Path.GetFileNameWithoutExtension(formFile.FileName);
            AppDirectoryData appDirectoryData = new AppDirectoryData(appName);
            string filePath = appDirectoryData.GetAppDirectoryPath() + "/" + formFile.FileName;
            string dedicatedVersionRepoPath = appDirectoryData.GetCurrentVersionRepoPath();
            string masterRepoPath = appDirectoryData.GetMasterRepoPath();

            createRepo(dedicatedVersionRepoPath);
            saveUploadedRepoZip(formFile, filePath);
            extractRepoZip(filePath, dedicatedVersionRepoPath);
            clearRepo(masterRepoPath);
            extractRepoZip(filePath, masterRepoPath);
            removeCompressedFile(filePath);

            appDirectoryData.IncreaseCurrentVersion();
        }

        void createRepo(string repoPath)
        {
            Directory.CreateDirectory(repoPath);
        }

        void clearRepo(string repoPath)
        {
            foreach (string filePath in Directory.GetFiles(repoPath))
                System.IO.File.Delete(filePath);
            foreach (string directoryPath in Directory.GetDirectories(repoPath))
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
