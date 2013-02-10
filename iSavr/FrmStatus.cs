using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ISavr
{
    /// <summary>
    /// GUI Form used to control the saving of files from the iPod to disk.
    /// The form uses a BackgroundWorker to asynchronously copy files from the iPod,
    /// ensuring that the UI stays responsive (and updates a progressbar) as the saving takes place.
    /// The form calls the FileSaver class to perform the actual saving actions.
    /// </summary>
    public partial class FrmStatus : Form
    {
        /// <summary>
        /// A list of MediaItems to save to disk
        /// </summary>
        List<MediaItem> list;

        /// <summary>
        /// BackgroundWorker used for asynchronous saving of the MP3s
        /// </summary>
        BackgroundWorker bw;


        string formatstr, path, message; //message displayed when the saving is complete

        /// <summary>
        /// Constructor for the status GUI form.  
        /// </summary>
        /// <param name="li">List of MediaItems to process</param>
        /// <param name="formatstr">Format String used to parse filenames</param>
        /// <param name="path">Base save path of tracks</param>
        public FrmStatus(List<MediaItem> li, string formatstr, string path)
        {
            InitializeComponent();
            this.list = li;
            this.path = path;
            this.formatstr = formatstr;
            this.Cursor = Cursors.WaitCursor; //set the cursor to the hourglass
            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.DoWork += new DoWorkEventHandler(save_files); //save_files run 
            bw.ProgressChanged += bw_onProgressChanged; //called when the bw updates progress
            bw.RunWorkerCompleted += bw_completed; //called when the bw is completed
            bw.RunWorkerAsync(); //start the bw
        }

        /// <summary>
        /// Asynchronous callback run in the BackgroundWorker object.  
        /// This method is called in the context of the BackgroundWorker thread.
        /// </summary>
        /// <param name="sender">The class calling the method</param>
        /// <param name="dwea">Event arguments for the background worker</param>
        public void save_files(object sender, DoWorkEventArgs dwea)
        {
            FileSaver fs = new FileSaver(list, formatstr, path, bw);
            try
            {
                fs.saveFiles();
            }
            catch (Exception e2)
            {
                MessageBox.Show(e2.Message + "  Saving has been cancelled.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                message = "Saving failed!";
            }
        }

        /// <summary>
        /// Asynchronous method called when FileSaver updates the progress of saving.
        /// This method is called in the context of the UI thread!
        /// </summary>
        /// <param name="sender">The class sending the </param>
        /// <param name="dwea"></param>
        public void bw_onProgressChanged(object sender, ProgressChangedEventArgs dwea)
        {
            this.progressBar1.Value = dwea.ProgressPercentage;
            if (dwea.ProgressPercentage == 100)
            {
                message = "Saving complete!";
            }

        }

        /// <summary>
        /// Closes the Status form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Asynchronous method called when the background worker has completed (either successfully or not).
        /// This method is called in the context of the UI thread.
        /// </summary>
        /// <param name="sender">The object calling the class</param>
        /// <param name="e">Event args for the method</param>
        private void bw_completed(object sender, RunWorkerCompletedEventArgs e)
        {
            label1.Text = message;
            btnClose.Enabled = true;
            this.Cursor = Cursors.Default;

        }
    }

}
