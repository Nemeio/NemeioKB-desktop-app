using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;

namespace CustomActions
{
    public static class CustomActions
    {
        private const string NemeioFolderName                               = "Nemeio";
        private const string NemeioProcessName                              = "Nemeio";

        [CustomAction]
        public static ActionResult CloseProcesses(Session session)
        {
            session.Log("Begin CloseProcesses");

            try
            {
                var processes = Process.GetProcessesByName(NemeioProcessName);
                foreach (var process in processes)
                {
                    session.Log($"CloseProcesses kill process <{process.Id}>");
                    process.Kill();
                }
            }
            catch (Exception exception)
            {
                session.Log($"CloseProcesses caught exception <{exception}>");
                throw;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RemoveScheduleTask(Session session)
        {
            session.Log("Begin RemoveScheduleTask");

            try
            {
                using (var ts = new TaskService())
                {
                    ts.RootFolder.DeleteTask(NemeioProcessName, false);
                }
            }
            catch(Exception exception)
            {
                session.Log($"RemoveScheduleTask caught exception <{exception}>");
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RemoveApplicationData(Session session)
        {
            session.Log("Begin RemoveApplicationData");

            try
            {
                //  Do not delete Nemeio folder on Common Application Data : "C:\ProgramData" which holds logs

                //  Delete Nemeio folder on Application Data all user : "C:\Users\[USER]\AppData\Roaming\Nemeio"

                var appDataFolders = GetAppDataFolderPathForAllUsersOfTheSystem();
                foreach (var appDataFolder in appDataFolders)
                {
                    Directory.Delete(
                        Path.Combine(
                            appDataFolder,
                            NemeioFolderName
                        ),
                        true
                    );
                }

            }
            catch (DirectoryNotFoundException exception)
            {
                session.Log($"Error RemoveApplicationData {exception.Message}");
            }

            //  In all case we want to return success
            //  Maybe user want to uninstall application just after install it, so "C:\ProgramData\Nemeio" does not exists

            return ActionResult.Success;
        }

        private static IEnumerable<string> GetAppDataFolderPathForAllUsersOfTheSystem()
        {
            const string appDataKey = @"HKEY_USERS\$SID$\Volatile Environment\";
            const string appDataValueName = "AppData";

            var userKeys = Registry.Users.GetSubKeyNames();

            foreach (var userKey in userKeys)
            {
                var key = appDataKey.Replace("$SID$", userKey);
                var appDataValue = Registry.GetValue(key, appDataValueName, null) as string;
                if (!string.IsNullOrWhiteSpace(appDataValue))
                {
                    yield return appDataValue;
                }
            }
        }

        [CustomAction]
        public static ActionResult RemoveLeftOverFiles(Session session)
        {
            session.Log("Begin RemoveLeftOverFiles");

            var folderPath = string.Empty;

            if (Environment.Is64BitOperatingSystem)
            {
                folderPath = Environment.ExpandEnvironmentVariables("%ProgramW6432%");
                session.Log($"RemoveLeftOverFiles 64bits detected : <{folderPath}>");
            }
            else
            {
                folderPath = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%");
                session.Log($"RemoveLeftOverFiles 86bits detected : <{folderPath}>");
            }

            folderPath = Path.Combine(folderPath, NemeioFolderName);

            try
            {
                foreach (var directory in Directory.GetDirectories(folderPath))
                {
                    session.Log($"\tTry to delete directory <{directory}> and child");
                    Directory.Delete(directory, true);
                    session.Log($"\t\tSuccess");
                }

                foreach (var file in Directory.GetFiles(folderPath))
                {
                    session.Log($"\tTry to delete file <{file}>");
                    File.Delete(file);
                    session.Log($"\t\tSuccess");
                }

                Directory.Delete(folderPath);
            }
            catch (Exception exception) when (exception is SecurityException || 
                                              exception is UnauthorizedAccessException || 
                                              exception is DirectoryNotFoundException)
            {
                session.Log($"Error RemoveLeftOverFiles {exception.Message}");
            }

            //  Main goal here is to clean "Nemeio" install directory
            //  But we wont stop uninstall if doesn't success
            //  So we always return "Success"

            return ActionResult.Success;
        }
    }
}
