using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace ISavr
{
    /// <summary>
    /// Interface for a database reader of any type (iPod, Creative Zen etc)
    /// </summary>
    interface IDatabaseReader
    {
        /// <summary>
        /// Read the database, parsing all information out into the MediaDatabase
        /// </summary>
        void readDatabase();
        /// <summary>
        /// Reload the database file, reloading all information into the MediaDatabase.
        /// </summary>
        void reloadDatabaseFile();

    }
}
