using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SQLite;
using System.Drawing;
using Id3;

namespace msc_pls.classes
{
    public class Library
    {
        public static string databaseFile = "library.db";
        private SQLiteConnection db;

        public Library()
        {
            db = Library.getSQLiteConnection();

            initDatabase();
        }

        public static SQLiteConnection getSQLiteConnection()
        {
            // load database
            SQLiteConnection db = new SQLiteConnection();
            db.ConnectionString = "Data Source=" + Library.databaseFile;
            db.Open();
            return db;
        }

        public void dropSongDatabase()
        {
            SQLiteCommand command = new SQLiteCommand(db);
            command.CommandText = "DROP TABLE IF EXISTS songs;";
            command.ExecuteNonQuery();
            command.Dispose();
        }

        public void dropCompleteDatabase()
        {
            SQLiteCommand command = new SQLiteCommand(db);
            command.CommandText = "DROP TABLE IF EXISTS songs; DROP TABLE IF EXISTS playlists; DROP TABLE IF EXISTS playlist_songs; DROP TABLE IF EXISTS directories;";
            command.ExecuteNonQuery();
            command.Dispose();
        }

        public void initDatabase()
        {
            SQLiteCommand command = new SQLiteCommand(db);

            // create song table if not exists
            command.CommandText = "CREATE TABLE IF NOT EXISTS songs (title VARCHAR(255) NOT NULL, artist VARCHAR(255), album VARCHAR(255), duration VARCHAR(255), path VARCHAR(260) NOT NULL, image TEXT, hash VARCHAR(100));";
            command.ExecuteNonQuery();
            
            // create playlist table if not exists
            command.CommandText = "CREATE TABLE IF NOT EXISTS playlists (id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE, title VARCHAR(255) NOT NULL);";
            command.ExecuteNonQuery();

            // create playlist songs association table if not exists
            command.CommandText = "CREATE TABLE IF NOT EXISTS playlist_songs (playlistID INT NOT NULL, songPath VARCHAR(260) NOT NULL);";
            command.ExecuteNonQuery();

            // create directory table if not exists
            command.CommandText = "CREATE TABLE IF NOT EXISTS directories (directory VARCHAR(260) NOT NULL, recursive INT NOT NULL);";
            command.ExecuteNonQuery();

            command.Dispose();
        }

