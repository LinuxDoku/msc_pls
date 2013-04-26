using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace msc_pls.classes
{
    public class Playlist
    {
        private int id;
        private SQLiteConnection db;
        public String title { get; set; }
        List<Song> songs = new List<Song>();

        public Playlist(object identifier)
        {
            db = Library.getSQLiteConnection();

            // get playlist by title
            if(identifier.GetType() == typeof(String)) {
                SQLiteCommand command = new SQLiteCommand(db);
                command.CommandText = "SELECT id FROM playlists WHERE title = @title LIMIT 1";
                command.Parameters.AddWithValue("title", identifier.ToString());
                SQLiteDataReader playlist = command.ExecuteReader();
                playlist.Read();
                try
                {
                    id = Convert.ToInt32(playlist[0]);
                }
                catch
                {
                    throw new Exception();
                }
                playlist.Dispose();
                command.Dispose();
                title = identifier.ToString();
            }
            
            // get playlist by id
            else if(identifier.GetType() == typeof(int)) {
                id = Convert.ToInt32(identifier);
                SQLiteCommand command = new SQLiteCommand(db);
                command.CommandText = "SELECT title FROM playlists WHERE id = @id";
                command.Parameters.AddWithValue("id", id);
                SQLiteDataReader playlist = command.ExecuteReader();
                playlist.Read();
                try
                {
                    title = playlist[0].ToString();
                }
                catch
                {
                    throw new Exception();
                }
                playlist.Dispose();
                command.Dispose();
            }

            // load songs
            if (id > 0)
            {
                SQLiteCommand commandSongs = new SQLiteCommand(db);
                commandSongs.CommandText = "SELECT songPath FROM playlist_songs WHERE playlistID = @id";
                commandSongs.Parameters.AddWithValue("id", id);
                SQLiteDataReader song = commandSongs.ExecuteReader();
                while (song.Read())
                {
                    songs.Add(new Song(song[0].ToString()));
                }
                song.Dispose();
                commandSongs.Dispose();
            }
        }

        public int getId()
        {
            return id;
        }

        public List<Song> getSongs()
        {
            return songs;
        }

        public void addSong(Song song)
        {
            // dont add a song twice
            if (songs.Find(s => s.path == song.path) == null)
            {
                songs.Add(song);

                // add to database
                SQLiteCommand command = new SQLiteCommand(db);
                command.CommandText = "INSERT INTO playlist_songs (playlistID, songPath) VALUES (@id, @path)";
                command.Parameters.AddWithValue("id", id);
                command.Parameters.AddWithValue("path", song.path);
                command.ExecuteNonQuery();
                command.Dispose();
            }
        }

        public void removeSong(Song song)
        {
            songs.Remove(song);

            // remove from database
            SQLiteCommand command = new SQLiteCommand(db);
            command.CommandText = "DELETE FROM playlist_songs WHERE playlistID = @id songPath = @path";
            command.Parameters.AddWithValue("id", id);
            command.Parameters.AddWithValue("path", song.path);
            command.ExecuteNonQuery();
            command.Dispose();
        }

        public void clear()
        {
            songs.Clear();
            
            // remove from database
            SQLiteCommand command = new SQLiteCommand(db);
            command.CommandText = "DELETE FROM playlist_songs WHERE playlistID = @id";
            command.Parameters.AddWithValue("id", id);
            command.ExecuteNonQuery();
            command.Dispose();
        }
    }
}