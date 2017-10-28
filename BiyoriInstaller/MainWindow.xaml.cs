using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using BiyoriInstaller.Properties;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System.IO;
using BiyoriInstaller.Files;
using Ionic.Zip;
using SevenZip;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BiyoriInstaller
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public String savepath = null;
        public String prgFiles = BaseFIFO.prgfiles();
        public String folder = String.Format("{0}\\{1}\\{2}", BaseFIFO.prgfiles(), Properties.Settings.Default.Website, Properties.Settings.Default.gameFolder);
        public FileSizeFormatProvider ff = new FileSizeFormatProvider();
        public Dictionary<object, long> files = new Dictionary<object, long>();
        public SevenZipExtractor arc;
        public long packedsize = 0;
        
        public MainWindow()
        {
            Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
            this.Title = Properties.Settings.Default.title;
            InitializeComponent();
        }
        
        public void Init(object sender,RoutedEventArgs ev)
        {
            this.savepath = folder;
            killProcessesCI();
            //getFiles();
            this.Closing += (s, e) =>
            {
                killProcessesCI();
            };

            FrameWrap.Unloaded += (s, e) =>
            {
                loader.IsBusy = true;
            };
            FrameWrap.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            FrameWrap.Navigate(new Content.Welcome());

            if (File.Exists(Utility.LocalPath + "\\data\\music.mp3"))
            {
                bgaudio.LoadedBehavior = MediaState.Manual;
                bgaudio.Source = new Uri(Utility.LocalPath + "\\data\\music.mp3");
                bgaudio.Volume = 0.2;
                bgaudio.Play();
            }

        }

        public void killProcessesCI()
        {

            if (Utility.IsProcessOpen("checkfiles"))
            {
                Process openQSFV = Utility.getProcess("checkfiles");
                openQSFV.Kill();
            }
        }

        private void Frame_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Utility.Shutdown();
        }


        

        
        
        public long GetTotalFreeSpace(string driveName)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == driveName)
                {
                    return drive.TotalFreeSpace;
                }
            }
            return 0;
        }
        public long GetTotalUnzippedSize(string zipFileName)
        {
            SevenZipExtractor f = new SevenZipExtractor(zipFileName, Properties.Settings.Default.encryptKey);
            ulong space = 0;
            foreach (ArchiveFileInfo l in f.ArchiveFileData)
            {
                space += l.Size;
            }
            return (long)space;
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }
        private Uri runPage(string path)
        {
            Uri p = new Uri(@path, UriKind.RelativeOrAbsolute);
            return p;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if(bgaudio.HasAudio)
            {
                if (e.Key == Key.M)
                {
                    bgaudio.IsMuted = bgaudio.IsMuted ? false : true;
                }
                if (e.Key == Key.Down)
                {
                    bgaudio.Volume -= 0.1;
                }
                if (e.Key == Key.Up)
                {
                    bgaudio.Volume += 0.1;
                }
                if (e.Key == Key.N)
                {
                    bgaudio.Pause();
                }
                if (e.Key == Key.B)
                {
                    bgaudio.Play();
                }
            }
        }

        private void navBack(object sender, RoutedEventArgs e)
        {
            if(FrameWrap.CanGoBack)
            {
                FrameWrap.NavigationService.GoBack();
            }
        }

        private void FrameWrap_Loaded(object sender, RoutedEventArgs e)
        {
            loader.IsBusy = false;
        }
    }
}
