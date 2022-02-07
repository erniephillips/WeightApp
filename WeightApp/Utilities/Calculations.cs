using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

/*
* Ernie Phillips III : 01/27/2022
* Purpose: Handle core app calulations and logic
* Function: Several methods for calculations to output results to stats page
*/

namespace WeightApp.Utilities {
  public class Calculations {

    /// <summary>
    /// //calcuations: https://www.cdc.gov/nccdphp/dnpao/growthcharts/training/bmiage/page5_2.html
    ///Weight of 37 lbs and 4 oz = 37.25 lbs (16 ounces = 1 pound so 4 oz/16 oz = 0.25).
    ///(37.25 lbs / 41.5 in / 41.5 in) x 703 = 15.2
    /// </summary>
    /// <param name="weight"></param>
    /// <param name="height"></param>
    /// <returns>double</returns>
    public double GetBmi(string weight, string height, string system) {
      if (system == "Metric") {
        double convertWeight = Convert.ToDouble(weight);
        double convertHeight = Convert.ToDouble(height);
        double BMI = (convertWeight / convertHeight / convertHeight) * 10000;
        double roundedBmi = Math.Round(BMI, 1, MidpointRounding.AwayFromZero);

        return roundedBmi;
      } else {
        //string[] splitWeight = weight.Split(".");

        //double convertWeight = Convert.ToDouble(splitWeight[0]) + ConvertOuncesToDecimal(splitWeight[1]);
        double convertWeight = Convert.ToDouble(weight);
        double convertHeight = ConvertFeetToInches(height);
        //double BMI = (convertWeight / (convertHeight * convertHeight)) * 703;
        double BMI = 703.0 * convertWeight / Math.Pow(convertHeight, 2);
        //double bmi = (200.0 / Math.Pow(68.4, 2)) * 703.0;

        double roundedBmi = Math.Round(BMI, 1, MidpointRounding.AwayFromZero);

        return roundedBmi;
      }
    }

    /// <summary>
    /// Returns average weekly weight loss
    /// </summary>
    /// <param name="weights"></param>
    /// <returns>string</returns>
    public string GetAverageWeeklyWeightLoss(List<Weight> weights, string startWeight, string system) {
      //https://stackoverflow.com/questions/10284133/sum-range-of-ints-in-listint
      //https://stackoverflow.com/questions/16732206/how-to-cast-the-listobject-to-listt

      if (system == "Metric") {
        int sum = 0;
        int average = 0;

        int lastWeight = Convert.ToInt32(startWeight);

        foreach (Weight w in weights) {
          int currentWeight = Convert.ToInt32(w.WEIGHT_ENTRY);
          sum += (lastWeight - currentWeight);
          lastWeight = Convert.ToInt32(w.WEIGHT_ENTRY);
        }
        average = sum / (weights.Count); //divide the sum of the difference of numbers by count of weights

        if (average > 0) //show user loss or gain
          return string.Format("Average loss of {0} kg per entry", average);
        else
          return string.Format("Average gain of {0} kg per entry", Math.Abs(average));
      } else { //imperial
        double sum = 0;
        double average = 0;

        //string[] splitStartWeight = startWeight.Split(".");
        //double lastWeight = Convert.ToDouble(splitStartWeight[0]) + ConvertOuncesToDecimal(splitStartWeight[1]);
        double lastWeight = Convert.ToDouble(startWeight);

        foreach (Weight w in weights) {
          //string[] splitWeightEntry = w.WEIGHT_ENTRY.Split(".");
          //double currentWeight = Convert.ToDouble(splitWeightEntry[0]) + ConvertOuncesToDecimal(splitWeightEntry[1]);
          double currentWeight = Convert.ToDouble(w.WEIGHT_ENTRY);
          sum += (lastWeight - currentWeight);
          //lastWeight = Convert.ToDouble(splitWeightEntry[0]) + ConvertOuncesToDecimal(splitWeightEntry[1]);
          lastWeight = Convert.ToDouble(w.WEIGHT_ENTRY);
        }
        average = sum / (weights.Count); //divide the sum of the difference of numbers by count of weights

        bool isInt = average == (int)average;

        //if (!isInt) {
        //  //string[] splitAverage = average.ToString().Split("."); //convert to string to extract decimal
        //  double cnvDecimalToOz = (Convert.ToDouble("." + splitAverage[1]) * .16);
        //  if (average < 0) //check if negative avg, if so subtract oz in order to add
        //    average = Convert.ToDouble(splitAverage[0]) - cnvDecimalToOz;
        //  else
        //    average = Convert.ToDouble(splitAverage[0]) + cnvDecimalToOz;
        //}

        double rounded = Math.Round(average, 2, MidpointRounding.AwayFromZero);

        if (rounded > 0) //show user loss or gain
          return string.Format("Average loss of {0} lbs per entry", rounded);
        else
          return string.Format("Average gain of {0} lbs per entry", Math.Abs(rounded));

      }
    }

