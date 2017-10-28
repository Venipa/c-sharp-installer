using System;
using System.Collections.Generic;
using System.IO;
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

namespace BiyoriInstaller.Content
{
    /// <summary>
    /// Interaktionslogik für Welcome.xaml
    /// </summary>
    public partial class Welcome : Page
    {
        public Welcome()
        {
            InitializeComponent();

            if (File.Exists(Utility.LocalPath + "\\data\\trailer.mp4"))
            {
                trailervideo.LoadedBehavior = MediaState.Manual;
                trailervideo.Source = new Uri(Utility.LocalPath + "\\data\\trailer.mp4");
                trailervideo.Volume = 0;
                trailervideo.IsMuted = true;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            trailervideo.Stop();
            this.NavigationService.Navigate(new Content.Install());
        }

        private void exit(object sender, RoutedEventArgs e)
        {
            if(Utility.Question("Do you really want to cancel the Setup?"))
            {
                Utility.Shutdown();
            }
        }
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            trailervideo.Play();
        }
    }
}
