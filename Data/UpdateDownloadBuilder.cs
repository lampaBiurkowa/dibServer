using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;

namespace web2.Data
{
    static class UpdateDownloadBuilder
    {
        public static void PrepareDownload(string appName, int clientVersion, int targetVersion)
        {
            if (clientVersion < targetVersion)
                prepareUpdatePack(appName, clientVersion, targetVersion);
            else
                ;
        }

        static void prepareUpdatePack(string appName, int clientVersion, int targetVersion)
        {
            string clientRepoPath = getRepoPath(appName, clientVersion);
            string targetRepoPath = getRepoPath(appName, targetVersion);
            List<string> clientVersionPaths = getRepoPaths(appName, clientVersion);
            List<string> targetVersionPaths = getRepoPaths(appName, targetVersion);

            var removedFilesPaths = clientVersionPaths.Except(targetVersionPaths);
            var newFilesPaths = targetVersionPaths.Except(clientVersionPaths);
            List<string> modifiedFilesPaths = getModifiedFilesPaths(clientVersionPaths, targetVersionPaths, clientRepoPath, targetRepoPath);

            System.Console.WriteLine("REMOVED");
            foreach (string path in removedFilesPaths)
                System.Console.WriteLine(path);
            System.Console.WriteLine("NW");
            foreach (string path in newFilesPaths)
                System.Console.WriteLine(path);
            System.Console.WriteLine("MODIFIED");
            foreach (string path in modifiedFilesPaths)
                System.Console.WriteLine(path);
        }

        static string getRepoPath(string appName, int version)
        {
            AppDirectoryData appDirectoryData = new AppDirectoryData(appName);
            return appDirectoryData.GetVersionRepoPath(version);
        }

        static List<string> getRepoPaths(string appName, int version)
        {
            string repoPath = getRepoPath(appName, version);

            List<string> fullPaths = new List<string>(Directory.GetFiles(repoPath, "*", SearchOption.AllDirectories));
            List<string> output = new List<string>();
            foreach (string fullPath in fullPaths)
                output.Add(fullPath.Replace(repoPath, ""));

            return output;
        }

        static List<string> getModifiedFilesPaths(List<string> clientVersionPaths, List<string> targetVersionPaths, string clientRepoPath, string targetRepoPath)
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

        static bool areFilesSame(string path1, string path2)
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
    }
}