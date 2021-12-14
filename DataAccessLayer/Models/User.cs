using SQLite;
using System;

/*
* Ernie Phillips III : 12/10/2021
* User object class schema for storing and retrieving db results
*/

namespace DataAccessLayer.Models {

  [Table("User")]
  public class User {

    [PrimaryKey, AutoIncrement, NotNull]
    public int USER_ID { get; set; }

    //[MaxLength(50), Unique, NotNull]
    [MaxLength(50), NotNull]
    public string USERNAME { get; set; }

    [MaxLength(64), NotNull]
    public string PASSWORD { get; set; }

    //[MaxLength(64), Unique, NotNull]
    [MaxLength(64), NotNull]
    public string EMAIL { get; set; }

    [MaxLength(100), NotNull]
    public string NAME { get; set; }

    public bool IS_LOCKED { get; set; } = false;

    public int FAILED_LOGIN_ATTEMPT { get; set; }

    public DateTime CREATED_DATE { get; set; }

    public DateTime LAST_LOGIN_DATE { get; set; }

    [NotNull]
    public string SECURITY_QUESTION { get; set; }

    [NotNull]
    public string SECURITY_ANSWER { get; set; }
  }
}
