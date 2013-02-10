using System;
using System.Collections.Generic;
using System.Text;

namespace ISavr
{
    /// <summary>
    /// A Media Item in the database - currently only used to store MP3s however
    /// this is extendable for Videos and Music Videos (although would require some refactoring).
    /// </summary>
    public class MediaItem
    {

        private string title, filename, artist, album, genre;
        private long trackid, length;
        private types type;
        private int year;

        /// <summary>
        /// These types come from the iTunesDB.
        /// </summary>
        public enum types
        {
            MP3 = 1,
            Video = 2,
            MusicVideo = 32,
            Unknown
        }

        public types Type
        {
            get { return type; }
            set { type = value; }
        }

        public string Artist
        {
            get { return artist; }
            set { artist = value; }
        }

        public long Length
        {
            get { return length; }
            set { length = value; }
        }

        public string Genre
        {
            get { return genre; }
            set { genre = value; }
        }
        public string Album
        {
            get { return album; }
            set { album = value; }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        public long TrackID
        {
            get { return trackid; }
            set { trackid = value; }
        }

        public int Year
        {
            get { return year; }
            set { year = value; }
        }


        /// <summary>
        /// Create a new media item with the type and tracknumber.
        /// The reason for not collecting all info in the constructor is that it gets
        /// ripped out of the database consequitively.
        /// </summary>
        /// <param name="type">Media Type</param>
        /// <param name="tracknum">Track Number</param>
        public MediaItem(types type, long tracknum)
        {
            this.title = null;
            this.filename = null;
            this.album = null;
            this.artist = null;
            this.type = type;
            this.trackid = tracknum;
        }

        /// <summary>
        /// ToString method - prints out the information from the track.
        /// </summary>
        /// <returns>a string containing all information about the mediaitem</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Media Track\n");
            sb.Append(String.Format("Track Number : {0}\n", this.TrackID));
            sb.Append(String.Format("Title : {0}\n", this.Title));
            sb.Append(String.Format("Artist : {0}\n", this.Artist));
            sb.Append(String.Format("Album : {0}\n", this.Album));
            sb.Append(String.Format("Year : {0}\n", this.Year));
            sb.Append(String.Format("Type : {0}\n", this.Type));
            sb.Append(String.Format("Genre : {0}\n", this.Genre));
            return sb.ToString();

        }



    }
}

