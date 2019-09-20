using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using web2.Data;

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
            appDirectoryData.IncreaseCurrentVersion();

            string filePath = appDirectoryData.GetAppDirectoryPath() + "/" + formFile.FileName;
            string dedicatedVersionRepoPath = appDirectoryData.GetCurrentVersionRepoPath();
            string masterRepoPath = appDirectoryData.GetMasterRepoPath();

            createRepo(dedicatedVersionRepoPath);
            saveUploadedRepoZip(formFile, filePath);
            modifyDIBMDFile(AppVersionHandler.GetVersion(appName));
            extractRepoZip(filePath, dedicatedVersionRepoPath);
            clearRepo(masterRepoPath);
            extractRepoZip(filePath, masterRepoPath);
            removeCompressedFile(filePath);
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

        [DllImport("DIBMDHandler.dll")]
        static public extern IntPtr CreateDIBMDHandlerClass(string path);

        [DllImport("DIBMDHandler.dll")]
        static public extern int SetRepoVersion(IntPtr DIBMDHandlerObject, int version);

        void modifyDIBMDFile(int version)
        {
            string pathToDIBMD = ".dib/.dibmd";
            IntPtr DIBMDHandler = CreateDIBMDHandlerClass(pathToDIBMD);
            SetRepoVersion(DIBMDHandler, version);
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
