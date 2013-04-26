using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.ComponentModel;
using System.IO;

namespace msc_pls
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private String activePage;
        private pages.PageMain pageMain;
        private pages.PageSettings pageSettings;

        public classes.Library library;
        private BackgroundWorker worker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();

            // restore last session settings
            this.Height = Properties.Settings.Default.WindowHeight;
            this.Width = Properties.Settings.Default.WindowWidth;

            // open main page
            pageMain = new pages.PageMain(this);
            this.Content = pageMain;
            activePage = "Main";

            // start importing library
            worker.WorkerReportsProgress = true;
            worker.DoWork += libraryStart;
            worker.RunWorkerCompleted += libraryStartCompleted;
            worker.RunWorkerAsync();
        }

        private void libraryStart(object sender, DoWorkEventArgs e)
        {
            library = null;
            GC.Collect();
            library = new classes.Library();

            // load music files from the current users library
            String homeDirectory = System.Environment.GetEnvironmentVariable("USERPROFILE");
            library.loadDirectory(homeDirectory + "/Music/");

            // finished
            worker.ReportProgress(100);
        }

        private void libraryStartCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // get added songs and place in window
            List<classes.Song> songs = library.getSongs();
            pageMain.progressIndicatorSetActive(false);
            pageMain.addSongs(songs);

            // render menu
            pageMain.renderMenu();
        }

        private void buttonSettings_Click(object sender, RoutedEventArgs e)
        {
            // switch window content between library and settings view
            if (activePage == "Main")
            {
                if (pageSettings == null)
                    pageSettings = new pages.PageSettings();
                this.Content = pageSettings;
                activePage = "Settings";
                buttonSettings.Content = "library";
            }
            else
            {
                if (pageMain == null)
                    pageMain = new pages.PageMain(this);
                this.Content = pageMain;
                activePage = "Main";
                buttonSettings.Content = "settings";
            }
        }

        private void MetroWindow_Closing_1(object sender, CancelEventArgs e)
        {;
            // save settings
            Properties.Settings.Default.WindowHeight = this.Height;
            Properties.Settings.Default.WindowWidth = this.Width;
            Properties.Settings.Default.Save();
        }
    }
}
