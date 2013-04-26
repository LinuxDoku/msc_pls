using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace msc_pls_l33t
{
    class Program
    {
        msc_pls.classes.Library library;
        msc_pls.classes.Player player;
        /**
     * static entry point
         */
        static void Main(string[] args)
        {
            new Program().init().commands();
        }

        /**
         * init player core
         */
        public Program init()
        {
            Console.WriteLine("   init msc_pls_l33t...");
            Console.WriteLine("   load music library...");

            // open music library
            this.library = new msc_pls.classes.Library();

            // load music files from the current users library
            String homeDirectory = System.Environment.GetEnvironmentVariable("USERPROFILE");
            library.loadDirectory(homeDirectory + "/Music/");

            Console.WriteLine("   msc_pls_l33t r3ady");

            return this;
        }

        /**
         * commands for player functions
         */
        public void commands()
        {
            String[] command;
            Console.WriteLine();
            Console.Write(" > ");
            command = Console.ReadLine().Split(' ');
            Console.WriteLine();
            if (command[0] == "help")
            {
                Console.WriteLine("   n00b commands:");
                Console.WriteLine();
                Console.WriteLine("   list       | list current found songs");
                Console.WriteLine("   find       | find a song in the library");
                Console.WriteLine("   play       | start playing the resultset or all songs");
                Console.WriteLine("   stop       | stop playing");
                Console.WriteLine("   next       | play the next song");
                Console.WriteLine("   prev       | play the previous song");
                Console.WriteLine("   repeat     | configure repeating");
                Console.WriteLine("   exit       | exit player");
                Console.WriteLine();
                Console.WriteLine("   g33k commands:");
                Console.WriteLine();
                Console.WriteLine("   lists      | show all playlists");
                Console.WriteLine("   createlist | create a new playlist");
                Console.WriteLine("   viewlist   | show songs of a playlist");
                Console.WriteLine("   clearlist  | remove all songs from a playlist");
                Console.WriteLine("   deletelist | delete a playlist");
                Console.WriteLine();
                Console.WriteLine("   l33t commands:");
                Console.WriteLine();
                Console.WriteLine("   cleardb    | clear current music library database");
                Console.WriteLine("   adddir     | add directory to the current music library");
                commands();
            }
            else if (command[0] == "list")
            {
                Console.WriteLine("   Title - Artist");
                Console.WriteLine("   ---------------------------------");
                List<msc_pls.classes.Song> songs;
                if (command.Length == 1)
                {
                    songs = this.library.getSongs();
                }
                else
                {
                    songs = this.library.searchSongs(command[1]);
                }
                foreach (msc_pls.classes.Song song in songs)
                {
                    Console.WriteLine("   " + song.title + " - " + song.artist);
                }
                commands();
            }
            else if (command[0] == "count")
            {
                Console.WriteLine("   curr3nt songs in db: "+library.getSongs().Count);
                commands();
            }
            else if (command[0] == "find")
            {
                if (command.Length == 1)
                {
                    Console.WriteLine("   U need to enter a searchstring! find <searchstring>");
                }
                else
                {
                    try
                    {
                        Console.WriteLine("   Title - Artist");
                        Console.WriteLine("   ---------------------------------");
                        List<msc_pls.classes.Song> songs;

                        String searchstring;
                        if (command[command.Length - 2] == "add")
                        {
                            searchstring = String.Join(" ", command, 1, command.Length - 3);
                            songs = this.library.searchSongs(searchstring);
                            msc_pls.classes.Playlist playlist;
                            try
                            {
                                playlist = new msc_pls.classes.Playlist(command[command.Length - 1]);
                                foreach (msc_pls.classes.Song song in songs)
                                    playlist.addSong(song);
                            }
                            catch (Exception e)
                            {
                                
                            }
                        }
                        else
                        {
                            searchstring = String.Join(" ", command, 1, command.Length - 1);
                            songs = this.library.searchSongs(searchstring);
                        }
                        foreach (msc_pls.classes.Song song in songs)
                        {
                            Console.WriteLine("   " + song.title + " - " + song.artist);
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("   Whops, s33ms your searchstring is invalid!");
                    }
                }
                commands();
            }
            else if (command[0] == "play")
            {
                if (command.Length == 2)
                {
                    player = new msc_pls.classes.Player(new msc_pls.classes.Playlist(command[1]));
                    player.Play();
                    msc_pls.classes.Song song = player.getSong();
                    if (song != null)
                    {
                        Console.WriteLine("   Now playing: " + song.artist + " - " + song.title);
                    }
                    else
                    {
                        Console.WriteLine("   Playlist is 3mpty!");
                    }
                }
                else if(command.Length == 1)
                {
                    if (player != null)
                    {
                        if (!player.isPlaying())
                        {
                            if (!player.isPaused() && player.getSong() == null)
                            {
                                player.JumpFirst();
                            }
                            player.Play();
                            msc_pls.classes.Song song = player.getSong();
                            Console.WriteLine("   Now playing: " + song.artist + " - " + song.title);
                        }
                        else
                        {
                            Console.WriteLine("   Listen, I am allr3ady playing!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("   Gimme a playlist pls!");
                    }
                }
                commands();
            }
            else if (command[0] == "stop")
            {
                player.Stop();
                Console.WriteLine("   kk, I'm silent!");
                commands();
            }
            else if (command[0] == "current")
            {
                if (player != null && player.isPlaying())
                {
                    msc_pls.classes.Song song = player.getSong();
                    Console.WriteLine("   Now playing: " + song.artist + " - " + song.title);
                }
                else
                {
                    Console.WriteLine("   Nothing to play!");
                }
                commands();
            }
            else if (command[0] == "next")
            {
                if (player != null)
                {
                    player.Next();
                    if (player.isPlaying())
                    {
                        Console.WriteLine("   Yay, hope this one rocks!");
                        msc_pls.classes.Song song = player.getSong();
                        Console.WriteLine("   Now playing: " + song.artist + " - " + song.title);
                    }
                    else
                    {
                        Console.WriteLine("   Playlist ended, gimme a new one!");
                    }
                }
                commands();
            }
            else if (command[0] == "prev")
            {
                if (player != null)
                {
                    player.Prev();
                    if (player.isPlaying())
                    {
                        Console.WriteLine("   Okay, and again!");
                        msc_pls.classes.Song song = player.getSong();
                        Console.WriteLine("   Now playing: " + song.artist + " - " + song.title);
                    }
                    else
                    {
                        Console.WriteLine("   3rr, nothing to play!");
                    }
                }
                commands();
            }
            else if (command[0] == "repeat")
            {
                if (command.Length == 2 && player != null && player.isPlaying())
                {
                    if (command[1] == "none")
                    {
                        player.setting_repeat = false;
                        player.setting_repeat_single = false;
                    }
                    else if (command[1] == "all")
                    {
                        player.setting_repeat = true;
                        player.setting_repeat_single = false;
                    }
                    else if (command[1] == "single")
                    {
                        player.setting_repeat = false;
                        player.setting_repeat_single = true;
                    }
                    else if (command[1] == "both")
                    {
                        player.setting_repeat = true;
                        player.setting_repeat_single = true;
                    }
                }
                else
                {
                    if (!player.isPlaying())
                    {
                        Console.WriteLine("   Pls start playing first!");
                        Console.WriteLine();
                    }    
                    Console.WriteLine("   Current Setting:");
                    Console.WriteLine("   playlist:\t "+player.setting_repeat.ToString());
                    Console.WriteLine("   single:\t "+player.setting_repeat_single.ToString());                
                }
                commands();
            }
            else if (command[0] == "lists")
            {
                List<msc_pls.classes.Playlist> playlists = library.getAllPlaylists();
                Console.WriteLine("   Title\tSongs");
                Console.WriteLine("   ---------------------------------");
                foreach (msc_pls.classes.Playlist playlist in playlists)
                {
                    Console.WriteLine("   " + playlist.title + " \t" + playlist.getSongs().Count);
                }
                commands();
            }
            else if (command[0] == "createlist")
            {
                if (command[1] != "")
                {
                    library.createPlaylist(command[1].ToString());
                }
                else
                {
                    Console.WriteLine("   Gimme a name for this list!");
                }
                commands();
            }
            else if (command[0] == "clearlist")
            {
                if (command[1] != "")
                {
                    msc_pls.classes.Playlist playlist = new msc_pls.classes.Playlist(command[1]);
                    playlist.clear();
                    Console.WriteLine("   playlist cl3ared bro!");
                }
                commands();
            }
            else if (command[0] == "deletelist")
            {
                if (command[1] != "")
                {
                    library.deletePlaylist(command[1].ToString());
                    Console.WriteLine("   playlist was d3l3ated!");
                    Console.WriteLine();
                }
                commands();
            }
            else if (command[0] == "viewlist")
            {
                if (command.Length > 1)
                {
                    msc_pls.classes.Playlist playlist = new msc_pls.classes.Playlist(command[1]);
                    Console.WriteLine("   Title - Artist");
                    Console.WriteLine("   ---------------------------------");
                    foreach (msc_pls.classes.Song song in playlist.getSongs())
                    {
                        Console.WriteLine("   " + song.title + " - " + song.artist);
                    }
                }
                else
                {
                    Console.WriteLine("   Hey, give us a playlist to l00k!");
                }
                commands();
            }
            else if (command[0] == "exit")
            {
                // no internal route, close app
            }
            else if (command[0] == "cleardb")
            {
                Console.WriteLine("   clear complete database... (this could take a few minutes)");
                library.dropCompleteDatabase();
                library.initDatabase();
                Console.WriteLine("   clearing database done!");
                commands();
            }
            else if (command[0] == "adddir")
            {
                try
                {
                    String dir = String.Join(" ", command, 1, command.Length - 1);
                    Console.WriteLine("   adding directory \"" + dir + "\" to ur music database...");
                    if (library.loadDirectory(dir))
                    {
                        Console.WriteLine("   adding done, have fun listening!");
                    }
                    else
                    {
                        Console.WriteLine("   path \"" + dir + "\" doesn't exit on ur system pls check?!");
                    }
                }
                catch
                {
                    Console.WriteLine("   pls give a dir on your system!");
                }
                commands();
            }
            else
            {
                Console.WriteLine("   invalid command: \"" + command[0] + "\", pls see \"help\"");
                commands();
            }
        }
    }
}
