using DataAccessLayer.Interfaces;
using SQLite;
using SQLite.Net.Cipher.Security;
using SQLite.Net.Platform.XamarinAndroid;
using System;
using System.IO;

/*
* Ernie Phillips III : 12/11/2021
* Class access of weightapp database
* Returns: database name and user's personal file location for the db as a SQLiteConnection
* Set as static connection, otherwise I'll have to inject the connection inside every class constructor that even makes reference to a data call
* https://docs.microsoft.com/en-us/xamarin/xamarin-forms/data-cloud/data/databases
*/

namespace DataAccessLayer.Dao {
  public class GetSQLiteConnnection : ISQLiteConnection {
    public static SQLiteConnection GetSQLConnection() {
      return new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "WeightApp.db"));
    }
    public static string GetDbFilePath() {
      return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WeightApp.db");
    }
    public static SQLitePlatformAndroid GetPlatform() {
      return new SQLitePlatformAndroid();
    }
    public static string GetSalt() {
      return CryptoService.GenerateRandomKey(16);
    }
    public static string SeedKey() {
      return "s#xtQ0CvRDr2pQ$Qwae91AJeHIfp%*485Pc";
    }
  }
}
