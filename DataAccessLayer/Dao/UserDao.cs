using DataAccessLayer.Models;
using SQLite;
using System.Collections.Generic;

/*
* Ernie Phillips III : 12/11/2021
* User data access class, all methods will deal with user access to the db
* Methods consist of login/registration/validation/etc
* https://docs.microsoft.com/en-us/xamarin/android/data-cloud/data-access/using-sqlite-orm
*/

namespace DataAccessLayer.Dao {
  public class UserDao {
    private SQLiteConnection _SQLiteConnection = GetSQLiteConnnection.GetSQLConnection();

    public UserDao() {
      //create instance of table if none exists
      _SQLiteConnection.CreateTable<User>();
    }

    //GET: get list of all users
    public List<User> GetUsers() {
      //calling table returns all records
      return _SQLiteConnection.Table<User>().ToList();
    }

    //GET BY ID
    public User GetUser(int id) {
      return _SQLiteConnection.Get<User>(id);
    }

    //GET BY USERNAME
    public User GetUserByUsername(string username) {
      return _SQLiteConnection.Table<User>().Where(x => x.USERNAME == username).FirstOrDefault();
    }

    //INSERT
    public void AddUser(User user) {
      _SQLiteConnection.Insert(user);
    }

    //UPDATE
    public void UpdateUser(User user) { 
      _SQLiteConnection.Update(user);
    }

    //DELETE
    public void DeleteUser(User user) {
      _SQLiteConnection.Delete(user);
    }

    public void DeleteAll() {
      _SQLiteConnection.DeleteAll<User>();
    }

    #region USER LOGIN LOGIC
    public bool VerifyLogin(string username, string password) {
      int recordCount = _SQLiteConnection.Table<User>().Where(x => x.USERNAME == username && x.PASSWORD == password).Count();
      if (recordCount > 0)
        return true;
      return false;
    }
    #endregion
  }
}