    /// <summary>
    /// Returns weight loss to date from list of weights passed
    /// </summary>
    /// <param name="weights"></param>
    /// <returns>string</returns>
    public string GetWeightLossToDate(string recentWeight, string startWeight, string system) {
      if (system == "Metric") {
        double startWeightCnv = Convert.ToDouble(startWeight);
        double recentWeightCnv = Convert.ToDouble(recentWeight);
        if (startWeightCnv == recentWeightCnv)
          return "You have not lost any weight so far";
        if (startWeightCnv < recentWeightCnv)
          return string.Format("You have gained {0} kg", (recentWeightCnv - startWeightCnv));
        if (startWeightCnv > recentWeightCnv)
          return string.Format("You have lost {0} kg", (startWeightCnv - recentWeightCnv));
        return "";
      } else {
        double startWeightCnv = Convert.ToDouble(startWeight);
        double recentWeightCnv = Convert.ToDouble(recentWeight);
        if (startWeightCnv == recentWeightCnv)
          return "You have not lost any weight so far";
        if (startWeightCnv < recentWeightCnv)
          return string.Format("You have gained {0} lbs", Math.Round((recentWeightCnv - startWeightCnv), 2, MidpointRounding.AwayFromZero));
        if (startWeightCnv > recentWeightCnv)
          return string.Format("You have lost {0} lbs", Math.Round((startWeightCnv - recentWeightCnv), 2, MidpointRounding.AwayFromZero));
        return "";
      }
    }

    /// <summary>
    /// Get the bmi status
    /// </summary>
    /// <param name="weight"></param>
    /// <returns>string</returns>
    public string GetBmiStatus(double bmi) {
      //BMI numbers as outlined here: https://www.rqhealth.ca/department/bariatric-surgical-program/bariatric-surgical-program-body-mass-index-bmi
      if (bmi < 18.5) {
        return "Underweight";
      } else if (bmi >= 18.5 && bmi <= 24.9) {
        return "Normal Weight";
      } else if (bmi >= 25.0 && bmi <= 29.9) {
        return "Overweight (Pre-obesity)";
      } else if (bmi >= 30.0 && bmi <= 34.9) {
        return "Overweight (Obesity class I)";
      } else if (bmi >= 35.0 && bmi <= 39.9) {
        return "Overweight (Obesity class II)";
      } else if (bmi >= 40.0) {
        return "Overweight (Obesity class III)";
      }
      return "";
    }

    /// <summary>
    /// Get the BMI message depending on BMI level sent
    /// </summary>
    /// <param name="bmi"></param>
    /// <returns>string</returns>
    public string GetBmiMessage(double bmi) {
      //BMI numbers as outlined here: https://www.rqhealth.ca/department/bariatric-surgical-program/bariatric-surgical-program-body-mass-index-bmi
      if (bmi < 18.5) {
        return "You are at an increased risk of developing health problems";
      } else if (bmi >= 18.5 && bmi <= 24.9) {
        return "You are at a healthy BMI level";
      } else if (bmi >= 25.0 && bmi <= 29.9) {
        return "You are at an increased risk of developing health problems";
      } else if (bmi >= 30.0 && bmi <= 34.9) {
        return "You are at an high risk of developing health problems";
      } else if (bmi >= 35.0 && bmi <= 39.9) {
        return "You are at an very high risk of developing health problems";
      } else if (bmi >= 40.0) {
        return "You are at an extremely high risk of developing health problems";
      }
      return "";
    }

    public string GetDaysTilGoalDate(DateTime goalDate) {
      double days = (goalDate - DateTime.Now).TotalDays;
      return Convert.ToInt32(days) + " days left until " + goalDate.ToShortDateString();
    }

    /// <summary>
    /// Convert lbs to Kg
    /// </summary>
    /// <param name="pounds"></param>
    /// <returns>double</returns>
    public ListviewTextLeftRight ConvertPoundsToKg(double pounds) {
      ListviewTextLeftRight item = new ListviewTextLeftRight();
      double kilograms = pounds * 0.45359237;
      double roundedKg = Math.Ceiling(kilograms);
      string[] splitKg = roundedKg.ToString().Split(".");

      item.TextRightSide = splitKg[0] + " kg";
      item.HiddenTextForConversion = splitKg[0].ToString();

      return item;
    }

    /// <summary>
    /// Convert lbs to kg
    /// </summary>
    /// <param name="kilograms"></param>
    /// <returns>ListviewTextLeftRight</returns>
    public ListviewTextLeftRight ConvertKgToPounds(double kilograms) {
      ListviewTextLeftRight item = new ListviewTextLeftRight();
      double pounds = kilograms * 2.20462262185;
      //string[] splitPounds = pounds.ToString().Split(".");
      string[] splitPounds = Math.Floor(pounds).ToString().Split(".");

      if (splitPounds.Length < 2) {
        item.TextRightSide = splitPounds[0] + " lbs";
        item.HiddenTextForConversion = splitPounds[0];
      } else {
        //double cvtPounds = Math.Round((Convert.ToDouble("." + splitPounds[1]) * .16), 2, MidpointRounding.AwayFromZero);
        //double finalWeight = Convert.ToDouble(splitPounds[0]) + cvtPounds;
        double finalWeight = Convert.ToDouble(splitPounds[0]);
        //string[] splitFinalWeight = finalWeight.ToString().Split(".");
        //if (splitFinalWeight.Length < 2) {
        item.TextRightSide = finalWeight + " lbs";
        item.HiddenTextForConversion = finalWeight.ToString();
        //} else {
        //  item.TextRightSide = splitFinalWeight[0] + " lbs " + splitFinalWeight[1] + " oz";
        //  item.HiddenTextForConversion = splitFinalWeight[0] + "." + splitFinalWeight[1];
        //}
      }
      return item;
    }

