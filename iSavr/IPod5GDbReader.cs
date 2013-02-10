using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

namespace ISavr
{
    /// <summary>
    /// Implementation of IDatabaseReader for a 5th Generation iPod
    /// </summary>
    class IPod5GDbReader : IDatabaseReader
    {
        private String iTunesDBFile, iPodDrive;
        /// <summary>
        /// Binary stream of the iTunesDB
        /// </summary>
        private BufferedStream strDB;
        /// <summary>
        /// MediaDatabase to put all information into.
        /// </summary>
        private MediaDatabase mediaDB;
        /// <summary>
        /// Types from the iTunesDB - these correspond to the mhit (see below!).
        /// </summary>
        private enum Types : long
        {
            Title = 1,
            Location = 2,
            Album  = 3,
            Artist = 4,
            Genre = 5
        }

        /// <summary>
        /// Constructor for class - create a new reader for a given Database.
        /// Read the settings for the iPod and iTunesDB location.
        /// </summary>
        /// <param name="mediaDB">The database in which to add the tracks.</param>
        public IPod5GDbReader(MediaDatabase mediaDB)
        {
            this.iTunesDBFile = ISavr.Properties.Settings.Default.iPodControlDir;
            this.mediaDB = mediaDB;
            this.iPodDrive = ISavr.Properties.Settings.Default.DriveLetter;
        }

        /// <summary>
        /// Read the iPod5G database into the MediaDatabase.
        /// 
        /// </summary>
        public void readDatabase()
        {
            int buf;
            long position;
            //Create the handle to the iTunes database
            strDB = new BufferedStream(new FileStream(iTunesDBFile, FileMode.Open, FileAccess.Read));
            //read!
            while ((buf = this.strDB.ReadByte()) != -1)
            {
                if (buf == (int)'m') //Read until we find a 'm'
                {
                    position = strDB.Position;

                    byte[] buff = new byte[3];
                    this.strDB.Read(buff, 0, 3);
                    if (Encoding.Default.GetString(buff).Equals("hit")) //'mhit' describes a song in the database
                    {
                         parseMhit();


                    }
                    else
                    {
                        this.strDB.Seek(position, SeekOrigin.Begin); //go back to just after the m
                    }

                }
            }
        }


        private void parseMhit()
        {
            long position;
            byte[] buffer = new byte[4]; //most stuff in iTunesDB is done in 4s..
            position = strDB.Position; //save our current position
            strDB.Read(buffer, 0, 4); //read header length - we are now at position 8
            long header = System.BitConverter.ToUInt32(buffer, 0);
            debug("Header length is: " + header);
            seek(4); //skip over total length of mhit - now at position 12
            strDB.Read(buffer, 0, 4); //read number of mhods (strings)
            long numMhods = System.BitConverter.ToUInt32(buffer, 0);
            debug("Number of mhods: " + numMhods);
            seek(28); //skip over various fields - now at position 44
            debug("Reading track number");
            strDB.Read(buffer, 0, 4); //read track number - now at position 48
            long trackNum = System.BitConverter.ToUInt32(buffer, 0);
            debug("Reading year");
            seek(4); //seek to year (position 52 of mhit)
            strDB.Read(buffer, 0, 4);
            int year = (int) System.BitConverter.ToUInt32(buffer, 0);
            seek(152); //seek to track type (position 208 of mhit)
            strDB.Read(buffer, 0, 4); //read track type
            MediaItem.types trackType = (MediaItem.types)System.BitConverter.ToUInt32(buffer, 0);
            MediaItem mi = new MediaItem(trackType, trackNum);
            mi.Year = year;
            //skip to start of mhods - a mhod is a certain piece of information about the track
            strDB.Seek(position, SeekOrigin.Begin); //seek back to where the 'header' is - this is the start of the mhods.
            strDB.Seek(header - 4, SeekOrigin.Current);
            debug("Parsing mhods..");
            for (int i = 0; i < numMhods; i++)
            {
                this.parseMhod(mi);
            }
            mediaDB.add(mi); //add item to database
            debug(mi.ToString());


        }

        private void parseMhod(MediaItem media)
        {
            byte[] buff = new byte[4];
            long position = strDB.Position;
            debug("Found mhod");
            seek(8); //skip over mhod to total length
            strDB.Read(buff, 0, 4); //read total length
            long totalLength = System.BitConverter.ToUInt32(buff, 0);
            debug("Size of this mhod is " + totalLength);
            strDB.Read(buff, 0, 4); //now at position 20 of this mhod
            long type = System.BitConverter.ToUInt32(buff, 0);
            if ((type == (long)Types.Album) || (type == (long)Types.Artist) || (type == (long)Types.Genre) ||
                (type == (long)Types.Location) || (type == (long)Types.Title)) //artist, album, title, filename
            {
                debug("Read type : " + type);
                seek(12);
                strDB.Read(buff, 0, 4); //read length of string
                long strLen = System.BitConverter.ToUInt32(buff, 0);
                debug("String length is " + strLen);
                seek(8); //now at position 40 of mhod (actual string)
                byte[] str = new byte[strLen];
                strDB.Read(str, 0, str.Length);
                String theString = new UnicodeEncoding().GetString(str);
                 switch (type)
                {
                    case (long)Types.Title:
                        media.Title = theString;
                        break;
                    case (long)Types.Album:
                        media.Album = theString;
                        break;
                    case (long)Types.Artist:
                        media.Artist = theString;
                        break;
                    case (long)Types.Location:
                        media.Filename = iPodDrive + theString.Replace(':', '\\');
                        break;
                     case (long)Types.Genre:
                         media.Genre = theString;
                        break;
                    default:
                        break;
                }

            }
            strDB.Seek(position, SeekOrigin.Begin);
            strDB.Seek(totalLength, SeekOrigin.Current);



        }

        /// <summary>
        /// Reload the database file - called when the preferences change.
        /// </summary>
        public void reloadDatabaseFile()
        {
            this.iTunesDBFile = ISavr.Properties.Settings.Default.iPodControlDir;
            this.iPodDrive = ISavr.Properties.Settings.Default.DriveLetter;
        }

        /// <summary>
        /// Seek a given amount into the BufferedReader
        /// </summary>
        /// <param name="distance">The number of bytes to skip.</param>
        private void seek(int distance)
        {
            debug("seeking To " + (strDB.Position + distance));
            strDB.Seek(distance, SeekOrigin.Current);
        }

        /// <summary>
        /// Debug messages
        /// </summary>
        /// <param name="message">The message to print (if debugging is on)</param>
        private void debug(String message)
        {
            if (Program.isDebug)
            {
                Console.WriteLine("(" + strDB.Position + ") " + message);
            }

        }

       


    }
}

