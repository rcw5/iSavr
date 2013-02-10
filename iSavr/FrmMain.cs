using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace ISavr
{
    /// <summary>
    /// The main form of the program - used to display the listview containing 
    /// all media tracks and to control all other forms in the application
    /// to change preferences and to save tracks to the computer from the iPod.
    /// </summary>
    public partial class iSavr : Form
    {
        /// <summary>
        /// The MediaDatabase which the listview pulls its data from
        /// </summary>
        MediaDatabase mediaDataBase;
        /// <summary>
        /// The current sort column of the listview
        /// </summary>
        int curColumn; 
        /// <summary>
        /// The current SortOrder of the listview
        /// </summary>
        SortOrder so;
        
        /// <summary>
        /// The database reader instance used for the application.
        /// </summary>
        IDatabaseReader db;

        public iSavr()
        {
            InitializeComponent();

        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            mediaDataBase = new MediaDatabase();
            db = new IPod5GDbReader(mediaDataBase);

            //Ask for the iTunesDB the first time the application is loaded.
            if ((ISavr.Properties.Settings.Default.iPodControlDir.Equals("")) || (ISavr.Properties.Settings.Default.DriveLetter.Equals("")))
            {
                MessageBox.Show("Welcome to iSavr!\nAs this is the first time you have loaded the application, you will need to point me to your iTunes Database file!", "Welcome to iSavr!");
                showPreferencesForm();
            }
            try
            {
                reloadDatabase();
            }
            catch (Exception)
            {
                //loop forever until we have a database!
                while (true)
                {
                    DialogResult result = MessageBox.Show("Cannot access iTunes Database - check that your iPod is connected.", "Error Reading Database", MessageBoxButtons.RetryCancel);
                    if (result == DialogResult.Retry)
                    {
                        try
                        {
                            reloadDatabase();
                            break;
                        }
                        catch (Exception)
                        {
                            //go around again!
                        }
                        

                    }
                    else if (result == DialogResult.Cancel)
                    {
                        break;
                    }
                }
                
            }

        }

        /// <summary>
        /// Reload the database - called when the itunesDB property has changed.
        /// </summary>
        private void reloadDatabase()
        {
            mediaDataBase.clearDb();
            db.reloadDatabaseFile();
            db.readDatabase();
            LoadList();
        }

        /// <summary>
        /// Populate the Listview with the items from the music DataTable
        /// </summary>
        private void LoadList()
        {
            //BUGFIX: Stops program hanging when a new iTunesDb is loaded whilst list is sorted
            lvMedia.ListViewItemSorter = null;

            curColumn = 0;
            DataSet dataSet = mediaDataBase.MusicData;
            DataTable dataTable = dataSet.Tables["Music"];
            lvMedia.Items.Clear();
            lvMedia.BeginUpdate(); //This is done to stop flickering
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                DataRow dataRow = dataTable.Rows[i];
                //Create three columns - title, artist and album, and then 'tag' the row with the dataRow.
                ListViewItem lvi = new ListViewItem(dataRow["Title"].ToString());
                lvi.SubItems.Add(dataRow["Artist"].ToString());
                lvi.SubItems.Add(dataRow["Album"].ToString());
                lvi.UseItemStyleForSubItems = false;
                lvi.Tag = dataRow;
                lvMedia.Items.Add(lvi);
            }
            lvMedia.EndUpdate();
        }

        /// <summary>
        /// Reload the listview with an array of DataRows.
        /// This method is called when a search has been performed.
        /// </summary>
        /// <param name="rows">An array of rows of which to populate the listview with</param>
        private void LoadList(DataRow[] rows)
        {
            //Firstly, set the ListViewItemSorter to null to improve performance
            lvMedia.ListViewItemSorter = null;
            lvMedia.Items.Clear();
            lvMedia.BeginUpdate(); //stop flickering
            for (int i = 0; i < rows.Length; i++)
            {
                //Same as above - three rows and a 'tag' of the datarow.
                DataRow dataRow = rows[i];
                ListViewItem lvi = new ListViewItem(dataRow["Title"].ToString());
                lvi.SubItems.Add(dataRow["Artist"].ToString());
                lvi.SubItems.Add(dataRow["Album"].ToString());
                lvi.UseItemStyleForSubItems = false;
                lvi.Tag = dataRow;
                lvMedia.Items.Add(lvi);
            }
            //Set the ListViewItemSorter to null to improve performance
            lvMedia.ListViewItemSorter = null;
            lvMedia.EndUpdate();

        }


        /// <summary>
        /// Method to fomat a column in the listview a certain colour.
        /// This is called when a column is clicked for sorting.
        /// </summary>
        private void formatListViewColumn()
        {
            foreach (ListViewItem lvi in lvMedia.Items)
            {
                lvi.SubItems[0].BackColor = Color.White;
                lvi.SubItems[1].BackColor = Color.White;
                lvi.SubItems[2].BackColor = Color.White;
                lvi.SubItems[curColumn].BackColor = Color.WhiteSmoke;
            }
        }


        /// <summary>
        /// Search in the Dataset for an entry in the search box.
        /// This perform three LIKE searches in the Artist, Title or Album 
        /// and returns all tracks that match.
        /// </summary>
        /// <param name="searchstr">The string to search for</param>
        private void search(string searchstr)
        {
            DataSet dataSet = mediaDataBase.MusicData;
            DataTable dataTable = dataSet.Tables["Music"];
            DataRow[] rows;
            if (searchstr.Equals("")) 
            {
                //For performance, when the search string is empty save a lengthy search
                rows = dataTable.Select(); 
            }
            else
            {
                rows = dataTable.Select(String.Format("Artist like '%{0}%' OR Title like '%{0}%' OR Album like '%{0}%'", searchstr));
            }
            LoadList(rows);

            /*Because the listview is cleared after searching, the columns 
             * need reformatting and the listview needs sorting once more.
             */

            this.formatListViewColumn();
            lvMedia.ListViewItemSorter = new ListViewSorter(curColumn, so);
            lvMedia.Sort();
        }


        /// <summary>
        /// Called when the listview has a column clicked, for sorting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvMedia_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            int newColumn = e.Column;
            //swap the sortorder over if required (i.e. on two clicks of the column)
            if (curColumn == -1)
            {
                so = SortOrder.Ascending;
            }
            else if (curColumn == newColumn && so == SortOrder.Ascending)
            {
                so = SortOrder.Descending;
            }
            else if (curColumn == newColumn && so == SortOrder.Descending)
            {
                so = SortOrder.Ascending;
            }
            else
            {
                so = SortOrder.Ascending;
            }

            curColumn = e.Column;
            //After sorting, format the listview and apply the sorter algorithm
            this.formatListViewColumn();
            lvMedia.ListViewItemSorter = new ListViewSorter(e.Column, so);
            lvMedia.Sort();


        }

        /// <summary>
        /// Method called when the search textbox changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

            search(txtSearch.Text);
        }

        /// <summary>
        /// Method called when 'Exit' is selected from the toolbar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Method called when the 'save' button is clicked.
        /// This displays frmSave, which in turn (when the user pressed 'OK' on this form)
        /// kicks off the status form to save all the MP3s to disk.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (lvMedia.SelectedItems.Count == 0)
            {
                MessageBox.Show("You must select some items to save.", "Error Saving");
                return;
            }

            FrmSave frms = new FrmSave();
            if (frms.ShowDialog() == DialogResult.OK)
            {
                string path = frms.BaseDir;
                if (!path.EndsWith("\\"))
                {
                    path = path + "\\";
                }

                string formatstr = frms.TextMask;
                List<MediaItem> selectedItems = new List<MediaItem>();

                foreach (ListViewItem li in lvMedia.SelectedItems)
                {
                    DataRow dr = (DataRow)li.Tag;
                    MediaItem mi = (MediaItem)dr["MediaItem"];
                    selectedItems.Add(mi);
                }
                new FrmStatus(selectedItems, formatstr, path).ShowDialog();
            }
        }

        /// <summary>
        /// Method to handle pressing the Preferences button on the toolbar.  Simply calls the
        /// showPreferencesForm method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrefs_Click(object sender, EventArgs e)
        {
            showPreferencesForm();
        }


        /// <summary>
        /// Method to display the Preferences form - this saves the state of the form
        /// to the configuration XML file and reloads the database.
        /// </summary>
        private void showPreferencesForm()
        {
            FrmPrefs fp = new FrmPrefs();
            if (fp.ShowDialog() == DialogResult.OK)
            {
                string drive = fp.dbLocation.Text.Substring(0, 2);
                ISavr.Properties.Settings.Default.iPodControlDir = fp.dbLocation.Text;
                ISavr.Properties.Settings.Default.DriveLetter = drive;
                ISavr.Properties.Settings.Default.Save();
                reloadDatabase();
            }
        }

        /// <summary>
        /// Method called when the Settings menu option is chosen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showPreferencesForm();
        }

        /// <summary>
        /// Method called when the About option is chosen from the Help menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmAbout fa = new FrmAbout();
            fa.ShowDialog();
        }


    }

    /// <summary>
    /// Class implementing IComparer to perform sorting of the Listview items in either
    /// ascending or descending alphabetical order.
    /// </summary>
    class ListViewSorter : System.Collections.IComparer
    {
        private int CurrentColumn;
        private SortOrder so;

        /// <summary>
        /// Compare two ListViewItems
        /// </summary>
        /// <param name="x">The first ListViewItem to compare</param>
        /// <param name="y">The second ListViewItem to compare</param>
        /// <returns>Should the sortorder be ascending, then the ListViewItem further up (towards A) in the alphabet, should the sort order be descending then vice-versa.</returns>
        public int Compare(object x, object y)
        {
            ListViewItem rowa = (ListViewItem)x;
            ListViewItem rowb = (ListViewItem)y;
            if (so == SortOrder.Ascending)
            {
                return String.Compare(rowa.SubItems[CurrentColumn].Text, rowb.SubItems[CurrentColumn].Text);
            }
            else
            {
                return String.Compare(rowb.SubItems[CurrentColumn].Text, rowa.SubItems[CurrentColumn].Text);
            }

        }

        /// <summary>
        /// Constructure for ListViewSorter
        /// </summary>
        /// <param name="column">The columnID to sort on</param>
        /// <param name="so">The sort order (Ascending/Descending) to sort on</param>
        public ListViewSorter(int column, SortOrder so)
        {
            this.CurrentColumn = column;
            this.so = so;
        }


    }

}