        public bool loadDirectory(string path, bool recursive=true) 
        {
            if (Directory.Exists(path))
            {
                SQLiteCommand command;
                command = new SQLiteCommand(db);

                if (recursive)
                {
                    // get sub directories
                    DirectoryInfo directoryInfo = new DirectoryInfo(path);
                    DirectoryInfo[] subDirectories = directoryInfo.GetDirectories();
                    foreach (DirectoryInfo subDirectory in subDirectories)
                    {
                        loadDirectory(subDirectory.FullName);
                    }
                }

                // read directory
                string[] files = Directory.GetFiles(path, "*.mp3");
                foreach (string file in files)
                {
                    // get hash for duplicate detection
                    FileInfo info = new FileInfo(file);
                    long fileSize = info.Length;
                    String hash = helper.md5.getHash(info.Name + fileSize.ToString());

                    // check if song allready exists in database
                    command.CommandText = "SELECT hash FROM songs WHERE hash = '" + hash + "'";

                    SQLiteDataReader reader = command.ExecuteReader();
                    if (reader.HasRows == false)
                    {
                        reader.Dispose();

                        String title = info.Name;
                        String artist = "";
                        String album = "";
                        String image = "";
                        float duration = 0;

                        // get id3 tag information
                        try
                        {
                            var mp3 = new Mp3File(file);
                            Id3Tag tag = mp3.GetTag(Id3TagFamily.FileStartTag);
                            if (tag != null)
                            {

                                // get image
                                if (tag.Pictures.Count > 0)
                                {
                                    MemoryStream pictureStream = new MemoryStream(tag.Pictures[0].PictureData);
                                    byte[] byteArray = pictureStream.ToArray();
                                    image = Convert.ToBase64String(byteArray);
                                }

                                // prepare information
                                if (tag.Title.Value != null)
                                    title = tag.Title.Value;
                                else
                                    title = info.Name;
                                artist = tag.Artists.Value;
                                album = tag.Album.Value;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                        // write to database
                        command.CommandText = "INSERT INTO songs (`title`, `artist`, `album`, `duration`, `path`, `image`, `hash`) VALUES (@title, @artist, @album, @duration, @path, @image, @hash)";
                        command.Parameters.AddWithValue("title", title);
                        command.Parameters.AddWithValue("artist", artist);
                        command.Parameters.AddWithValue("album", album);
                        command.Parameters.AddWithValue("duration", duration);
                        command.Parameters.AddWithValue("path", file);
                        command.Parameters.AddWithValue("image", image);
                        command.Parameters.AddWithValue("hash", hash);
                        command.ExecuteNonQuery();
                    }
                    if (!reader.IsClosed)
                        reader.Dispose();
                }
                command.Dispose();
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<Song> getSongs() {
            List<Song> list = new List<Song>();
            SQLiteCommand command = new SQLiteCommand(db);
            command.CommandText = "SELECT path FROM songs";
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Song song = new Song(reader[0].ToString());
                list.Add(song);
            }
            reader.Dispose();
            command.Dispose();
            return list;
        }

        public List<Song> searchSongs(String text, bool title=true, bool artist=true, bool album=true)
        {
            String where = "";

            text = text.Replace("\"", "");

            if (title && artist && album)
                where = "WHERE title || ' ' || artist || ' ' || album LIKE '%" + text + "%'";
            else if(title && artist)
                where = "WHERE title || ' ' || artist LIKE '%" + text + "%'";

            List<Song> list = new List<Song>();
            SQLiteCommand command = new SQLiteCommand(db);
            command.CommandText = "SELECT path FROM songs " + where;
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Song song = new Song(reader[0].ToString());
                list.Add(song);
            }
            reader.Dispose();
            command.Dispose();
            return list;
        }

        public List<Playlist> getAllPlaylists()
        {
            SQLiteCommand command = new SQLiteCommand(db);
            command.CommandText = "SELECT id FROM playlists";
            SQLiteDataReader playlist = command.ExecuteReader();
            List<Playlist> playlists = new List<Playlist>();
            while (playlist.Read())
            {
                if(playlist["id"] != null)
                    playlists.Add(new Playlist(Convert.ToInt32(playlist["id"])));
            }
            playlist.Dispose();
            command.Dispose();
            return playlists;
        }

        public Playlist createPlaylist(String title)
        {
            SQLiteCommand command = new SQLiteCommand(db);
            command.CommandText = "INSERT INTO playlists (title) VALUES (@title)";
            command.Parameters.AddWithValue("title", title);
            command.ExecuteNonQuery();
            command.Dispose();
            command = new SQLiteCommand(db);
            command.CommandText = "SELECT last_insert_rowid()";
            int id = Convert.ToInt32(command.ExecuteScalar());
            command.Dispose();
            return new Playlist(id);
        }

        public void deletePlaylist(object identifier)
        {
            db = Library.getSQLiteConnection();

            String id = "";
            // get playlist by name
            if (identifier.GetType() == typeof(String))
            {
                SQLiteCommand command = new SQLiteCommand(db);
                command.CommandText = "SELECT id FROM playlists WHERE title = @title;";
                command.Parameters.AddWithValue("title", identifier.ToString());
                SQLiteDataReader playlist = command.ExecuteReader();
                playlist.Read();
                id = playlist[0].ToString();
                playlist.Dispose();
                command.Dispose();
            }

            else if (identifier.GetType() == typeof(int))
            {
                id = identifier.ToString();
            }

            SQLiteCommand delete = new SQLiteCommand(db);
            delete.CommandText = "DELETE FROM playlists WHERE id = @id; DELETE FROM playlist_songs WHERE playlistID = @id;";
            delete.Parameters.AddWithValue("id", id);
            delete.ExecuteNonQuery();
            delete.Dispose();
        }
    }
}
