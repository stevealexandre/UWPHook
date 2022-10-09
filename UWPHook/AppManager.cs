using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using UWPHook.Properties;

namespace UWPHook
{
    /// <summary>
    /// Functions to manage UWP apps
    /// </summary>
    static class AppManager
    {
        private static int? runningProcessId;
        private static string executablePath;

        /// <summary>
        /// Launch a UWP App using a ApplicationActivationManager and sets a internal id to launched proccess id
        /// </summary>
        /// <param name="aumid">The AUMID of the app to launch</param>
        public static void LaunchUWPApp(string[] args)
        {
            // We receive the args from Steam, 
            // 0 is application location, 
            // 1 is the aumid,
            // 2 is the executable, the rest are extras
            string aumid = args[1];
            executablePath = args[2].Contains("/") ? args[2].Replace('/', '\\') : args[2];
            Log.Verbose("Arguments => " + String.Join("/", args));

            string extra_args = String.Join(" ", args.Skip(3).Take(args.Length - 3).Select(eachElement => eachElement.Clone()).ToArray());

            try
            {
                Process process = new Process();
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.UseShellExecute = true;
                processStartInfo.FileName = @"shell:appsFolder\" + aumid;
                process.StartInfo = processStartInfo;
                process.Start();

                //Bring the launched app to the foreground, this fixes in-home streaming
                BringProcess();
            }
            catch (Exception e)
            {
                Log.Error("Error while trying to launch your app." + Environment.NewLine + e.Message);
                throw new Exception("Error while trying to launch your app." + Environment.NewLine + e.Message);
            }
        }

        /// <summary>
        /// Checks if the launched app is running
        /// </summary>
        /// <returns>True if the perviously launched app is running, false otherwise</returns>
        public static Boolean IsRunning()
        {
            runningProcessId = getProcess();
            if (runningProcessId != null && runningProcessId > 0) return true;
            return false;
        }

        /// <summary>
        /// Find process path with their dedicated pid
        /// </summary>
        /// <returns>Map of processes to their path and pid. Any process in this object may have already terminated</returns>
        private static Dictionary<string, (string Path, int Pid)> GetProcess()
        {
            var result = new Dictionary<string, (string Path, int Pid)>();

            using (var searcher = new ManagementObjectSearcher("select processid, Name, ExecutablePath from win32_process"))
            {
                foreach (var process in searcher.Get())
                {
                    string processName = Convert.ToString(process.Properties["Name"].Value);
                    int processId = Convert.ToInt32(process.Properties["processid"].Value);
                    string processPath = Convert.ToString(process.Properties["ExecutablePath"].Value);

                    if (String.IsNullOrWhiteSpace(processName) || result.ContainsKey(processName)) continue;

                    result.Add(processName, (processPath, processId));
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a list of installed UWP Apps on the system, containing each app name + AUMID + executable path, separated by '|' 
        /// </summary>
        /// <returns>List of installed UWP Apps</returns>
        public static List<String> GetInstalledApps()
        {
            List<String> result = null;
            var assembly = Assembly.GetExecutingAssembly();
            //Load the powershell script to get installed apps
            var resourceName = "UWPHook.Resources.GetAUMIDScript.ps1";
            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        //Every entry is listed separated by ;
                        result = ScriptManager.RunScript(reader.ReadToEnd()).Split(';').ToList<string>();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Error trying to get installed apps on your PC " + Environment.NewLine + e.Message, e.InnerException);
                throw new Exception("Error trying to get installed apps on your PC " + Environment.NewLine + e.Message, e.InnerException);
            }

            return result;
        }

        [DllImport("user32.dll")]
        private static extern
        bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern
        bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern
        bool IsIconic(IntPtr hWnd);

        public static void BringProcess()
        {
            /*
            const int SW_HIDE = 0;
            const int SW_SHOWNORMAL = 1;
            const int SW_SHOWMINIMIZED = 2;
            const int SW_SHOWMAXIMIZED = 3;
            const int SW_SHOWNOACTIVATE = 4;
            const int SW_RESTORE = 9;
            const int SW_SHOWDEFAULT = 10;
            */

            if (runningProcessId != null && runningProcessId > 0)
            {
                Process process = Process.GetProcessById((int)runningProcessId);

                // get the window handle
                IntPtr hWnd = process.MainWindowHandle;

                // if iconic, we need to restore the window
                if (IsIconic(hWnd))
                {
                    ShowWindowAsync(hWnd, 3);
                }

                // bring it to the foreground
                SetForegroundWindow(hWnd);
            }
        }

        private static int? getProcess()
        {
            bool secondCheck = false;

            do
            {
                // Handle process running by some launcher by checking their executable path and name
                var processes = GetProcess();
                foreach (var process in processes)
                {
                    string executableFile = executablePath.Contains('\\') ? executablePath.Substring(executablePath.LastIndexOf('\\') + 1) : executablePath;
                    Log.Verbose("Process " + process.Value.Path + " contains " + executablePath + " ? : " + process.Value.Path.Contains(executablePath).ToString());
                    Log.Verbose("Process " + process.Key + " contains " + executableFile + " ? : " + process.Key.Contains(executableFile).ToString());
                    if (process.Value.Path.Contains(executablePath) || process.Key.Contains(executableFile))
                    {
                        return process.Value.Pid;
                    }
                }

                secondCheck = !secondCheck;

                if (secondCheck)
                {
                    Log.Debug("Process has not been found. Last chance to find it !");
                    Thread.Sleep(Settings.Default.Seconds * 5000);
                }
            } while (secondCheck);

            Log.Debug("Process is not running.");
            return null;
        }
    }
}
