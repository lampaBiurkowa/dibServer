using System.Collections.Generic;
using System.IO;

namespace web2.Data
{
    static class AppVersionHandler
    {
        const char SEPARATOR = ':';
        static readonly string VERSIONS_FILE_PATH = $"{AppDirectoryData.PATH_TO_APP_DIRECTORIES}/versions.ver";

        static AppVersionHandler()
        {
            if (!File.Exists(VERSIONS_FILE_PATH))
                File.WriteAllText(VERSIONS_FILE_PATH, "\0");
        }

        public static bool ContainsApp(string appName)
        {
            string[] entries = File.ReadAllLines(VERSIONS_FILE_PATH);
            foreach (string entry in entries)
            {
                string[] values = entry.Split(SEPARATOR);
                if (values.Length != 2)
                {
                    System.Console.WriteLine($"Warning: bad syntax of line {entry} in {VERSIONS_FILE_PATH} file");
                    continue;
                }

                string name = values[0];
                if (name == appName)
                    return true;
            }

            return false;
        }

        public static int GetVersion(string appName)
        {
            string[] entries = File.ReadAllLines(VERSIONS_FILE_PATH);
            foreach (string entry in entries)
            {
                string[] values = entry.Split(SEPARATOR);
                if (values.Length != 2)
                {
                    System.Console.WriteLine($"Warning: bad syntax of line {entry} in {VERSIONS_FILE_PATH} file");
                    continue;
                }

                string name = values[0];
                if (name == appName)
                    return int.Parse(values[1]);
            }

            System.Console.WriteLine($"Warning: app name not found in {VERSIONS_FILE_PATH}");
            return 0;
        }

        public static void SetVersion(string appName, int version)
        {
            List<string> entriesToSave = new List<string>();

            bool appEntryFound = false;
            string[] entries = File.ReadAllLines(VERSIONS_FILE_PATH);
            foreach (string entry in entries)
            {
                string[] values = entry.Split(SEPARATOR);
                if (values.Length != 2)
                {
                    System.Console.WriteLine($"Warning: bad syntax of line {entry} in {VERSIONS_FILE_PATH} file");
                    continue;
                }

                string name = values[0];
                if (name == appName)
                {
                    appEntryFound = true;
                    entriesToSave.Add($"{name}{SEPARATOR}{version}");
                }
                else
                    entriesToSave.Add(entry);
            }

            if (!appEntryFound)
                entriesToSave.Add($"{appName}{SEPARATOR}{version}");

            File.WriteAllLines(VERSIONS_FILE_PATH, entriesToSave.ToArray());
        }
    }
}