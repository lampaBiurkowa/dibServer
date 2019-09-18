using System.Collections.Generic;
using System.IO;

namespace web2.Data
{
    class AppDirectoryData
    {
        const string MASTER_REPO_ID = "master";
        const string PATH_TO_APP_DIRECTORIES = "AppRepos";
        static Dictionary<string, int> currentVersions = new Dictionary<string, int>();

        string appName;

        static AppDirectoryData()
        {
            if (!Directory.Exists(PATH_TO_APP_DIRECTORIES))
                Directory.CreateDirectory(PATH_TO_APP_DIRECTORIES);
        }

        public AppDirectoryData(string appName)
        {
            this.appName = appName;
            if (!currentVersions.ContainsKey(appName))
            {
                currentVersions.Add(appName, 0);
                Directory.CreateDirectory(GetAppDirectoryPath());
                Directory.CreateDirectory(GetMasterRepoPath());
            }
        }

        public void IncreaseCurrentVersion()
        {
            currentVersions[appName]++;
        }

        public string GetAppDirectoryPath()
        {
            return $"{PATH_TO_APP_DIRECTORIES}/{appName}";
        }

        public string GetCurrentVersionRepoPath()
        {
            return $"{PATH_TO_APP_DIRECTORIES}/{appName}/{currentVersions[appName]}";
        }

        public string GetMasterRepoPath()
        {
            return $"{PATH_TO_APP_DIRECTORIES}/{appName}/{MASTER_REPO_ID}";
        }
    }
}