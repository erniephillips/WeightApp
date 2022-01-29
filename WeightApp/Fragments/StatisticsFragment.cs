using Android.OS;
using Android.Views;
using DataAccessLayer.Models;
using System.Collections.Generic;
using Microcharts;
using SkiaSharp;
using Entry = Microcharts.ChartEntry;
using Microcharts.Droid;
using DataAccessLayer.Dao;
using System;
using Android.Content;
using Android.App;
using Android.Widget;
using Google.Android.Material.Dialog;

/*
* Ernie Phillips III : 12/09/2021
* Statistics page, will pull by user's stored records
*/

namespace WeightApp.Fragments {
  public class StatisticsFragment : AndroidX.Fragment.App.Fragment {
    //instantiate weight & profile dao
    WeightDao weightDao = new WeightDao();
    ProfileDao profileDao = new ProfileDao();

    //set vars
    Profile profile;
    List<Weight> weights;

    public override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);
      // Create your fragment here
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
      // Use this to return your custom view for this Fragment
      View view = inflater.Inflate(Resource.Layout.fragment_statistics, container, false);

      //get stored user account info
      ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
      string userId = pref.GetString("UserId", String.Empty);

      //get the user's profile info
      profile = profileDao.GetProfileByUserId(Convert.ToInt32(userId));

