using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Media;
using System.Threading;
using WMPLib;

namespace msc_pls.classes
{
    public class Player
    {
        WMPLib.WindowsMediaPlayer player;
        Playlist playlist;
        int currentSongIndex = -1;
        Song currentSong;

        public bool setting_repeat = false;
        public bool setting_repeat_single = false;

        public Player(Playlist playlist)
        {
            // new player instance
            player = new WMPLib.WindowsMediaPlayer();
            player.settings.autoStart = false;

            // add playlist internal
            this.playlist = playlist;
            this.Next();
        }

        public classes.Playlist getPlaylist()
        {
            return playlist;
        }

        public bool isPlaying()
        {
            return player.playState == WMPLib.WMPPlayState.wmppsPlaying || player.playState == WMPLib.WMPPlayState.wmppsTransitioning;
        }

        public bool isPaused()
        {
            return player.playState == WMPLib.WMPPlayState.wmppsPaused;
        }

        public void Play()
        {
            try
            {
                if (currentSong != null && isPaused() == false)
                {
                    // assign event listener for play state changes
                    player.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(PlayStateChangeHandler);

                    // open file and play
                    player.URL = currentSong.path;
                    player.controls.play();
                }
                else if(currentSong != null && isPaused() == true)
                {
                    player.controls.play();
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public void PlaySong(classes.Song song)
        {
            currentSong = song;
            player.URL = currentSong.path;
            Play();
        }

        private void PlayStateChangeHandler(int NewState)
        {
            // when playing is stopped
            if (player.playState == WMPLib.WMPPlayState.wmppsStopped)
            {
                // if song was not the last in the playlist
                if (currentSongIndex != playlist.getSongs().Count - 1)
                {
                    if (!setting_repeat_single)
                    {
                        Next();
                    }
                    Play();

                }
                // last song? jump to first song and play again if setting is set
                else if(setting_repeat)
                {
                    JumpFirst();
                    Play();
                }
            }
        }

        public void Stop()
        {
            if (isPlaying())
            {
                player.PlayStateChange -= PlayStateChangeHandler;
                player.controls.stop();
            }
        }

        public void Pause()
        {
            if (isPlaying())
            {
                player.controls.pause();
            }
        }

        public void JumpFirst()
        {
            if (isPlaying())
            {
                // we have to stop the player to change the song
                Stop();
                JumpFirst();
                Play();
            }
            else
            {
                // reset index
                currentSongIndex = -1;
                Next();
            }
        }

        public void Next()
        {
            if (isPlaying())
            {
                // we have to stop the player to change the song
                Stop();
                Next();
                Play();
            }
            else
            {
                // load song information
                if (currentSongIndex != playlist.getSongs().Count - 1)
                {
                    currentSongIndex++;
                    currentSong = playlist.getSongs()[currentSongIndex];
                }
                else
                {
                    currentSong = null;
                }
            }
        }

        public void Prev()
        {
            if (isPlaying())
            {
                // we have to stop the player to change the song
                Stop();
                Prev();
                Play();
            }
            else
            {
                // load song information
                if (currentSongIndex != 0)
                {
                    currentSongIndex--;
                    currentSong = playlist.getSongs()[currentSongIndex];
                }
                else
                {
                    currentSong = null;
                }
            }
        }

        public Song getSong()
        {
            return currentSong;
        }

        public double pendingTime() 
        {
            return player.currentMedia.duration - player.controls.currentPosition;
        }
    }
}
