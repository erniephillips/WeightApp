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
        Utilities.Calculations calculations = new Utilities.Calculations();
        //var value = calculations.GetBmi(mostRecentEntry.WEIGHT_ENTRY, profile.HEIGHT);
        var value = calculations.GetBmi("275.0", "5.5");

        TextView txt = view.FindViewById<TextView>(Resource.Id.txt_bmi);
        txt.Text = "BMI: " + value.ToString();

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