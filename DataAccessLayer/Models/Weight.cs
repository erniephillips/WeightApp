using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

/*
* Ernie Phillips III : 12/10/2021
* Weight entry object class schema for storing and retrieving db results
*/

namespace DataAccessLayer.Models {

  [Table("Weight")]
  public class Weight {

    [PrimaryKey, AutoIncrement, NotNull]
    public int WEIGHT_ID { get; set; }

    [ForeignKey(typeof(Profile)), NotNull]
    public int PROFILE_ID { get; set; }

    [NotNull]
    public decimal WEIGHT_ENTRY { get; set; }

    [NotNull]
    public DateTime DATE_ENTRY { get; set; }

    //need to reference this later for image storage: https://docs.microsoft.com/en-us/dotnet/standard/data/sqlite/blob-io
    public byte[] IMAGE { get; set; }

    [MaxLength(75)]
    public string IMAGE_NAME { get; set; }

    public int IMAGE_SIZE { get; set; }

    [MaxLength(10)]
    public string IMAGE_TYPE { get; set; }
  }
}
