using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;

namespace ISavr
{
    /// <summary>
    /// Form used when the "save" button is pressed
    /// </summary>
    public partial class FrmSave : Form
    {
        public FrmSave()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtBaseDir.Text = fbd.SelectedPath;
            }
        }

        public string TextMask
        {
            get { return txtMask.Text; }
        }

        public string BaseDir
        {
            get { return txtBaseDir.Text; }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (TextMask.EndsWith(@"\"))
            {
                MessageBox.Show("Invalid file mask specified (mask must not end with a slash).", "Error Saving Files");
                DialogResult = DialogResult.None;
            }
            else if (TextMask.Length == 0)
            {
                MessageBox.Show("No file mask specified. Please select a file mask and retry.", "Error Saving Files");
                DialogResult = DialogResult.None;
            }
            else if (!Directory.Exists(BaseDir))
            {
                MessageBox.Show(String.Format("Base directory {0} does not exist.", BaseDir), "Error Saving Files");
                DialogResult = DialogResult.None;
            }
        }
 
    }
}