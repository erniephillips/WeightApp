using DataAccessLayer.Interfaces;
using SQLite;
using System;
using System.IO;

/*
* Ernie Phillips III : 12/11/2021
* Class access of weightapp database
* Returns: database name and user's personal file location for the db as a SQLiteConnection
*/

namespace DataAccessLayer.Dao {
  public class GetSQLiteConnnection : ISQLiteConnection {
    public SQLiteConnection GetSQLConnection() {
      return new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "WeightApp.db"));
    }
  }
}
