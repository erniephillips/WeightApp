using DataAccessLayer.Models;
using SQLite;
using System.Collections.Generic;

/*
* Ernie Phillips III : 12/11/2021
* Weight data access class, all methods will deal with weight access to the db
* Methods consist of login/registration/validation/etc
* https://docs.microsoft.com/en-us/xamarin/android/data-cloud/data-access/using-sqlite-orm
*/

namespace DataAccessLayer.Dao {
  public class WeightDao {
    private SQLiteConnection _SQLiteConnection = GetSQLiteConnnection.GetSQLConnection();

    public WeightDao() {
      //create instance of table if none exists
      _SQLiteConnection.CreateTable<Weight>();
    }

    //GET: get list of all weights
    public List<Weight> GetWeights() {
      //calling table returns all records
      return _SQLiteConnection.Table<Weight>().ToList();
    }

    //GET BY ID
    public Weight GetWeight(int id) {
      return _SQLiteConnection.Get<Weight>(id);
    }

    //GET BY USER ID
    public List<Weight> GetWeightsByProfileId(int id) {
      return _SQLiteConnection.Table<Weight>().Where(x => x.PROFILE_ID == id).ToList();
    }

    //GET WEIGHTS BY USER ID - ORDERY BY DATE ASC
    public List<Weight> GetWeightsByProfileIdOrderByDateAsc(int id) {
      return _SQLiteConnection.Table<Weight>().Where(x => x.PROFILE_ID == id)
        .OrderBy(x => x.DATE_ENTRY).ToList();
    }

    //GET WEIGHTS BY USER ID - ORDERY BY DATE DESC
    public List<Weight> GetWeightsByProfileIdOrderByDateDesc(int id) {
      return _SQLiteConnection.Table<Weight>().Where(x => x.PROFILE_ID == id)
        .OrderByDescending(x => x.DATE_ENTRY).ToList();
    }

    //GET WEIGHTS BY USER ID - MOST RECENT WEIGHT
    public Weight GetWeightsByProfileIdMostRecentDate(int id) {
      return _SQLiteConnection.Table<Weight>().Where(x => x.PROFILE_ID == id)
        .OrderByDescending(x => x.DATE_ENTRY).FirstOrDefault();
    }

    //INSERT
    public void AddWeight(Weight weight) {
      _SQLiteConnection.Insert(weight);
    }

    //UPDATE
    public void UpdateWeight(Weight weight) {
      _SQLiteConnection.Update(weight);
    }

    public void BulkUpdateWeights(List<Weight> weights) {
      _SQLiteConnection.UpdateAll(weights);
    }

    //DELETE
    public void DeleteWeight(Weight weight) {
      _SQLiteConnection.Delete(weight);
    }

    //DELETE ALL
    public void DeleteAll() {
      _SQLiteConnection.DeleteAll<Weight>();
    }
  }
}
