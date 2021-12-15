using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

/*
* Ernie Phillips III : 12/11/2021
* Profile data access class, all methods will deal with Profile access to the db
* Methods consist of login/registration/validation/etc
* https://docs.microsoft.com/en-us/xamarin/android/data-cloud/data-access/using-sqlite-orm
*/

namespace DataAccessLayer.Dao {
  public class ProfileDao {
    private SQLiteConnection _SQLiteConnection = GetSQLiteConnnection.GetSQLConnection();

    public ProfileDao() {
      //create instance of table if none exists
      _SQLiteConnection.CreateTable<Profile>();
    }

    //GET: get list of all Profiles
    public List<Profile> GetProfiles() {
      //calling table returns all records
      return _SQLiteConnection.Table<Profile>().ToList();
    }

    //GET BY ID
    public Profile GetProfile(int id) {
      return _SQLiteConnection.Get<Profile>(id);
    }

    //GET BY USER ID
    public Profile GetProfileByUserId(int userId) {
      return _SQLiteConnection.Table<Profile>().Where(x => x.USER_ID == userId).FirstOrDefault();
    }

    //INSERT
    public void AddProfile(Profile profile) {
      _SQLiteConnection.Insert(profile);
    }

    //UPDATE
    public void UpdateProfile(Profile profile) { 
      _SQLiteConnection.Update(profile);
    }

    //DELETE
    public void DeleteProfile(Profile profile) {
      _SQLiteConnection.Delete(profile);
    }

    //DELETE ALL
    public void DeleteAll() {
      _SQLiteConnection.DeleteAll<Profile>();
    }
  }
}
