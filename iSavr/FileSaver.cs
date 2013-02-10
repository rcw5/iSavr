using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.IO;

namespace ISavr
{

    /// <summary>
    /// Class to save files from the iPod to a local drive
    /// </summary>
    class FileSaver
    {
        /// <summary>
        /// List of MediaItems to save
        /// </summary>
        private List<MediaItem> list;
        /// <summary>
        /// Format string (simple regex) used to parse track types
        /// </summary>
        private string formatstr;
        /// <summary>
        /// Base directory to save files into
        /// </summary>
        private string basedir;
        /// <summary>
        /// Background worker used to save the actual files
        /// </summary>
        private BackgroundWorker bw;

        /// <summary>
        /// Constructure for FileSaver class
        /// </summary>
        /// <param name="li">List of Media Items</param>
        /// <param name="formatstr">Format string used</param>
        /// <param name="basedir">Base save directory</param>
        /// <param name="bw">Background worker used to save the file</param>
        public FileSaver(List<MediaItem> li, string formatstr, string basedir, BackgroundWorker bw)
        {
            this.list = li;
            this.formatstr = formatstr;
            this.basedir = basedir;
            this.bw = bw;
        }

        /// <summary>
        /// Save the files from the iPod to disk
        /// </summary>
        public void saveFiles()
        {
            //firstly try and access a file.. this will save us creating directories unnecessarily.

            if (!File.Exists(list[0].Filename))
            {
                throw new Exception("Cannot open file off media player.  Please ensure that your device is connected.");

            }
            //Check if baseDir exists
            DirectoryInfo di = new DirectoryInfo(basedir);
            if (!di.Exists)
            {
                throw new Exception("Base directory does not exist!");
            }
            //At this stage, baseDir exists and we know the iPod is connected.
            int numFiles = 0;
            foreach (MediaItem mi in list)
            {
                string iPodfilename = mi.Filename;
                string newFilename = parseFileName(mi, formatstr);
                string[] directories = newFilename.Split('\\');
                string workingPath = basedir;
                for (int i = 0; i < directories.Length - 1; i++)
                {
                    string directory = directories[i];
                    createIfNotExists(workingPath + directory);
                    workingPath = workingPath + directory + "\\";
                }

                copyFile(iPodfilename, basedir + newFilename);
                numFiles++;
                double progress = (double)numFiles / (double)list.Count;
                int percentageDone = (int)(progress * 100);
                //report our progress to the background worker
                bw.ReportProgress(percentageDone);

            }
        }

        /// <summary>
        /// Replaces any invalid characters in a string - Windows will error if you create a file with these characters!
        /// </summary>
        /// <param name="text">String to check</param>
        /// <returns>A string with the invalid characters removed.</returns>
        private string replaceInvalidChars(string text)
        {
            text = text.Replace('/', '_');
            text = text.Replace('|', '_');
            text = text.Replace(':', '_');
            text = text.Replace('*', '_');
            text = text.Replace('>', '_');
            text = text.Replace('<', '_');
            text = text.Replace('?', '_');
            text = text.Replace('"', '_');
            return text;
        }

        /// <summary>
        /// Copy a file from one location to another.
        /// </summary>
        /// <param name="source">Source of file (full path)</param>
        /// <param name="target">Destination of file (full path)</param>
        private void copyFile(string source, string target)
        {
            string extension = source.Substring(source.LastIndexOf("."));
            target = target + extension;
            try
            {
                File.Copy(source, target);
            }
            catch (Exception)
            {
                throw new Exception(String.Format("An error occured whilst trying to save file {0} to path {1}", source, target));

            }
        }

        /// <summary>
        /// Create a directory if it does not exist
        /// </summary>
        /// <param name="path">Path to directory.</param>
        private void createIfNotExists(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            if (!di.Exists)
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception)
                {
                    throw new ArgumentException("Error whilst creating directory");
                }

            }

        }

        /// <summary>
        /// Parse a filename using the 'regexp' given in FrmSave.
        /// </summary>
        /// <param name="mi">MediaItem to parse</param>
        /// <param name="formatstr">formatString containing the 'regexp'</param>
        /// <returns></returns>
        private string parseFileName(MediaItem mi, string formatstr)
        {
            string filename = formatstr;
            filename = filename.Replace("%a", mi.Artist);
            filename = filename.Replace("%A", mi.Album);
            filename = filename.Replace("%t", Convert.ToString(mi.Title));
            filename = filename.Replace("%y", Convert.ToString(mi.Year));
            filename = filename.Replace("%n", Convert.ToString(mi.TrackID));
            filename = filename.Replace("%N", mi.TrackID < 10 ? "0" + Convert.ToString(mi.TrackID) : Convert.ToString(mi.TrackID));
            filename = filename.Replace("%g", mi.Genre);
            filename = replaceInvalidChars(filename);
            return filename;

        }
    }
}