    /// <summary>
    /// Convert cm to ft/in
    /// </summary>
    /// <param name="centimeters"></param>
    /// <returns>ListviewTextLeftRight</returns>
    public ListviewTextLeftRight ConvertCmToFtIn(double centimeters) {
      ListviewTextLeftRight item = new ListviewTextLeftRight();
      double totalInches = centimeters / 2.54;
      int feet = Convert.ToInt32((totalInches - totalInches % 12) / 12);
      var roundedUp = Math.Ceiling(totalInches % 12);
      int inches = Convert.ToInt32(roundedUp);
      //int inches = Convert.ToInt32(totalInches % 12);

      item.TextRightSide = feet + " ft " + inches + " in";
      item.HiddenTextForConversion = feet + "." + inches;

      return item;
    }


    /// <summary>
    /// Convert imperial height to cm
    /// </summary>
    /// <param name="height"></param>
    /// <returns>double</returns>
    public ListviewTextLeftRight ConvertHeightToCm(string height) {
      //First, convert 5 feet to inches: 5 feet × 12 inches/foot = 60 inches
      //Add up our inches: 60 + 2 = 62 inches
      //Convert inches to cm: 62 inches × 2.54 cm / inch = 157.48 cm

      ListviewTextLeftRight item = new ListviewTextLeftRight();
      string[] splitHeight = height.Split(".");
      double inches = (Convert.ToDouble(splitHeight[0]) * 12) + Convert.ToDouble(splitHeight[1]);
      double cenimeters = inches * 2.54;

      item.TextRightSide = Math.Floor(cenimeters) + " cm";
      item.HiddenTextForConversion = Math.Ceiling(cenimeters).ToString();

      return item;
    }

    /// <summary>
    /// convert oz to a decimal format
    /// </summary>
    /// <param name="ounces"></param>
    /// <returns>double</returns>
    private double ConvertOuncesToDecimal(string ounces) {
      double ozDecimal = 0.00;
      switch (Convert.ToInt32(ounces)) {
        case 0:
          ozDecimal = 0.00;
          break;
        case 1:
          ozDecimal = 0.0625;
          break;
        case 2:
          ozDecimal = 0.125;
          break;
        case 3:
          ozDecimal = 0.1875;
          break;
        case 4:
          ozDecimal = 0.25;
          break;
        case 5:
          ozDecimal = 0.3125;
          break;
        case 6:
          ozDecimal = 0.375;
          break;
        case 7:
          ozDecimal = 0.4375;
          break;
        case 8:
          ozDecimal = 0.5;
          break;
        case 9:
          ozDecimal = 0.5625;
          break;
        case 10:
          ozDecimal = 0.625;
          break;
        case 11:
          ozDecimal = 0.6875;
          break;
        case 12:
          ozDecimal = 0.75;
          break;
        case 13:
          ozDecimal = 0.8125;
          break;
        case 14:
          ozDecimal = 0.875;
          break;
        case 15:
          ozDecimal = 0.9375;
          break;
        default:
          ozDecimal = 0;
          break;
      }
      return ozDecimal;
    }

    /// <summary>
    /// convert inches to a decimal format
    /// </summary>
    /// <param name="inches"></param>
    /// <returns>double</returns>
    private double ConvertInchesToDecimal(string inches) {
      double inDecimal = 0.00;
      switch (Convert.ToInt32(inches)) {
        case 0:
          inDecimal = 0.00;
          break;
        case 1:
          inDecimal = 0.0833;
          break;
        case 2:
          inDecimal = 0.1667;
          break;
        case 3:
          inDecimal = 0.25;
          break;
        case 4:
          inDecimal = 0.3333;
          break;
        case 5:
          inDecimal = 0.4167;
          break;
        case 6:
          inDecimal = 0.5;
          break;
        case 7:
          inDecimal = 0.5833;
          break;
        case 8:
          inDecimal = 0.6667;
          break;
        case 9:
          inDecimal = 0.75;
          break;
        case 10:
          inDecimal = 0.8333;
          break;
        case 11:
          inDecimal = 0.9167;
          break;
        default:
          inDecimal = 0;
          break;
      }
      return inDecimal;
    }

    /// <summary>
    /// Convert feet to inches
    /// </summary>
    /// <param name="height"></param>
    /// <returns>double</returns>
    private double ConvertFeetToInches(string height) {
     string[] splitHeight = height.Split(".");
      double feet = Convert.ToDouble(splitHeight[0]) * 12;
      double inches = Convert.ToDouble(splitHeight[1]);

      return feet + inches;
    }
  }
}