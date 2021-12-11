using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

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
      _SQLiteConnection.CreateTable<User>();
    }

    //GET: get list of all users
    public List<User> GetUsers() {
      //calling table returns all records
      return _SQLiteConnection.Table<User>().ToList();
    }

    public void AddUser(User user) {
      _SQLiteConnection.Insert(user);
    }
  }
}
