using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

/*
* Ernie Phillips III : 12/10/2021
* Profile object class schema for storing and retrieving db results
* SQLite -net-pcl does not offer PK/FK contraints out of the box. There is a sqlite-net-extensions pack that does and supports blob
* https://social.msdn.microsoft.com/Forums/en-US/13104ac7-9ae2-4276-a792-62905890f4d7/how-i-can-create-foreign-key-constraints-in-sqlite-using-sqlitenetpcl-nuget-package?forum=xamarincrossplatform
*/

namespace DataAccessLayer.Models {

  [Table("Profile")]
  public class Profile {

    [PrimaryKey, AutoIncrement, NotNull]
    public int PROFILE_ID { get; set; }

    [ForeignKey(typeof(User)), NotNull]
    public int USER_ID { get; set; }

    [NotNull]
    public string NAME { get; set; }

    [NotNull]
    public string START_WEIGHT { get; set; }

    [NotNull]
    public DateTime START_DATE { get; set; }

    [NotNull]
    public string HEIGHT { get; set; }

    [NotNull]
    public string GENDER { get; set; }

    [NotNull]
    public string MEASUREMENT_SYSTEM { get; set; }

    [NotNull]
    public string TARGET_WEIGHT { get; set; }

    [NotNull]
    public DateTime TARGET_DATE { get; set; }
  }
}
