using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using DataAccessLayer.Dao;
using DataAccessLayer.Models;
using Google.Android.Material.Dialog;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextField;
using System;
using System.Collections.Generic;
using System.Linq;
using WeightApp.Adapters;

/*
* Ernie Phillips III : 12/09/2021
* Profile page, will pull and store in sqlite
*/

namespace WeightApp.Fragments {
  public class ProfileFragment : AndroidX.Fragment.App.Fragment {

    ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
    ProfileDao profileDao = new ProfileDao();
    ProfileListViewAdapter adapter;
    Profile profile = new Profile();
    ListView listView;

    public override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);

      // Create your fragment here
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
      // Use this to return your custom view for this Fragment
      View view = inflater.Inflate(Resource.Layout.fragment_profile, container, false);

      Button btnUpdateProfile = view.FindViewById<Button>(Resource.Id.btn_update_profile);

      listView = view.FindViewById<ListView>(Resource.Id.profile_listView);
      LoadData(); //clear any position index

      //set the listview item click
      listView.ItemClick += (s, eLV) => {
        //setting up a swith for the position selected to pull up a dialog box for user to make a selection depending
        switch (eLV.Position) {
          case 0: //WEIGHT OPTION
            View weightView = inflater.Inflate(Resource.Layout.dialog_spinner, container, false);

            //Number picker: https://medium.com/@sc71/android-numberpickers-3ef535c45487

            NumberPicker pckWeightPoundsNum = weightView.FindViewById<NumberPicker>(Resource.Id.dialog_spinner_number_picker_one);
            NumberPicker pckWeightOzNum = weightView.FindViewById<NumberPicker>(Resource.Id.dialog_spinner_number_picker_two);

            TextView txtWeightTextOne = weightView.FindViewById<TextView>(Resource.Id.dialog_spinner_text_one);
            TextView txtWeightTextTwo = weightView.FindViewById<TextView>(Resource.Id.dialog_spinner_text_two);
            txtWeightTextOne.Text = "lbs";
            txtWeightTextTwo.Text = "oz";

            //set the whole weight number
            string[] weightPoundNumbers = Enumerable.Range(1, 400).Select(x => x.ToString()).ToArray(); //create an array to 400 lbs
            pckWeightPoundsNum.MinValue = 1;
            pckWeightPoundsNum.MaxValue = weightPoundNumbers.Length;
            pckWeightPoundsNum.Value = 150; //set the start value
            pckWeightPoundsNum.SetDisplayedValues(weightPoundNumbers);

            //set the whole weight number
            string[] weightOzNumbers = Enumerable.Range(1, 16).Select(x => x.ToString()).ToArray(); //create an array to 400 lbs
            pckWeightOzNum.MinValue = 1;
            pckWeightOzNum.MaxValue = weightOzNumbers.Length;
            pckWeightOzNum.Value = 1; //set the start value
            pckWeightOzNum.SetDisplayedValues(weightOzNumbers);

            new MaterialAlertDialogBuilder(Activity).SetView(weightView)
              .SetTitle("What's your current weight?")
              .SetMessage("")
              .SetNegativeButton("Cancel", (s, e) => { })
              .SetPositiveButton("OK", (sender, e) => {

                var selectedLbs = pckWeightPoundsNum.Value;
                var selectedOz = pckWeightOzNum.Value;

                adapter.SetSelectedTextValue(eLV.Position, selectedLbs + "." + selectedOz + " lbs", selectedLbs + "." + selectedOz);
              })
              .Show();
            break;
          case 1://HEIGHT OPTION
            View heightView = inflater.Inflate(Resource.Layout.dialog_spinner, container, false);

            //Number picker: https://medium.com/@sc71/android-numberpickers-3ef535c45487

            NumberPicker pckHeightFtNum = heightView.FindViewById<NumberPicker>(Resource.Id.dialog_spinner_number_picker_one);
            NumberPicker pckHeightInNum = heightView.FindViewById<NumberPicker>(Resource.Id.dialog_spinner_number_picker_two);

            TextView txtHeightTextOne = heightView.FindViewById<TextView>(Resource.Id.dialog_spinner_text_one);
            TextView txtHeightTextTwo = heightView.FindViewById<TextView>(Resource.Id.dialog_spinner_text_two);
            txtHeightTextOne.Text = "ft";
            txtHeightTextTwo.Text = "in";

            //set the whole weight number
            string[] heightFeetNumbers = Enumerable.Range(1, 8).Select(x => x.ToString()).ToArray(); //create an array to 400 lbs
            pckHeightFtNum.MinValue = 1;
            pckHeightFtNum.MaxValue = heightFeetNumbers.Length;
            pckHeightFtNum.Value = 5; //set the start value
            pckHeightFtNum.SetDisplayedValues(heightFeetNumbers);

            //set the whole weight number
            string[] weightInchNumbers = Enumerable.Range(1, 15).Select(x => x.ToString()).ToArray(); //create an array to 400 lbs
            pckHeightInNum.MinValue = 1;
            pckHeightInNum.MaxValue = weightInchNumbers.Length;
            pckHeightInNum.Value = 1; //set the start value
            pckHeightInNum.SetDisplayedValues(weightInchNumbers);

            new MaterialAlertDialogBuilder(Activity).SetView(heightView)
              .SetTitle("What's your height?")
              .SetMessage("")
              .SetNegativeButton("Cancel", (s, e) => { })
              .SetPositiveButton("OK", (sender, e) => {

                var selectedFt = pckHeightFtNum.Value;
                var selectedIn = pckHeightInNum.Value;

                adapter.SetSelectedTextValue(eLV.Position, selectedFt + " ft " + selectedIn + " in", selectedFt + "." + selectedIn);
              })
              .Show();
            break;
          case 2:
            new MaterialAlertDialogBuilder(Activity)
              .SetTitle("Select your gender")
              .SetMessage("")
              .SetPositiveButton("OK", (sender, e) => {
              })
              .Show();
            break;
          case 3:
            new MaterialAlertDialogBuilder(Activity)
              .SetTitle("Input your goal weight")
              .SetMessage("")
              .SetPositiveButton("OK", (sender, e) => { })
              .Show();
            break;
          case 4:
            new MaterialAlertDialogBuilder(Activity)
              .SetTitle("Input your goal weight date")
              .SetMessage("")
              .SetPositiveButton("OK", (sender, e) => { })
              .Show();
            break;
        }
      };

