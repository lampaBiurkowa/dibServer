using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace web2.Data
{
    class Uploader
    {
        const string RELATIVE_PATH_TO_DIBMD = ".dib/.dibmd";

        string appName;

        public void Upload(IFormFile file)
        {
            if (file.Length == 0)
                return;

            appName = Path.GetFileNameWithoutExtension(file.FileName);
            AppDirectoryData appDirectoryData = new AppDirectoryData(appName);
            appDirectoryData.IncreaseCurrentVersion();

            string filePath = appDirectoryData.GetAppDirectoryPath() + "/" + file.FileName;
            string dedicatedVersionRepoPath = appDirectoryData.GetCurrentVersionRepoPath();
            string masterRepoPath = appDirectoryData.GetMasterRepoPath();

            createRepo(dedicatedVersionRepoPath);
            saveUploadedRepoZip(file, filePath);
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

        void extractRepoZip(string filePath, string directoryPath)
        {
            string extractPath = directoryPath;
            ZipFile.ExtractToDirectory(filePath, extractPath);
            modifyDIBMDFile(directoryPath, AppVersionHandler.GetVersion(appName));
        }

        void modifyDIBMDFile(string repoPath, int version)
        {
            string path = $"{repoPath}/{RELATIVE_PATH_TO_DIBMD}"; 
            IntPtr DIBMDHandler = CreateDIBMDHandlerClass(path);
            SetRepoVersion(DIBMDHandler, version);
        }

        [DllImport("DIBMDHandler.dll")]
        static public extern IntPtr CreateDIBMDHandlerClass(string path);

        [DllImport("DIBMDHandler.dll")]
        static public extern int SetRepoVersion(IntPtr DIBMDHandlerObject, int version);

        void removeCompressedFile(string filePath)
        {
            File.Delete(filePath);
        }
    }
}