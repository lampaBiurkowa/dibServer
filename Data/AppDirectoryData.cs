using System.IO;

namespace web2.Data
{
    class AppDirectoryData
    {
        const string MASTER_REPO_ID = "master";
        public const string PATH_TO_APP_DIRECTORIES = "AppRepos";
        public const string PATH_TO_APP_DOWNLOADS = "AppDownloads";

        string appName;

        static AppDirectoryData()
        {
            if (!Directory.Exists(PATH_TO_APP_DIRECTORIES))
                Directory.CreateDirectory(PATH_TO_APP_DIRECTORIES);

            if (!Directory.Exists(PATH_TO_APP_DOWNLOADS))
                Directory.CreateDirectory(PATH_TO_APP_DOWNLOADS);
        }

        public AppDirectoryData(string appName)
        {
            this.appName = appName;

            const int INIT_VERSION = -1;
            if (!AppVersionHandler.ContainsApp(appName))
            {
                AppVersionHandler.SetVersion(appName, INIT_VERSION);
                Directory.CreateDirectory(GetAppDirectoryPath());
                Directory.CreateDirectory(GetMasterRepoPath());
            }
        }

        public string GetAppDirectoryPath()
        {
            return $"{PATH_TO_APP_DIRECTORIES}/{appName}";
        }

        public string GetCurrentVersionRepoPath()
        {
            return $"{PATH_TO_APP_DIRECTORIES}/{appName}/{AppVersionHandler.GetVersion(appName)}";
        }

        public string GetMasterRepoPath()
        {
            return $"{PATH_TO_APP_DIRECTORIES}/{appName}/{MASTER_REPO_ID}";
        }

        public string GetVersionRepoPath(int version)
        {
            return $"{PATH_TO_APP_DIRECTORIES}/{appName}/{version}";
        }

        public void IncreaseCurrentVersion()
        {
            int currentVersion = AppVersionHandler.GetVersion(appName);
            AppVersionHandler.SetVersion(appName, currentVersion + 1);
        }
    }
}