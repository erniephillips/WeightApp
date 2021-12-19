using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer.Models {
  public class ListviewTextLeftRight {
    public int Id { get; set; }
    public string TextLeftSide {get;set;}
    public string TextRightSide { get; set; }
    public string HiddenTextForConversion { get; set; }
  }
}
