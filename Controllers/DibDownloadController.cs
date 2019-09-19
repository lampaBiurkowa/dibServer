using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.IO.Compression;
using web2.Data;

namespace web2.Controllers
{
    [Route("api/[controller]")]
    public class DibDownloadController : Controller
    {
        [HttpGet("{appName}")]
        public IActionResult DownladRepo(string appName)
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
        public IActionResult DownladRepo(string appName, int version)
        {
            AppDirectoryData appDirectoryData = new AppDirectoryData(appName);
            string appVersionRepoPath = appDirectoryData.GetVersionRepoPath(version);
            string repoZipName = $"{appName}.zip";
            string repoZipPath = $"{AppDirectoryData.PATH_TO_APP_DOWNLOADS}/{repoZipName}";

            FileStream fileStream = createZipInMemory(appVersionRepoPath, repoZipPath);
            return getResult(fileStream, repoZipName);
        }

        [HttpGet("{appName}/{targetVersion}/{clientVersion}")]
        public IActionResult DownladRepo(string appName, int targetVersion, int clientVersion)
        {
            UpdateDownloadBuilder.PrepareDownload(appName, clientVersion, targetVersion);
            return Ok(new {});
        }
    }
}