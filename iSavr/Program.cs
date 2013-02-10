using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace ISavr
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

           Application.EnableVisualStyles();
           Application.SetCompatibleTextRenderingDefault(false);
           Application.Run(new iSavr());


        }
        /// <summary>
        /// Are we debugging or not?
        /// </summary>
        public readonly static bool isDebug = false;

        public readonly static double version = 1.0;
    }
}