      if (profile == null) { //if null, user needs to fill out profile, show modal that redirects
        new MaterialAlertDialogBuilder(Activity)
          .SetTitle("Weight App Alert")
          .SetIcon(Resource.Drawable.ic_info)
          .SetMessage("Let's get your profile filled out so you can visit this page.")
          .SetPositiveButton("OK", (sender, e) => {
            this.FragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, new ProfileFragment(), "Fragment").Commit();
          })
          .SetCancelable(false)
          .Show();
      } else {
        //get list of weights by ascending order
        weights = weightDao.GetWeightsByProfileIdOrderByDateAsc(profile.PROFILE_ID);

        //get the most recent entry
        Weight mostRecentEntry = weightDao.GetWeightsByProfileIdMostRecentDate(profile.PROFILE_ID);

        //set the vars from XML
        TextView txtCurrentWeight = view.FindViewById<TextView>(Resource.Id.txt_current_weight);
        TextView txtStartWeight = view.FindViewById<TextView>(Resource.Id.txt_start_weight);
        TextView txtGoalWeight = view.FindViewById<TextView>(Resource.Id.txt_goal_weight);
        TextView txtBmi = view.FindViewById<TextView>(Resource.Id.txt_bmi);
        TextView txtBmiStatus = view.FindViewById<TextView>(Resource.Id.txt_bmi_status);
        TextView txtBmiMessage = view.FindViewById<TextView>(Resource.Id.txt_bmi_message);
        TextView txtAverageLoss = view.FindViewById<TextView>(Resource.Id.txt_average_weekly_weight_loss);
        TextView txtLossToDate = view.FindViewById<TextView>(Resource.Id.txt_weight_loss_to_date);
        TextView txtGoalDate = view.FindViewById<TextView>(Resource.Id.txt_goal_date);

        //instantiate calculations class for methods
        Utilities.Calculations calculations = new Utilities.Calculations();

        if (profile != null) { //just an extra check to ensure before passing all prof vars
          string currentWeight;
          double bmiNumber;
          if (mostRecentEntry != null) { //grab user's most recent weight entry
            bmiNumber = calculations.GetBmi(mostRecentEntry.WEIGHT_ENTRY, profile.HEIGHT, profile.MEASUREMENT_SYSTEM); //set the BMI
            currentWeight = mostRecentEntry.WEIGHT_ENTRY; //set the current weight
          } else { //no entries, use profile weight
            bmiNumber = calculations.GetBmi(profile.START_WEIGHT, profile.HEIGHT, profile.MEASUREMENT_SYSTEM); //set the BMI
            currentWeight = profile.START_WEIGHT; //set the weight
          }

          if (weights.Count > 1) { //list of weights to work with
            txtLossToDate.Text = calculations.GetWeightLossToDate(mostRecentEntry.WEIGHT_ENTRY, profile.START_WEIGHT, profile.MEASUREMENT_SYSTEM);
            txtAverageLoss.Text = calculations.GetAverageWeeklyWeightLoss(weights, profile.START_WEIGHT, profile.MEASUREMENT_SYSTEM).ToString();
          } else if (weights.Count == 1) { //1 entry exists
            string weight = (Convert.ToDouble(profile.START_WEIGHT) - Convert.ToDouble(weights[0].WEIGHT_ENTRY)).ToString();
            if (profile.MEASUREMENT_SYSTEM == "Metric") {
              txtAverageLoss.Text = weight + " kg";
              txtLossToDate.Text = weight + " kg";
            } else {
              txtAverageLoss.Text = weight + " lbs";
              txtLossToDate.Text = weight + " lbs";
            }
          } else { //no entries found
            if (profile.MEASUREMENT_SYSTEM == "Metric") {
              txtAverageLoss.Text = "0 kg";
              txtLossToDate.Text = "0 kg";
            } else {
              txtAverageLoss.Text = "0 lbs";
              txtLossToDate.Text = "0 lbs";
            }
          }

          //set the cards textview on the front end
          if (profile.MEASUREMENT_SYSTEM == "Metric") {
            txtCurrentWeight.Text = "Current: " + currentWeight + " kg";
            txtStartWeight.Text = "Start: " + profile.START_WEIGHT + " kg";
            txtGoalWeight.Text = "Goal: " + profile.TARGET_WEIGHT + " kg";
          } else {
            txtCurrentWeight.Text = "Current: " + currentWeight + " lbs";
            txtStartWeight.Text = "Start: " + profile.START_WEIGHT + " lbs";
            txtGoalWeight.Text = "Goal: " + profile.TARGET_WEIGHT + " lbs";
          }
          txtBmi.Text = "Current #: " + bmiNumber.ToString();
          txtBmiStatus.Text = calculations.GetBmiStatus(bmiNumber);
          txtBmiMessage.Text = calculations.GetBmiMessage(bmiNumber);
          txtGoalDate.Text = calculations.GetDaysTilGoalDate(profile.TARGET_DATE);
        }


        //https://www.c-sharpcorner.com/blogs/xamarin-android-microcharts
        //Create a chart to display list of weights and associated date
        double checkWeight = 1000;
        List<Entry> entries = new List<Entry>(); //declare new entry

        //insert the user's profile start weight
        weights.Insert(0, new Weight() { WEIGHT_ENTRY = profile.START_WEIGHT, DATE_ENTRY = profile.START_DATE });

        //loop through list of weights
        foreach (Weight weight in weights) {
          //display red for down trend and blue for up trend of weight entry
          if (Convert.ToDouble(weight.WEIGHT_ENTRY) < checkWeight) {
            checkWeight = Convert.ToDouble(weight.WEIGHT_ENTRY);
            entries.Add(
              new Entry(float.Parse(weight.WEIGHT_ENTRY)) {
                Color = SKColor.Parse("#004aad"),
                Label = weight.DATE_ENTRY.ToShortDateString(),
                ValueLabel = weight.WEIGHT_ENTRY
              });
          } else {
            checkWeight = Convert.ToDouble(weight.WEIGHT_ENTRY);
            entries.Add(new Entry(float.Parse(weight.WEIGHT_ENTRY)) {
              Color = SKColor.Parse("#FF1943"),
              Label = weight.DATE_ENTRY.ToShortDateString(),
              ValueLabel = weight.WEIGHT_ENTRY
            });
          }

        }

        //find the XML chart view
        ChartView lineChart = view.FindViewById<ChartView>(Resource.Id.line_chart);
        var lineChartEntries = new LineChart() {
          LabelTextSize = 21f,
          Entries = entries
        };
        //set the chart
        lineChart.Chart = lineChartEntries;
      }

      return view;
    }
  }
}