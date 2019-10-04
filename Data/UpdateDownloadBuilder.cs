using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;

namespace web2.Data
{
    class UpdateDownloadBuilder
    {
        const string DIB_DIRECTORY_RELATIVE_PATH = ".dib";
        const string DIBRM_FILE_RELATIVE_PATH = DIB_DIRECTORY_RELATIVE_PATH + "/.dibrm";

        string appName;
        string clientRepoPath;
        string targetRepoPath;
        int clientVersion;
        int targetVersion;
        string clientGuid;

        public FileStream GetUpdatePack(string appName, int clientVersion)
        {
            return GetUpdatePack(appName, clientVersion, AppVersionHandler.GetVersion(appName));
        }

        public FileStream GetUpdatePack(string appName, int clientVersion, int targetVersion)
        {
            this.appName = appName;
            this.clientVersion = clientVersion;
            this.targetVersion = targetVersion;
            clientRepoPath = getRepoPath(clientVersion);
            targetRepoPath = getRepoPath(targetVersion);

            generateClientGuid();
            prepareUpdatePack();

            FileStream updatePackStream = new FileStream(getZipPath(), FileMode.Open, FileAccess.Read, FileShare.Delete);
            File.Delete(getZipPath());
            return updatePackStream;
        }

        string getRepoPath(int version)
        {
            AppDirectoryData appDirectoryData = new AppDirectoryData(appName);
            return appDirectoryData.GetVersionRepoPath(version);
        }

        void prepareUpdatePack()
        {
            List<string> clientVersionPaths = getRepoPaths(clientVersion);
            List<string> targetVersionPaths = getRepoPaths(targetVersion);

            List<string> removedFilesPaths = new List<string>(clientVersionPaths.Except(targetVersionPaths));
            List<string> newFilesPaths = new List<string>(targetVersionPaths.Except(clientVersionPaths));
            List<string> modifiedFilesPaths = getModifiedFilesPaths(clientVersionPaths, targetVersionPaths);

            prepareDownloadDirectory(modifiedFilesPaths, newFilesPaths, removedFilesPaths);
            compressUpdatePack();
        }

        List<string> getRepoPaths(int version)
        {
            string repoPath = getRepoPath(version);

            List<string> fullPaths = new List<string>(Directory.GetFiles(repoPath, "*", SearchOption.AllDirectories));
            List<string> output = new List<string>();
            foreach (string fullPath in fullPaths)
                output.Add(fullPath.Replace(repoPath, ""));

            return output;
        }

        List<string> getModifiedFilesPaths(List<string> clientVersionPaths, List<string> targetVersionPaths)
        {
            List<string> modifiedFilesPaths = new List<string>();

            const int NOT_FOUND_ID = -1;
            foreach (string clientVersionPath in clientVersionPaths)
            {
                int index = targetVersionPaths.IndexOf(clientVersionPath);
                if (index == NOT_FOUND_ID)
                    continue;

                if (!areFilesSame($"{clientRepoPath}/{clientVersionPath}", $"{targetRepoPath}/{targetVersionPaths[index]}"))
                    modifiedFilesPaths.Add(clientVersionPath);
            }

            return modifiedFilesPaths;
        }

        bool areFilesSame(string path1, string path2)
        {
            string hash1;
            string hash2;

            using (MD5 md5 = MD5.Create())
            {
                using (FileStream stream = File.OpenRead(path1))
                    hash1 = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                using (FileStream stream = File.OpenRead(path2))
                    hash2 = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
            }

            return hash1 == hash2;
        }

        void prepareDownloadDirectory(List<string> modifiedFilesPaths, List<string> newFilesPaths, List<string> removedFilesPaths)
        {
            createDownloadDirectory();
            createDIBRMFile(removedFilesPaths);
            addFilesToSend(modifiedFilesPaths, newFilesPaths);
        }

        void generateClientGuid()
        {
            string guid = Guid.NewGuid().ToString();
            while (Directory.Exists($"{AppDirectoryData.PATH_TO_APP_DOWNLOADS}/{guid}"))
                guid = Guid.NewGuid().ToString();

            clientGuid = guid;
        }

        void createDownloadDirectory()
        {
            Directory.CreateDirectory(getDownloadDirectoryPath());
        }

        string getDownloadDirectoryPath()
        {
            return $"{AppDirectoryData.PATH_TO_APP_DOWNLOADS}/{clientGuid}";
        }

        void createDIBRMFile(List<string> removedFilesPaths)
        {
            createDIBDirectory();
            string DIBRMFilePath = $"{getDownloadDirectoryPath()}/{DIBRM_FILE_RELATIVE_PATH}";
            File.WriteAllLines(DIBRMFilePath, removedFilesPaths);
        }

        void createDIBDirectory()
        {
            Directory.CreateDirectory($"{getDownloadDirectoryPath()}/{DIB_DIRECTORY_RELATIVE_PATH}");
        }

        void addFilesToSend(List<string> modifiedFilesPaths, List<string> newFilesPaths)
        {
            moveFilesToDownloadDirectory(modifiedFilesPaths, targetRepoPath);
            moveFilesToDownloadDirectory(newFilesPaths, targetRepoPath);
        }

        void moveFilesToDownloadDirectory(List<string> paths, string sourceDirectoryPath)
        {
            foreach (string path in paths)
            {
                string pathInDownloadDirectory = $"{getDownloadDirectoryPath()}/{path}";
                createDirectoryFromPath(Path.GetFullPath(Path.GetDirectoryName(pathInDownloadDirectory)));
                File.Copy($"{sourceDirectoryPath}/{path}", pathInDownloadDirectory);
            }
        }

        void createDirectoryFromPath(string path)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                createDirectoryFromPath(Path.GetDirectoryName(path));

            Directory.CreateDirectory(path);
        }

        void compressUpdatePack()
        {
            ZipFile.CreateFromDirectory(getDownloadDirectoryPath(), $"{AppDirectoryData.PATH_TO_APP_DOWNLOADS}/{clientGuid}.zip");
        }

        string getZipPath()
        {
            return $"{AppDirectoryData.PATH_TO_APP_DOWNLOADS}/{clientGuid}.zip";
        }
    }
}