      btnUpdateProfile.Click += (s, e) => {
        #region VALIDATION
        //provide validation check on fields and return dialog with missing
        List<ProfileListview> profileListviews = adapter.ValidateProfile();
        string error = "";
        foreach (ProfileListview profileItem in profileListviews) {
          if (profileItem.TextRightSide == "N/a") {
            error += profileItem.TextLeftSide + " is required.\n";
          }
        }
        if (error != "") {
          new MaterialAlertDialogBuilder(Activity)
             .SetTitle("Weight App Alert")
             .SetMessage(error)
             .SetPositiveButton("OK", (sender, e) => { })
             .Show();
        }
        #endregion
      };

      return view;
    }

    private void LoadData() {
      string userId = pref.GetString("UserId", String.Empty);
      profile = profileDao.GetProfileByUserId(Convert.ToInt32(userId));

      List<ProfileListview> profileItems;

      if (profile == null) {
        profileItems = new List<ProfileListview>() {
          new ProfileListview{ Id = 1, TextLeftSide = "Weight", TextRightSide = "N/a" },
          new ProfileListview{ Id = 2, TextLeftSide = "Height", TextRightSide = "N/a" },
          new ProfileListview{ Id = 3, TextLeftSide = "Gender", TextRightSide = "N/a" },
          new ProfileListview{ Id = 4, TextLeftSide = "Goal Weight", TextRightSide = "N/a" },
          new ProfileListview{ Id = 5, TextLeftSide = "Goal Target Date", TextRightSide = "N/a" }
        };
      } else {
        profileItems = new List<ProfileListview>() {
          new ProfileListview{ Id = 1, TextLeftSide = "Weight", TextRightSide = profile.START_WEIGHT.ToString() },
          new ProfileListview{ Id = 2, TextLeftSide = "Height", TextRightSide = profile.HEIGHT.ToString() },
          new ProfileListview{ Id = 3, TextLeftSide = "Gender", TextRightSide = profile.GENDER },
          new ProfileListview{ Id = 4, TextLeftSide = "Goal Weight", TextRightSide = profile.TARGET_WEIGHT.ToString() },
          new ProfileListview{ Id = 5, TextLeftSide = "Goal Target Date", TextRightSide = profile.TARGET_DATE.ToShortDateString() }
        };
      }

      adapter = new ProfileListViewAdapter(this, profileItems);

      // set new items
      listView.Adapter = adapter;
    }
  }
}