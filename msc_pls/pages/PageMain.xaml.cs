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
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace msc_pls.pages
{
    /// <summary>
    /// Interaktionslogik für PageMain.xaml
    /// </summary>
    public partial class PageMain : Page
    {
        private MainWindow MainWindow;
        private classes.Playlist currentPlaylist;
        private classes.Player player;
        private bool searchEnabled = true;
        private DispatcherTimer durationTimer;

        public PageMain(MainWindow MainWindow)
        {
            InitializeComponent();

            this.MainWindow = MainWindow;

            // plugin event
            App.Plugins.Run("OpenMainWindow", new object[] { this });
        }

        public void progressIndicatorSetActive(bool status)
        {
            this.progressIndicator.IsActive = status;
        }

        public void clearSongs()
        {
            wrapPanelLibrary.Children.RemoveRange(0, wrapPanelLibrary.Children.Count);
        }

        public void addSongs(List<classes.Song> songs)
        {
            foreach (classes.Song song in songs)
                addSong(song);
        }

        public void addSong(classes.Song song)
        {
            usercontrols.Cover cover = new usercontrols.Cover();
            cover.setSong(song);
            cover.Width = 150;
            cover.Height = 150;
            cover.PreviewMouseLeftButtonDown += cover_PreviewMouseLeftButtonDown;
            cover.PreviewMouseMove += cover_PreviewMouseMove;
            cover.PreviewMouseDoubleClick += cover_PreviewMouseDoubleClick;
            wrapPanelLibrary.Children.Add(cover);
        }

        private Point coverMoveStartPoint;
        private bool coverIsDragging = false;

        void cover_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !coverIsDragging)
            {
                Point position = e.GetPosition(null);
                if (Math.Abs(position.X - coverMoveStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - coverMoveStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    coverIsDragging = true;
                    usercontrols.Cover cover = (usercontrols.Cover)sender;
                    DataObject data = new DataObject(typeof(classes.Song), cover.getSong());
                    DragDropEffects de = DragDrop.DoDragDrop(cover, data, DragDropEffects.Copy);
                    coverIsDragging = false;
                }
            }
        }

        void cover_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            coverMoveStartPoint = e.GetPosition(null);
        }

        private void playlistMenuItem_Drop(object sender, DragEventArgs e)
        {
            classes.Playlist playlist = (classes.Playlist)(sender as ListBoxItem).DataContext;
            classes.Song song = (classes.Song)e.Data.GetData("msc_pls.classes.Song");
            playlist.addSong(song);
        }

        public void showAllSongs()
        {
            foreach (usercontrols.Cover cover in wrapPanelLibrary.Children)
            {
                cover.Visibility = Visibility.Visible;
            }
        }

        public void hideAllSongs()
        {
            foreach (usercontrols.Cover cover in wrapPanelLibrary.Children)
            {
                cover.Visibility = Visibility.Collapsed;
            }
        }

        public void showOnlyThisSongs(List<classes.Song> songs)
        {
            foreach (usercontrols.Cover cover in wrapPanelLibrary.Children)
            {
                if (songs.Find(i => i.path == cover.getSong().path) == null)
                {
                    cover.Visibility = Visibility.Collapsed;
                }
                else
                {
                    cover.Visibility = Visibility.Visible;
                }
            }
        }

        public void renderMenu()
        {
            playlistMenu.ItemsSource = MainWindow.library.getAllPlaylists();
        }

        private void mainMenuAllSongs_Selected(object sender, RoutedEventArgs e)
        {
            playlistMenu.UnselectAll();
            showAllSongs();
        }

        private void mainMenuCreatePlaylist_Selected(object sender, RoutedEventArgs e)
        {
            searchEnabled = false;
            CreateNewPlaylist.Visibility = Visibility.Visible;
            NewPlaylistName.Focus();
        }

        private void CreateNewPlaylistSubmit_Click(object sender, RoutedEventArgs e)
        {
            String title = NewPlaylistName.Text.Replace(' ', '-');
            if (title == "")
            {
                NewPlaylistName.Focus();
            } 
            // title is valid
            else {
                MainWindow.library.createPlaylist(title);
                renderMenu();
                CreateNewPlaylist.Visibility = Visibility.Collapsed;
                NewPlaylistName.Text = "";
                searchEnabled = true;
                playlistMenu.UnselectAll();
            }
        }

        private void CreateNewPlaylistClose_Click(object sender, RoutedEventArgs e)
        {
            mainMenu.UnselectAll();
            CreateNewPlaylist.Visibility = Visibility.Collapsed;
            NewPlaylistName.Text = "";
            searchEnabled = true;
        }

        private void playlistMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (playlistMenu.SelectedItem != null)
            {
                mainMenu.UnselectAll();
                currentPlaylist = (classes.Playlist)playlistMenu.SelectedItem;
                showOnlyThisSongs(currentPlaylist.getSongs());
            }
        }

        void cover_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (currentPlaylist != null)
            {
                classes.Song song = ((usercontrols.Cover)sender).getSong();
                if (song != null)
                {
                    if (player == null)
                        player = new classes.Player(currentPlaylist);
                    player.PlaySong(song);
                    updateCurrentSongDetails();
                    ButtonPause.Visibility = Visibility.Visible;
                    ButtonPlay.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            // if a playlist is loaded
            if (currentPlaylist != null)
            {
                if (player != null && currentPlaylist == player.getPlaylist())
                {
                    player.Play();
                    ButtonPause.Visibility = Visibility.Visible;
                    ButtonPlay.Visibility = Visibility.Collapsed;
                }
                // load new playlist
                else
                {
                    if (player != null && player.isPlaying())
                    {
                        player.Stop();
                    }
                    player = new classes.Player(currentPlaylist);
                    player.Play();
                    ButtonPause.Visibility = Visibility.Visible;
                    ButtonPlay.Visibility = Visibility.Collapsed;
                }
                updateCurrentSongDetails();
            }
        }

        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            if (player != null && player.isPlaying())
            {
                player.Pause();
            }
            ButtonPlay.Visibility = Visibility.Visible;
            ButtonPause.Visibility = Visibility.Collapsed;
            updateCurrentSongDetails();
        }

        private void ButtonPrev_Click(object sender, RoutedEventArgs e)
        {
            if (player != null)
            {
                player.Prev();
            }
            updateCurrentSongDetails();
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            if (player != null)
            {
                player.Next();
            }
            updateCurrentSongDetails();
        }

        private void updateCurrentSongDetails()
        {
            classes.Song song = player.getSong();
            if (song != null)
            {
                if (song.image.Length > 0)
                {
                    CurrentSongCoverImage.Source = BitmapFrame.Create(song.image, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                }
                else
                {
                    BitmapImage cover = new BitmapImage();
                    cover.BeginInit();
                    cover.UriSource = new Uri("pack://application:,,,/msc_pls;component/static/images/cover.png");
                    cover.EndInit();
                    CurrentSongCoverImage.Source = cover;
                }
                CurrentSongTitle.Content = song.artist + " - " + song.title;
                CurrentSongAlbum.Content = song.album;
                startDurationCounter();
            }
            else
            {
                if(durationTimer != null)
                    durationTimer.Stop();
            }
        }

        private void startDurationCounter()
        {
            if (durationTimer != null)
                durationTimer.Stop();
            durationTimer = new DispatcherTimer();
            durationTimer.Tick += new EventHandler(durationTimer_Tick);
            durationTimer.Interval = new TimeSpan(0, 0, 0, 1);
            durationTimer.Start();
        }

        private void durationTimer_Tick(object sender, EventArgs e)
        {
            CurrentSongPendingTime.Content = string.Format("{0:F2}", player.pendingTime() / 60).Replace(',', ':');
        }

        private void deletePlaylist_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currentPlaylist != null)
            {
                MainWindow.library.deletePlaylist(currentPlaylist.getId());
                renderMenu();
                mainMenu.SelectedItem = mainMenu.Items[0];
            }
        }
    }
}
