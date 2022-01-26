using Android.Graphics;
using Android.OS;
using Android.Views;
using DataAccessLayer.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    WeightDao weightDao = new WeightDao();
    ProfileDao profileDao = new ProfileDao();
    Profile profile;
    List<Weight> weights;

    public override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);

      // Create your fragment here
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
      // Use this to return your custom view for this Fragment
      View view = inflater.Inflate(Resource.Layout.fragment_statistics, container, false);

      ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
      string userId = pref.GetString("UserId", String.Empty);
      profile = profileDao.GetProfileByUserId(Convert.ToInt32(userId));

      if (profile == null) {
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
        weights = weightDao.GetWeightsByProfileIdOrderByDateAsc(profile.PROFILE_ID);
        Weight mostRecentEntry = weightDao.GetWeightsByProfileIdMostRecentDate(profile.PROFILE_ID);

        TextView txtCurrentWeight = view.FindViewById<TextView>(Resource.Id.txt_current_weight);
        TextView txtBmi = view.FindViewById<TextView>(Resource.Id.txt_bmi);
        TextView txtBmiStatus = view.FindViewById<TextView>(Resource.Id.txt_bmi_status);
        TextView txtBmiMessage = view.FindViewById<TextView>(Resource.Id.txt_bmi_message);
        TextView txtAverageLoss = view.FindViewById<TextView>(Resource.Id.txt_average_weekly_weight_loss);
        TextView txtLossToDate = view.FindViewById<TextView>(Resource.Id.txt_weight_loss_to_date);

        Utilities.Calculations calculations = new Utilities.Calculations();

        if (profile != null) {
          string currentWeight;
          double bmiNumber;
          if (mostRecentEntry != null) { //grab user's most recent weight entry
            bmiNumber = calculations.GetBmi(mostRecentEntry.WEIGHT_ENTRY, profile.HEIGHT);
            currentWeight = mostRecentEntry.WEIGHT_ENTRY;
          } else { //no entries, use profile weight
            bmiNumber = calculations.GetBmi(profile.START_WEIGHT, profile.HEIGHT);
            currentWeight = profile.START_WEIGHT;
          }

          if (weights.Count > 1) {
            var weightsDesc = weightDao.GetWeightsByProfileIdOrderByDateDesc(profile.PROFILE_ID);
            txtLossToDate.Text = calculations.GetWeightLossToDate(mostRecentEntry.WEIGHT_ENTRY, profile.START_WEIGHT);
            txtAverageLoss.Text = calculations.GetAverageWeeklyWeightLoss(weightsDesc, profile.START_WEIGHT).ToString();
          } else if (weights.Count == 1) {
            string w = (Convert.ToDouble(profile.START_WEIGHT) - Convert.ToDouble(weights[0].WEIGHT_ENTRY)).ToString() + " lbs";
            txtAverageLoss.Text = w;
            txtLossToDate.Text = w;
          } else {
            txtAverageLoss.Text = "0";
            txtLossToDate.Text = "0";
          }

          txtCurrentWeight.Text = currentWeight + " lbs";
          txtBmi.Text = "Current #: " + bmiNumber.ToString();
          txtBmiStatus.Text = calculations.GetBmiStatus(bmiNumber);
          txtBmiMessage.Text = calculations.GetBmiMessage(bmiNumber);
        }





        //https://www.c-sharpcorner.com/blogs/xamarin-android-microcharts
        double checkWeight = 1000;
        List<Entry> entries = new List<Entry>();
        foreach (Weight weight in weights) {
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


        ChartView lineChart = view.FindViewById<ChartView>(Resource.Id.line_chart);
        var lineChartEntries = new LineChart() {
          LabelTextSize = 21f,
          Entries = entries
        };
        lineChart.Chart = lineChartEntries;
      }

      return view;
    }
  }
}