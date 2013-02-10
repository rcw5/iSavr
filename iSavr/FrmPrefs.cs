using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ISavr
{
    public partial class FrmPrefs : Form
    {
        public FrmPrefs()
        {
            InitializeComponent();
        }


        private void btnBrose_Click(object sender, EventArgs e)
        {
            if (OpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                dbLocation.Text = OpenFileDialog.FileName;
            }
        }

        private void FrmPrefs_Load(object sender, EventArgs e)
        {
            String dbPath = ISavr.Properties.Settings.Default.iPodControlDir;
            this.dbLocation.Text = dbPath;
        }

        private void OpenFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            if (!this.OpenFileDialog.FileName.EndsWith("iTunesDB"))
            {
                MessageBox.Show("Incorrect iTunesDB path (file selected not called iTunesDB).");
                e.Cancel = true;
                return;
            }

        }

        private void btnSave_Click(object sender, EventArgs e)

        {
            if (this.dbLocation.Text.Equals(""))
            {
                MessageBox.Show("Incorrect iTunesDB location - field cannot be empty");
                DialogResult = DialogResult.None;
            }
        }
    }
}