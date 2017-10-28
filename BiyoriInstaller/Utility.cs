using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace BiyoriInstaller
{
    class Utility
    {
        public static String LocalPath = String.Format("{0}\\", Environment.CurrentDirectory);

        public static void Information(String Message, params Object[] Arguments)
        {
            String FormattedMessage = String.Format(Message, Arguments);

            MessageBox.Show(FormattedMessage, Properties.Settings.Default.title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void Error(String Message, params Object[] Arguments)
        {
            String FormattedMessage = String.Format(Message, Arguments);

            if (FormattedMessage.Contains(Properties.Settings.Default.encryptKey)) { FormattedMessage = FormattedMessage.Replace(Properties.Settings.Default.encryptKey, "<snip>"); }

            MessageBox.Show(FormattedMessage, Properties.Settings.Default.title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static Boolean Question(String Message, params Object[] Arguments)
        {
            String FormattedMessage = String.Format(Message, Arguments);

            MessageBoxResult Result = MessageBox.Show(FormattedMessage, Properties.Settings.Default.title, MessageBoxButton.YesNo, MessageBoxImage.Information);

            if (Result == MessageBoxResult.Yes) { return true; }

            return false;
        }
        public static void Shutdown()
        {
            Application.Current.Shutdown();
        }
        public static void log(string g)
        {
            String log = LocalPath + "debug.log";
            if(!File.Exists(log))
            {
                File.Create(log);
            }
            if(g.Contains(Properties.Settings.Default.encryptKey))
            {
                g.Replace(Properties.Settings.Default.encryptKey, "<snip>");
            }
            using (StreamWriter Writer = new StreamWriter(log)) { Writer.WriteLine(g); Writer.Close(); }
        }
        private static long GetTotalFreeSpace(string driveName)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == driveName)
                {
                    return drive.TotalFreeSpace;
                }
            }
            return -1;
        }
        internal static void Restart()
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Utility.Shutdown();
        }
        public static String readUri(String url)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(new Uri(url));
            StreamReader reader = new StreamReader(stream);
            String content = reader.ReadToEnd();
            return content;
        }
        public static bool IsProcessOpen(string name)
        {
            //here we're going to get a list of all running processes on
            //the computer
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(name))
                {
                    return true;
                }
            }
            //otherwise we return a false
            return false;
        }
        public static Process getProcess(string name)
        {
            //here we're going to get a list of all running processes on
            //the computer
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(name))
                {
                    return clsProcess;
                }
            }
            return null;
        }

    }
}
