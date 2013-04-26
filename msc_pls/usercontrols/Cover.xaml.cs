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

namespace msc_pls.usercontrols
{
    /// <summary>
    /// Interaktionslogik für Cover.xaml
    /// </summary>
    public partial class Cover : UserControl
    {
        public Cover()
        {
            InitializeComponent();
            effect = imageCover.Effect;
            imageCover.Effect = null;
        }

        private System.Windows.Media.Effects.Effect effect;
        private classes.Song song;

        public void setSong(classes.Song song)
        {
            this.song = song;
            labelTitle.Content = song.title;
         //* null durch 3 ersetzt//
            if(song.image.Length > 0)
                imageCover.Source = BitmapFrame.Create(song.image, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        }

        public classes.Song getSong()
        {
            return song;
        }

        private void UserControl_MouseEnter_1(object sender, MouseEventArgs e)
        {
            imageCover.Effect = effect;
        }

        private void UserControl_MouseLeave_1(object sender, MouseEventArgs e)
        {
            imageCover.Effect = null;
        }
    }
}
