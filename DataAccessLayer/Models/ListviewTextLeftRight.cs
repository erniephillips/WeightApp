using System;
using System.Collections.Generic;
using System.Text;

/*
* Ernie Phillips III : 01/27/2022
* Object model used by profile and weight entry pages to output a title and input value from a object list
*/


namespace DataAccessLayer.Models {
  public class ListviewTextLeftRight {
    public int Id { get; set; }
    public string TextLeftSide {get;set;}
    public string TextRightSide { get; set; }
    public string HiddenTextForConversion { get; set; }
  }
}
