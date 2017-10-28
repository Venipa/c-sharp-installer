using Ookii.Dialogs.Wpf;
using SevenZip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BiyoriInstaller.Content
{
    /// <summary>
    /// Interaktionslogik für Install.xaml
    /// </summary>
    public partial class Install : Page
    {
        public MainWindow m = new MainWindow();
        public Install()
        {
            InitializeComponent();
            Init();
        }
        public void Init()
        {
        }

        private void browsepath_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Please select a destination for the Game.";
            dialog.UseDescriptionForTitle = true;
            if ((bool)dialog.ShowDialog(m))
                m.savepath = String.Format("{0}\\{1} - {2}", dialog.SelectedPath, Properties.Settings.Default.Website, Properties.Settings.Default.gameFolder);

            path.Content = m.savepath;
            long fsize = m.GetTotalFreeSpace(System.IO.Path.GetPathRoot(m.savepath));
            freeSpace.Content = String.Format(new FileSizeFormatProvider(), "{0:fs}", fsize);



            if (m.packedsize > fsize)
            {
                if (!Utility.Question("The Install size is bigger then your Free Space, do you want to continue?"))
                {
                    Utility.Shutdown();
                }
            }
            runGameCheck();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (m.savepath != null)
            {

                if (!Directory.Exists(m.folder))
                {
                    Directory.CreateDirectory(m.folder);
                }
            }
            else
            {
                if (Utility.Question("No Destination for the Game has been picked, do you want to pick one or do you want to use the Default Place to install it?"))
                {
                    browsepath_Click(sender, e);
                }
                else
                {
                    m.savepath = m.folder;
                }
            }
            try
            {
                extractTo();
            }
            catch (Exception ex)
            {
                Utility.Error("Something went wrong.");
                Utility.log(ex.StackTrace.ToString());
                Utility.Shutdown();
            }
        }

        public void extractTo()
        {
            if (!Directory.Exists(m.savepath))
                Directory.CreateDirectory(m.savepath);

            installbtn.IsEnabled = false;
            browsepath.IsEnabled = false;
            foreach (string data in m.files.Keys)
            {
                Task extractor = Task.Run(() =>
                {
                    m.arc = new SevenZipExtractor(data, Properties.Settings.Default.encryptKey);
                    string filename = System.IO.Path.GetFileName(data);
                    string after = filename;
                    string realfilename = "";
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        //Filelist.Items.Add(filename);
                        extractProgress.Value = 0;
                    }));
                    m.arc.FileExtractionStarted += (s, e) => {
                        Dispatcher.BeginInvoke((Action)(() => {
                            realfilename = e.FileInfo.FileName;
                            Filelist.Items.Insert(0, e.PercentDone + "% | " + realfilename);
                            extractingFile.Content = realfilename;

                        }));
                    };
                    m.arc.FileExtractionFinished += (s, e) =>
                    {
                        Dispatcher.BeginInvoke((Action)(() => {
                            realfilename = e.FileInfo.FileName;
                            Filelist.Items[0] = "Done " + realfilename;
                        }));
                    };
                    m.arc.Extracting += (s, e) =>
                    {
                        Dispatcher.BeginInvoke((Action)(() => {
                            int perc = e.PercentDone;
                            extractProgress.Value = perc;
                            percentage.Content = perc + "%";
                            /*
                            Filelist.Items.Remove(after);
                            after = perc + "% | " + filename;
                            Filelist.Items.Add(after);
                            extractingFile.Content = filename;*/
                        }));
                    };
                    m.arc.ExtractionFinished += (s, e) =>
                    {
                        Dispatcher.BeginInvoke((Action)(() => {
                            extractProgress.Value = 100;
                            percentage.Content = 100 + "%";
                            extractingFile.Content = "Finished Installing";
                            runGameCheck();
                        }));
                    };
                    m.arc.ExtractArchive(m.savepath);

                });
                if (extractor.IsFaulted)
                {
                    extractingFile.Content = "Error";
                    Utility.Error(String.Format("{0} has not been installed because an of Error.", Properties.Settings.Default.Game));
                }
                else if (extractor.IsCanceled)
                {
                    extractingFile.Content = "Canceled";
                    Utility.Information(String.Format("{0} has not been installed because it has been canceled.", Properties.Settings.Default.Game));
                }
            }
        }
        public void getFiles()
        {
            String dataFolder = String.Format("{0}{1}", Utility.LocalPath, Properties.Settings.Default.dataFolder);
            if (!Directory.Exists(dataFolder))
            {
                Utility.Error(String.Format("Could not find {0} Folder.", Properties.Settings.Default.dataFolder));
                Utility.Shutdown();
            }
            if (File.Exists(String.Format("{0}\\{1}", dataFolder, "data.biyori")))
            {
                String data = String.Format("{0}\\{1}", dataFolder, "data.biyori");

                m.packedsize = m.GetTotalUnzippedSize(data);
                m.files.Add(data, m.packedsize);
            }
            else
            {
                Utility.Error("No Data has been Found. Exiting...");
                Utility.Shutdown();
            }

            /*else if (File.Exists(String.Format("{0}\\{1}", dataFolder, "data.7z.001")))
            {
                String data = String.Format("{0}\\{1}", dataFolder, "data.7z.001");

                this.packedsize = GetTotalUnzippedSize(data);
                this.files.Add(data, this.packedsize);
            } else
            {
                foreach (object data in Directory.GetFiles(dataFolder, "data.7z.*"))
                {
                    if (data != null)
                    {
                        long splitsize = GetTotalUnzippedSize(data.ToString());
                        this.packedsize += splitsize;
                        this.files.Add(data, splitsize);
                    }
                }
        }*/
            Dispatcher.BeginInvoke(new Action((() =>
            {
                freeSpace.Content = String.Format(new FileSizeFormatProvider(), "{0:fs}", m.GetTotalFreeSpace(System.IO.Path.GetPathRoot(m.folder)));
                reqSpace.Content = String.Format(new FileSizeFormatProvider(), "{0:fs}", m.packedsize);
            })));


        }
        protected void runGameCheck()
        {
            string sfvpath = String.Format("{0}\\{1}", m.savepath, Properties.Settings.Default.sfvpath);
            if (File.Exists(sfvpath))
            {
                if (!File.Exists(String.Format("{0}\\{1}", m.savepath, Properties.Settings.Default.sfvDB)))
                {
                    Utility.Error(String.Format("Could not find File: {0}", Properties.Settings.Default.sfvDB));
                    Utility.Shutdown();
                }
                Task qscan = Task.Run(() =>
                {
                    Console.WriteLine("test");
                    Process quicksfv = new Process();
                    quicksfv.StartInfo.WorkingDirectory = m.savepath;
                    quicksfv.StartInfo.FileName = sfvpath;
                    quicksfv.StartInfo.Arguments = String.Format("{0} --check --percents", Properties.Settings.Default.sfvDB);
                    quicksfv.StartInfo.UseShellExecute = false;
                    quicksfv.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    quicksfv.StartInfo.CreateNoWindow = true;
                    quicksfv.StartInfo.RedirectStandardOutput = true;
                    quicksfv.EnableRaisingEvents = true;
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        Filelist.Items.Clear();
                    }));
                    Regex regPercentage = new Regex(@"/^ -?\$?[0 - 9] *\.?([0 - 9]{ 2})?%/");
                    quicksfv.OutputDataReceived += (s, e) =>
                    {
                        if(e.Data != null)
                        {
                            String o = e.Data;
                            if (regPercentage.IsMatch(e.Data))
                        {
                            Dispatcher.BeginInvoke((Action)(() => {
                                Filelist.Items.RemoveAt(0);
                                Filelist.Items.Insert(0, o);
                            }));
                        }
                        else
                        {
                            o = o.Replace("\u0009", "");
                            o = "OK - " + o.Replace("OK", "");
                            Dispatcher.BeginInvoke((Action)(() => {

                                Filelist.Items.Insert(0, o);
                            }));
                        }
                        }


                    };
                    quicksfv.Exited += (s, e) =>
                    {
                        //Utility.Information(String.Format("{0} has been installed successfully.", Properties.Settings.Default.Game));
                    };
                    quicksfv.ErrorDataReceived += (s, e) =>
                    {
                        Dispatcher.BeginInvoke((Action)(() => {
                            Console.WriteLine(e.Data);
                        }));
                    };
                    m.killProcessesCI();
                    quicksfv.Start();
                    quicksfv.BeginOutputReadLine();
                    //quicksfv.BeginErrorReadLine();

                });
            }
            else
            {
                Utility.Information(String.Format("{0} has been installed successfully but {1} could not be found, so we could not check the Integrity of the Files.", Properties.Settings.Default.Game, Properties.Settings.Default.sfvpath));
            }


        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            getFiles();
            m.killProcessesCI();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            m.killProcessesCI();
        }
    }
}
