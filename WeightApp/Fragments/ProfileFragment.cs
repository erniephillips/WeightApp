using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using DataAccessLayer.Dao;
using DataAccessLayer.Models;
using Google.Android.Material.Dialog;
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
    //get the stored userinfo 
    ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);

    //instantiate the profile dao
    ProfileDao profileDao = new ProfileDao();

    //declare listview adapter for list of profile items
    ListViewTextLeftRightAdapter adapter;

    //set vars
    Profile profile = new Profile();
    ListView listView;

    //creation of menu. Set to not display delete button if not incoming record
    public override void OnCreateOptionsMenu(Android.Views.IMenu menu, MenuInflater inflater) {
      inflater.Inflate(Resource.Menu.menu_save, menu);
      base.OnCreateOptionsMenu(menu, inflater);
    }

    //handle the menu click
    public override bool OnOptionsItemSelected(IMenuItem menu) {
      menu.SetChecked(true);
      switch (menu.ItemId) //check by clicked menu item
      {
        #region SAVE BUTTON CLICK
        case Resource.Id.menu_save:
          #region VALIDATION
          //provide validation check on fields and return dialog with missing
          string error = "";
          List<ListviewTextLeftRight> ListviewTextLeftRights = adapter.GetItems();
          foreach (ListviewTextLeftRight profileItem in ListviewTextLeftRights) {
            if (profileItem.TextRightSide == "N/a") {
              error += profileItem.TextLeftSide + " is required.\n";
            }
          }
          if (error != "") {
            new MaterialAlertDialogBuilder(Activity)
               .SetTitle("Weight App Alert")
               .SetIcon(Resource.Drawable.ic_info)
               .SetMessage(error)
               .SetPositiveButton("OK", (sender, e) => { })
               .Show();
            return true;
          }
          #endregion

          //everything validated, update profile
          string userId = pref.GetString("UserId", String.Empty);

          //initialize a new profile object
          Profile profile = new Profile() {
            USER_ID = Convert.ToInt32(userId),
            START_DATE = DateTime.Now
          };

          //get each list item rightside textview value and set fields to profile object
          foreach (ListviewTextLeftRight profileItem in ListviewTextLeftRights) {
            if (profileItem.TextLeftSide == "Weight")
              profile.START_WEIGHT = profileItem.HiddenTextForConversion;
            if (profileItem.TextLeftSide == "Height")
              profile.HEIGHT = profileItem.HiddenTextForConversion;
            if (profileItem.TextLeftSide == "Gender")
              profile.GENDER = profileItem.HiddenTextForConversion;
            if (profileItem.TextLeftSide == "Goal Weight")
              profile.TARGET_WEIGHT = profileItem.HiddenTextForConversion;
            if (profileItem.TextLeftSide == "Goal Date")
              profile.TARGET_DATE = DateTime.Parse(profileItem.HiddenTextForConversion);
          }

          //add to database if doesn't exist otherwise update
          Profile tempProfile = profileDao.GetProfileByUserId(Convert.ToInt32(userId));
          if (tempProfile == null)
            try { //add
              profileDao.AddProfile(profile);
            } catch (Exception ex) {
              new MaterialAlertDialogBuilder(Activity)
              .SetTitle("An error has occurred. Please contact the app administrator. Exception: " + ex.Message)
              .SetPositiveButton("OK", (sender, e) => { })
              .Show();
            }
          else {
            tempProfile.START_WEIGHT = profile.START_WEIGHT;
            tempProfile.HEIGHT = profile.HEIGHT;
            tempProfile.GENDER = profile.GENDER;
            tempProfile.TARGET_WEIGHT = profile.TARGET_WEIGHT;
            tempProfile.TARGET_DATE = profile.TARGET_DATE;

            try { //update
              profileDao.UpdateProfile(tempProfile);
            } catch (Exception ex) {
              new MaterialAlertDialogBuilder(Activity)
              .SetTitle("An error has occurred. Please contact the app administrator. Exception: " + ex.Message)
              .SetPositiveButton("OK", (sender, e) => { })
              .Show();
            }
          }

          //redirect user to weight entry
          this.FragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, new StatisticsFragment(), "Fragment").Commit();
          return true;
          #endregion
      }
      return base.OnOptionsItemSelected(menu);

    }

    public override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);
      //show the options menu
      HasOptionsMenu = true;
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
      // Use this to return your custom view for this Fragment
      View view = inflater.Inflate(Resource.Layout.fragment_profile, container, false);

      //set the listview by XML
      listView = view.FindViewById<ListView>(Resource.Id.profile_listView);
      
      //load listview data
      LoadData(); 

      //set the listview item click
      listView.ItemClick += (s, eLV) => {
        //setting up a swith for the position selected to pull up a dialog box for user to make a selection depending
        switch (eLV.Position) {
          #region WEIGHT OPTION
          case 0:
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
            pckWeightPoundsNum.Value = profile != null ? pckWeightPoundsNum.Value = Convert.ToInt32(profile.START_WEIGHT.Split(".")?[0]) : 150;
            pckWeightPoundsNum.SetDisplayedValues(weightPoundNumbers);

            //set the whole weight number
            string[] weightOzNumbers = Enumerable.Range(0, 17).Select(x => x.ToString()).ToArray(); //create an array to 400 lbs
            pckWeightOzNum.MinValue = 1;
            pckWeightOzNum.MaxValue = weightOzNumbers.Length - 1;
            pckWeightOzNum.Value = profile != null ? Convert.ToInt32(profile.START_WEIGHT.Split(".")?[1]) + 1 : 1; //set the start value
            pckWeightOzNum.SetDisplayedValues(weightOzNumbers);

            new MaterialAlertDialogBuilder(Activity).SetView(weightView)
              .SetTitle("What's your current weight?")
              .SetNegativeButton("Cancel", (s, e) => { })
              .SetPositiveButton("OK", (sender, e) => {

                var selectedLbs = pckWeightPoundsNum.Value;
                var selectedOz = pckWeightOzNum.Value - 1;

                adapter.SetSelectedTextValue(
                  eLV.Position,
                  selectedLbs + " lbs " + selectedOz + " oz",
                  selectedLbs + "." + selectedOz);
              })
              .Show();
            break;
          #endregion
          #region HEIGHT OPTION
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
            pckHeightFtNum.Value = profile != null ? Convert.ToInt32(profile.HEIGHT.Split(".")?[0]) : 5; //set the start value
            pckHeightFtNum.SetDisplayedValues(heightFeetNumbers);

            //set the whole weight number
            string[] weightInchNumbers = Enumerable.Range(0, 13).Select(x => x.ToString()).ToArray(); //create an array to 400 lbs
            pckHeightInNum.MinValue = 1;
            pckHeightInNum.MaxValue = weightInchNumbers.Length - 1;
            pckHeightInNum.Value = profile != null ? Convert.ToInt32(profile.HEIGHT.Split(".")?[1]) + 1 : 1; //set the start value
            pckHeightInNum.SetDisplayedValues(weightInchNumbers);

            new MaterialAlertDialogBuilder(Activity).SetView(heightView)
              .SetTitle("What's your height?")
              .SetNegativeButton("Cancel", (s, e) => { })
              .SetPositiveButton("OK", (sender, e) => {

                var selectedFt = pckHeightFtNum.Value;
                var selectedIn = pckHeightInNum.Value - 1;

                adapter.SetSelectedTextValue(
                  eLV.Position,
                  selectedFt + " ft " + selectedIn + " in",
                  selectedFt + "." + selectedIn);
              })
              .Show();
            break;
          #endregion
          #region GENDER OPTION
          case 2:
            View goalGenderView = inflater.Inflate(Resource.Layout.dialog_gender, container, false);

            RadioGroup rdgGender = goalGenderView.FindViewById<RadioGroup>(Resource.Id.radio_gender_group);

            //populate the dropdown if profile gender exists
            if (profile != null) {
              int count = rdgGender.ChildCount;
              for (int i = 0; i < count; i++) {
                View o = rdgGender.GetChildAt(i);
                if (o is RadioButton) {
                  RadioButton rdBtn = (RadioButton)o;
                  if (rdBtn.Text == profile.GENDER)
                    rdBtn.Checked = true;
                }
              }
            }

            new MaterialAlertDialogBuilder(Activity).SetView(goalGenderView)
              .SetTitle("Select your gender")
              .SetPositiveButton("OK", (sender, e) => {
                RadioButton radioGenderButton = goalGenderView.FindViewById<RadioButton>(rdgGender.CheckedRadioButtonId);
                adapter.SetSelectedTextValue(eLV.Position, radioGenderButton.Text, radioGenderButton.Text);
              }).Show();
            break;
          #endregion
          #region GOAL WEIGHT OPTION
          case 3:
            View goalWeightView = inflater.Inflate(Resource.Layout.dialog_spinner, container, false);

            //Number picker: https://medium.com/@sc71/android-numberpickers-3ef535c45487

            NumberPicker pckGoalWeightPoundsNum = goalWeightView.FindViewById<NumberPicker>(Resource.Id.dialog_spinner_number_picker_one);
            NumberPicker pckGoalWeightOzNum = goalWeightView.FindViewById<NumberPicker>(Resource.Id.dialog_spinner_number_picker_two);

            TextView txtGoalWeightTextOne = goalWeightView.FindViewById<TextView>(Resource.Id.dialog_spinner_text_one);
            TextView txtGoalWeightTextTwo = goalWeightView.FindViewById<TextView>(Resource.Id.dialog_spinner_text_two);
            txtGoalWeightTextOne.Text = "lbs";
            txtGoalWeightTextTwo.Text = "oz";

            //set the whole weight number
            string[] goalWeightPoundNumbers = Enumerable.Range(1, 400).Select(x => x.ToString()).ToArray(); //create an array to 400 lbs
            pckGoalWeightPoundsNum.MinValue = 1;
            pckGoalWeightPoundsNum.MaxValue = goalWeightPoundNumbers.Length;
            pckGoalWeightPoundsNum.Value = profile != null ? Convert.ToInt32(profile.TARGET_WEIGHT.Split(".")?[0]) : 150; //set the start value
            pckGoalWeightPoundsNum.SetDisplayedValues(goalWeightPoundNumbers);

            //set the whole weight number
            string[] goalWeightOzNumbers = Enumerable.Range(0, 17).Select(x => x.ToString()).ToArray(); //create an array to 400 lbs
            pckGoalWeightOzNum.MinValue = 1;
            pckGoalWeightOzNum.MaxValue = goalWeightOzNumbers.Length - 1;
            pckGoalWeightOzNum.Value = profile != null ? Convert.ToInt32(profile.TARGET_WEIGHT.Split(".")?[1]) + 1 : 1; //set the start value
            pckGoalWeightOzNum.SetDisplayedValues(goalWeightOzNumbers);

            new MaterialAlertDialogBuilder(Activity).SetView(goalWeightView)
              .SetTitle("What's your goal weight?")
              .SetNegativeButton("Cancel", (s, e) => { })
              .SetPositiveButton("OK", (sender, e) => {

                var selectedLbs = pckGoalWeightPoundsNum.Value;
                var selectedOz = pckGoalWeightOzNum.Value - 1;

                adapter.SetSelectedTextValue(
                  eLV.Position,
                  selectedLbs + " lbs " + selectedOz + " oz",
                  selectedLbs + "." + selectedOz);
              })
              .Show();
            break;
          #endregion
          #region GOAL DATE OPTION
          case 4:
            //I can't seem to expose the onpositivebutton click which I need. Will need android calendar
            //MaterialDatePicker datePicker = MaterialDatePicker.Builder.DatePicker()
            //      .SetTitleText("What is your goal date for your goal weight?")
            //      .Build();
            //datePicker.Show(FragmentManager, "");

            //Capture onclick for OK button: https://stackoverflow.com/questions/49009155/xamarin-forms-android-datepicker-timepicker-button-listener

            DatePickerDialog datePicker = new DatePickerDialog(Context);

            if (profile != null) {
              datePicker.DatePicker.DateTime = profile.TARGET_DATE;
            }

            datePicker.SetButton((int)DialogButtonType.Positive, Context.Resources.GetString(global::Android.Resource.String.Ok), (s, e) => {
              //int selectedDay = datePicker.DatePicker.DayOfMonth;
              //int selectedMonth = datePicker.DatePicker.Month + 1; //the months seem to be indexed at zero so I need to add 1
              //int selectedYear = datePicker.DatePicker.Year;

              //can just access whole date by calling datetime
              DateTime selectedDate = datePicker.DatePicker.DateTime;

              if (selectedDate < DateTime.Now) {
                new MaterialAlertDialogBuilder(Activity)
               .SetTitle("Weight App Alert?")
               .SetIcon(Resource.Drawable.ic_info)
               .SetMessage("Date must be greater than today's date")
               .SetPositiveButton("OK", (sender, e) => { })
               .Show();
              } else { //date passed check
                adapter.SetSelectedTextValue(eLV.Position, selectedDate.ToShortDateString(), selectedDate.ToString());
              }
            });
            datePicker.Show();

            break;
            #endregion
        }
      };

      //btnUpdateProfile.Click += (s, e) => {

      //};

      return view;
    }

    private void LoadData() {
      string userId = pref.GetString("UserId", String.Empty);
      profile = profileDao.GetProfileByUserId(Convert.ToInt32(userId));

      List<ListviewTextLeftRight> profileItems;

      if (profile == null) {
        profileItems = new List<ListviewTextLeftRight>() {
          new ListviewTextLeftRight{ Id = 1, TextLeftSide = "Weight", TextRightSide = "N/a" },
          new ListviewTextLeftRight{ Id = 2, TextLeftSide = "Height", TextRightSide = "N/a" },
          new ListviewTextLeftRight{ Id = 3, TextLeftSide = "Gender", TextRightSide = "N/a" },
          new ListviewTextLeftRight{ Id = 4, TextLeftSide = "Goal Weight", TextRightSide = "N/a" },
          new ListviewTextLeftRight{ Id = 5, TextLeftSide = "Goal Date", TextRightSide = "N/a" }
        };
      } else {
        string[] weightSplit = profile.START_WEIGHT.ToString().Split(".");
        string[] heightSplit = profile.HEIGHT.ToString().Split(".");
        string[] goalWeightSplit = profile.TARGET_WEIGHT.ToString().Split(".");
        profileItems = new List<ListviewTextLeftRight>() {
          new ListviewTextLeftRight{ 
            //I set strings to always save with a "." so there shouldn't be an error here (unless record doesn't save properly)
            Id = 1, TextLeftSide = "Weight",
            TextRightSide = weightSplit[0] + " lbs " + weightSplit[1] + " oz",
            HiddenTextForConversion = profile.START_WEIGHT.ToString() },
          new ListviewTextLeftRight{
            Id = 2, TextLeftSide = "Height",
            TextRightSide = heightSplit[0] + " ft " + heightSplit[1] + " in",
            HiddenTextForConversion = profile.HEIGHT.ToString() },
          new ListviewTextLeftRight{
            Id = 3, TextLeftSide = "Gender",
            TextRightSide = profile.GENDER,
            HiddenTextForConversion = profile.GENDER.ToString() },
          new ListviewTextLeftRight{
            Id = 4, TextLeftSide = "Goal Weight",
            TextRightSide = goalWeightSplit[0] + " lbs " + goalWeightSplit[1] + " oz",
            HiddenTextForConversion = profile.TARGET_WEIGHT.ToString() },
          new ListviewTextLeftRight{
            Id = 5, TextLeftSide = "Goal Date",
            TextRightSide = profile.TARGET_DATE.ToShortDateString(),
            HiddenTextForConversion = profile.TARGET_DATE.ToShortDateString() }
        };
      }

      adapter = new ListViewTextLeftRightAdapter(this, profileItems);

      // set new items
      listView.Adapter = adapter;
    }
  }
}