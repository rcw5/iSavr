using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace ISavr
{
    /// <summary>
    /// Media Database (held as a Dataset) containing all track information from an iPod.
    /// </summary>
    class MediaDatabase
    { 
        /// <summary>
        /// DataSet containing all the Media data
        /// </summary>
        private DataSet ds; 
        /// <summary>
        /// DataTable containing music only.
        /// </summary>
        private DataTable musicTable;

        /// <summary>
        /// Create a new MediaDatabase containing one DataSet and the Music datatable.
        /// Additionally populate the music table with the right fields.
        /// </summary>
        public MediaDatabase()
        {
            this.ds = new DataSet("MediaData");
            musicTable = new DataTable("Music");
            ds.Tables.Add(musicTable);
            this.populateTables();
        }

        /// <summary>
        /// Return the whole DataSet
        /// </summary>
        public DataSet MusicData
        {
            
            get { return ds; }
        }


        /// <summary>
        /// Populate tables with column information (this is taken from the iTunesDB)
        /// </summary>
        private void populateTables()
        {
            musicTable.Columns.Add("trackID", typeof(Int32));
            musicTable.Columns.Add("Title", typeof(string));
            musicTable.Columns.Add("Artist", typeof(string));
            musicTable.Columns.Add("Album", typeof(string));
            musicTable.Columns.Add("Location", typeof(string));
            musicTable.Columns.Add("Year", typeof(int));
            musicTable.Columns.Add("Genre", typeof(string));
            musicTable.Columns.Add("Length", typeof(long));
            musicTable.Columns.Add("MediaItem", typeof(MediaItem));

        }

        /// <summary>
        /// Add a new MediaItem to the database.
        /// </summary>
        /// <param name="mi">The Item to add to the database</param>
        public void add(MediaItem mi)
        {
            if (mi.Type == MediaItem.types.MP3)
            {
                musicTable.Rows.Add(new object[] { mi.TrackID, mi.Title, mi.Artist, mi.Album, mi.Filename, mi.Year, mi.Genre, mi.Length, mi });
            }


        }

        /// <summary>
        /// Clear the music database.
        /// </summary>
        public void clearDb()
        {
            musicTable.Clear();
        }


    }
}
