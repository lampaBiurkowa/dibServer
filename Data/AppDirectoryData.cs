using System.IO;

namespace web2.Data
{
    class AppDirectoryData
    {
        const string MASTER_REPO_ID = "master";
        public const string PATH_TO_APP_DIRECTORIES = "AppRepos";

        string appName;

        static AppDirectoryData()
        {
            if (!Directory.Exists(PATH_TO_APP_DIRECTORIES))
                Directory.CreateDirectory(PATH_TO_APP_DIRECTORIES);
        }

        public AppDirectoryData(string appName)
        {
            this.appName = appName;
            if (!AppVersionHandler.ContainsApp(appName))
            {
                AppVersionHandler.SetVersion(appName, 0);
                Directory.CreateDirectory(GetAppDirectoryPath());
                Directory.CreateDirectory(GetMasterRepoPath());
            }
        }

        public void IncreaseCurrentVersion()
        {
            int currentVersion = AppVersionHandler.GetVersion(appName);
            AppVersionHandler.SetVersion(appName, currentVersion + 1);
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
    }
}