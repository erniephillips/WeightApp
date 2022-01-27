using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WeightApp.Utilities {
  public class Calculations {

    /// <summary>
    /// //calcuations: https://www.cdc.gov/nccdphp/dnpao/growthcharts/training/bmiage/page5_2.html
    ///Weight of 37 lbs and 4 oz = 37.25 lbs (16 ounces = 1 pound so 4 oz/16 oz = 0.25).
    ///(37.25 lbs / 41.5 in / 41.5 in) x 703 = 15.2
    /// </summary>
    /// <param name="weight"></param>
    /// <param name="height"></param>
    /// <returns>double bmi weight</returns>
    public double GetBmi(string weight, string height) {
      string[] splitWeight = weight.Split(".");
      string[] splitHeight = height.Split(".");

      double convertWeight = Convert.ToDouble(splitWeight[0]) + ConvertOuncesToDecimal(splitWeight[1]);
      double convertHeight = ConvertFeetToInches(height);
      //double BMI = (convertWeight / (convertHeight * convertHeight)) * 703;
      double BMI = 703.0 * convertWeight / Math.Pow(convertHeight, 2);
      //double bmi = (200.0 / Math.Pow(68.4, 2)) * 703.0;

      double roundedBmi = Math.Round(BMI, 2, MidpointRounding.AwayFromZero);

      return roundedBmi;
    }

    /// <summary>
    /// Returns average weekly weight loss
    /// </summary>
    /// <param name="weights"></param>
    /// <returns>double</returns>
    public double GetAverageWeeklyWeightLoss(List<Weight> weights, string startWeight) {
      //https://stackoverflow.com/questions/10284133/sum-range-of-ints-in-listint
      //https://stackoverflow.com/questions/16732206/how-to-cast-the-listobject-to-listt

      double lastWeight = Convert.ToDouble(startWeight);
      double sum = 0;
      double average = 0;
      
      foreach(Weight w in weights) {
        sum += lastWeight - Convert.ToDouble(w.WEIGHT_ENTRY);
        lastWeight = Convert.ToDouble(w.WEIGHT_ENTRY);
      }
      average = sum / (weights.Count + 1);
      double rounded = Math.Round(average, 2, MidpointRounding.AwayFromZero);
      return rounded;

      ////possibly causing my end stream error
      ////List<double> newWeights = weights.Select(s => Convert.ToDouble(s.WEIGHT_ENTRY)).ToList();

      //List<double> newWeights = new List<double>();
      //foreach(Weight w in weights) {
      //  newWeights.Add(Convert.ToDouble(w.WEIGHT_ENTRY));
      //}

      //double sum = newWeights.Sum(); //get the sum

      //return 0;
    }

    /// <summary>
    /// Returns weight loss to date from list of weights passed
    /// </summary>
    /// <param name="weights"></param>
    /// <returns>string</returns>
    public string GetWeightLossToDate(string recentWeight, string startWeight) {
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pounds"></param>
    /// <returns></returns>
    private double ConvertPoundsToKg(double pounds) {
      double kilograms = pounds * 0.45359237;
      return kilograms;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="feet"></param>
    /// <param name="inches"></param>
    /// <returns></returns>
    private double ConvertHeightToMeters(double feet, double inches) {
      double meters = feet * 0.3048 + inches * 0.0254;
      return meters;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="height"></param>
    /// <returns></returns>
    private double ConvertHeightToCm(double height) {
      double cenimeters = height * 2.54;
      return cenimeters;
    }

    /// <summary>
    /// convert oz to a decimal format
    /// </summary>
    /// <param name="ounces"></param>
    /// <returns></returns>
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
    /// <returns></returns>
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
    /// 
    /// </summary>
    /// <param name="height"></param>
    /// <returns></returns>
    private double ConvertFeetToInches(string height) {
      var inches = Convert.ToDouble(height) * 12;
      return inches;
    }
  }
}