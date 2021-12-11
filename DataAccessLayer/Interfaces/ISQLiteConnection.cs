using SQLite;

/*
* Ernie Phillips III : 12/11/2021
* Defining interface for classes that will make use of database connection
*/

namespace DataAccessLayer.Interfaces {
  public interface ISQLiteConnection {
    static SQLiteConnection GetSQLConnection() { return null; }
  }
}
