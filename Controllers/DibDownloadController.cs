using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.IO.Compression;
using web2.Data;

namespace web2.Controllers
{
    [Route("api/[controller]")]
    public class DibDownloadController : Controller
    {
        [HttpGet("{appName}")]
        public IActionResult DownladMasterRepo(string appName)
        {
            AppDirectoryData appDirectoryData = new AppDirectoryData(appName);
            string appMasterRepoPath = appDirectoryData.GetMasterRepoPath();
            string repoZipName = $"{appName}.zip";
            string repoZipPath = $"{AppDirectoryData.PATH_TO_APP_DOWNLOADS}/{repoZipName}";

            FileStream fileStream = createZipInMemory(appMasterRepoPath, repoZipPath);
            return getResult(fileStream, repoZipName);
        }

        FileStreamResult getResult(FileStream fileStream, string repoZipName)
        {
            var contentType = "APPLICATION/octet-stream";
            return File(fileStream, contentType, repoZipName);
        }

        FileStream createZipInMemory(string repoPath, string repoZipPath)
        {
            ZipFile.CreateFromDirectory(repoPath, repoZipPath);
            FileStream fileStream = new FileStream(repoZipPath, FileMode.Open, FileAccess.Read, FileShare.Delete);
            System.IO.File.Delete(repoZipPath);

            return fileStream;
        }

        [HttpGet("{appName}/{version}")]
        public IActionResult DownladVersionRepo(string appName, int version)
        {
            AppDirectoryData appDirectoryData = new AppDirectoryData(appName);
            string appVersionRepoPath = appDirectoryData.GetVersionRepoPath(version);
            string repoZipName = $"{appName}.zip";
            string repoZipPath = $"{AppDirectoryData.PATH_TO_APP_DOWNLOADS}/{repoZipName}";

            FileStream fileStream = createZipInMemory(appVersionRepoPath, repoZipPath);
            return getResult(fileStream, repoZipName);
        }

        [HttpGet("{appName}/{targetVersion}/{clientVersion}")]
        public IActionResult DownladUpdateToVersionRepo(string appName, int targetVersion, int clientVersion)
        {
            string repoZipName = $"{appName}.zip";

            UpdateDownloadBuilder updateDownloadBuilder = new UpdateDownloadBuilder();
            FileStream fileStream = updateDownloadBuilder.GetUpdatePack(appName, clientVersion, targetVersion);

            return getResult(fileStream, repoZipName);
        }

        [HttpGet("{appName}/master/{clientVersion}")]
        public IActionResult DownladUpdateToMasterRepo(string appName, int clientVersion)
        {
            string repoZipName = $"{appName}.zip";

            UpdateDownloadBuilder updateDownloadBuilder = new UpdateDownloadBuilder();
            FileStream fileStream = updateDownloadBuilder.GetUpdatePack(appName, clientVersion);

            return getResult(fileStream, repoZipName);
        }
    }
}