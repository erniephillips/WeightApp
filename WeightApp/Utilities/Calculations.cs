using DataAccessLayer.Models;
using System;
using System.Collections.Generic;

namespace WeightApp.Utilities {
  public class Calculations {
    //calcuations: https://www.cdc.gov/nccdphp/dnpao/growthcharts/training/bmiage/page5_2.html
    //Weight of 37 lbs and 4 oz = 37.25 lbs (16 ounces = 1 pound so 4 oz/16 oz = 0.25).
    //(37.25 lbs / 41.5 in / 41.5 in) x 703 = 15.2
    public double GetBmi(string weight, string height) {
      string[] splitWeight = weight.Split(".");
      string[] splitHeight = height.Split(".");

      double convertWeight = Convert.ToDouble(splitWeight[0]) + ConvertOuncesToDecimal(splitWeight[1]);
      double convertHeight = ConvertFeetToInches(height);
      //double BMI = (convertWeight / (convertHeight * convertHeight)) * 703;
      double BMI = 703.0 * convertWeight / (convertHeight * convertHeight);

      // [weight (kg) / height (cm) / height (cm)] x 10,000
      //double kg = ConvertPoundsToKg(convertWeight);
      //double c = ConvertHeightToCm(convertHeight);
      //double meters = ConvertHeightToMeters(Convert.ToDouble(splitHeight[0]), Convert.ToDouble(splitHeight[1]));
      ////double test = (kg / c / c) * 10000;
      //double test = kg / ((meters / 100) * (meters / 100));
      double bmi = (200.0 / Math.Pow(68.4, 2)) * 703.0;

      return bmi;
    }

    private double ConvertPoundsToKg(double pounds) {
      double kilograms = pounds * 0.45359237;
      return kilograms;
    }

    private double ConvertHeightToMeters(double feet, double inches) {
      double meters = feet * 0.3048 + inches * 0.0254;
      return meters;
    }

    private double ConvertHeightToCm(double height) {
      double cenimeters = height * 2.54;
      return cenimeters;
    }

    //convert oz to a decimal format
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

    //convert inches to a decimal format
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

    private double ConvertFeetToInches(string height) {
      var inches = Convert.ToDouble(height) * 12;
      return inches;
    }
  }
}