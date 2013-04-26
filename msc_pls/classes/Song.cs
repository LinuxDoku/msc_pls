using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SQLite;

namespace msc_pls.classes
{
    public class Song
    {
        public String title;
        public String artist;
        public String album;
        public String path;
        public MemoryStream image;
        public float duration;

        public Song(String path)
        {
            this.path = path;

            SQLiteConnection db = Library.getSQLiteConnection();
            SQLiteCommand command = new SQLiteCommand(db);
            command.CommandText = "SELECT title, artist, album, image FROM songs WHERE path = @path";
            command.Parameters.AddWithValue("path", this.path);
            SQLiteDataReader song = command.ExecuteReader();
            song.Read();
            this.title = song[0].ToString();
            this.artist = song[1].ToString();
            this.album = song[2].ToString();
            //this.duration = (float)Convert.ToDouble(song[3].ToString()); @TODO fix this?

            // convert image
            String image = song[3].ToString();
            if (image != null)
            {
                byte[] byteArray = Convert.FromBase64String(image);
                this.image = new MemoryStream(byteArray);
            }
            song.Dispose();
            command.Dispose();
        }
    }
}
