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
using System.Text.RegularExpressions;
using WeightApp.Adapters;
using WeightApp.Utilities;

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
          string error = "", measurementType = "";
          bool hasError = false;
          List<ListviewTextLeftRight> ListviewTextLeftRights = adapter.GetItems();
          foreach (ListviewTextLeftRight profileItem in ListviewTextLeftRights) {
            if (profileItem.TextRightSide == "N/a" || profileItem.TextRightSide == "" || profileItem.TextRightSide == " lbs" || profileItem.TextRightSide == " kg" || profileItem.TextRightSide == " cm") {
              error += profileItem.TextLeftSide + " is required.\n";
              hasError = true;
            }
          }
          if (!hasError) {
            foreach (ListviewTextLeftRight profileItem in ListviewTextLeftRights) {
              if (profileItem.TextLeftSide.ToLower() == "system")
                measurementType = profileItem.TextRightSide;
            }
            if (measurementType != "") {
              foreach (ListviewTextLeftRight profileItem in ListviewTextLeftRights) {
                if (measurementType.ToLower() == "metric") {
                  if (profileItem.TextLeftSide.ToLower() == "weight") {
                    if (Convert.ToInt32(profileItem.HiddenTextForConversion) < 22 || Convert.ToInt32(profileItem.HiddenTextForConversion) > 227) {
                      error += "Weight must be between 22 and 227 kg.\n";
                    }
                  }
                  if (profileItem.TextLeftSide.ToLower() == "goal weight") {
                    if (Convert.ToInt32(profileItem.HiddenTextForConversion) < 22 || Convert.ToInt32(profileItem.HiddenTextForConversion) > 227) {
                      error += "Goal Weight must be between 22 and 227 kg.\n";
                    }
                  }
                  if (profileItem.TextLeftSide.ToLower() == "height") {
                    if (Convert.ToInt32(profileItem.HiddenTextForConversion) < 30 || Convert.ToInt32(profileItem.HiddenTextForConversion) > 243) {
                      error += "Height must be between 30 and 243 cm.\n";
                    }
                  }
                } else {
                  if (profileItem.TextLeftSide.ToLower() == "weight") {
                    if (Convert.ToInt32(profileItem.HiddenTextForConversion) < 50 || Convert.ToInt32(profileItem.HiddenTextForConversion) > 500) {
                      error += "Weight must be between 50 and 500 lbs.\n";
                    }
                  }
                  if (profileItem.TextLeftSide.ToLower() == "goal weight") {
                    if (Convert.ToInt32(profileItem.HiddenTextForConversion) < 50 || Convert.ToInt32(profileItem.HiddenTextForConversion) > 500) {
                      error += "Goal Weight must be between 50 and 500 lbs.\n";
                    }
                  }
                }
              }
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
          //string userId = pref.GetString("UserId", String.Empty);
          string profileId = pref.GetString("ProfileId", String.Empty);

          //initialize a new profile object
          Profile profile = new Profile() {
            //USER_ID = Convert.ToInt32(userId),
            START_DATE = DateTime.Now
          };

          //get each list item rightside textview value and set fields to profile object
          foreach (ListviewTextLeftRight profileItem in ListviewTextLeftRights) {
            if (profileItem.TextLeftSide == "Name")
              profile.NAME = profileItem.HiddenTextForConversion;
            if (profileItem.TextLeftSide == "System")
              profile.MEASUREMENT_SYSTEM = profileItem.HiddenTextForConversion;
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
          //Profile tempProfile = profileDao.GetProfileByUserId(Convert.ToInt32(userId));
          Profile tempProfile = profileDao.GetProfile(Convert.ToInt32(profileId));
          if (tempProfile == null)
            try { //add
              profileDao.AddProfile(profile);

              //retreive key/value pairs in userinfo from shared prefs
              ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
              ISharedPreferencesEditor edit = pref.Edit(); //establish edit mode
              edit.PutString("ProfileId", profile.PROFILE_ID.ToString());
              edit.PutString("ProfileName", profile.NAME.ToString());
              edit.Apply();
            } catch (Exception ex) {
              new MaterialAlertDialogBuilder(Activity)
              .SetTitle("An error has occurred. Please contact the app administrator. Exception: " + ex.Message)
              .SetPositiveButton("OK", (sender, e) => { })
              .Show();
            }
          else { //update mode
            //modify all existing weight entries if any
            //verify that user has changed measurement system
            if (tempProfile.MEASUREMENT_SYSTEM != profile.MEASUREMENT_SYSTEM) {
              Calculations calculations = new Calculations();
              WeightDao weightDao = new WeightDao();
              List<Weight> entries = weightDao.GetWeightsByProfileIdOrderByDateAsc(tempProfile.PROFILE_ID);
              if (profile.MEASUREMENT_SYSTEM == "Metric") {
                foreach (Weight weight in entries) {
                  ListviewTextLeftRight item = calculations.ConvertPoundsToKg(Convert.ToDouble(weight.WEIGHT_ENTRY));
                  weight.WEIGHT_ENTRY = item.HiddenTextForConversion.ToString();
                }
              } else { //imperial
                foreach (Weight weight in entries) {
                  ListviewTextLeftRight item = calculations.ConvertKgToPounds(Convert.ToDouble(weight.WEIGHT_ENTRY));
                  weight.WEIGHT_ENTRY = item.HiddenTextForConversion.ToString();
                }
              }

              if (entries.Count != 0)  //update list of weights
                weightDao.BulkUpdateWeights(entries);
            }

            tempProfile.NAME = profile.NAME;
            tempProfile.MEASUREMENT_SYSTEM = profile.MEASUREMENT_SYSTEM;
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
          #region NAME
          case 0:
            View profileNameView = inflater.Inflate(Resource.Layout.dialog_textbox, container, false);
            EditText edtProfileName = profileNameView.FindViewById<EditText>(Resource.Id.dialog_tiet_textbox);
            edtProfileName.Hint = "Name";

            //populate the textbox if profile gender exists
            if (profile != null) {
              //if profile name not null output to textbox  
              edtProfileName.Text = profile.NAME;
            }

            new MaterialAlertDialogBuilder(Activity).SetView(profileNameView)
               .SetTitle("What's your name?")
               .SetCancelable(false)
               .SetNegativeButton("Cancel", (s, e) => { })
               .SetPositiveButton("OK", (sender, e) => {
                 adapter.SetSelectedTextValue(eLV.Position, edtProfileName.Text, edtProfileName.Text);
               })
               .Show();

            break;
          #endregion
          #region MEASUREMENT SYSTEM OPTION
          case 1:
            View measurementSystemView = inflater.Inflate(Resource.Layout.dialog_measurement_system, container, false);

            RadioGroup rdgMeasurementSystem = measurementSystemView.FindViewById<RadioGroup>(Resource.Id.radio_measurement_system_group);

            //populate the dropdown if profile gender exists
            if (profile != null) {
              int count = rdgMeasurementSystem.ChildCount;
              for (int i = 0; i < count; i++) {
                View o = rdgMeasurementSystem.GetChildAt(i);
                if (o is RadioButton) {
                  RadioButton rdBtn = (RadioButton)o;
                  if (rdBtn.Text == profile.MEASUREMENT_SYSTEM)
                    rdBtn.Checked = true;
                }
              }
            }

            new MaterialAlertDialogBuilder(Activity).SetView(measurementSystemView)
              .SetTitle("Select your preferred measurement system")
              .SetPositiveButton("OK", (sender, e) => {
                RadioButton radioMeasurementSystemButton = measurementSystemView.FindViewById<RadioButton>(rdgMeasurementSystem.CheckedRadioButtonId);
                adapter.SetSelectedTextValue(eLV.Position, radioMeasurementSystemButton.Text, radioMeasurementSystemButton.Text);

                //modify the measurements if needed
                Calculations calculations = new Calculations();
                List<ListviewTextLeftRight> ListviewTextLeftRightsWeight = adapter.GetItems();
                foreach (ListviewTextLeftRight profileItem in ListviewTextLeftRightsWeight) {
                  //check if user selected metric
                  if (radioMeasurementSystemButton.Text == "Metric") {
                    if (profileItem.TextLeftSide == "Weight") {
                      if (profileItem.TextRightSide.Contains("lbs")) {
                        if (profileItem.TextRightSide != " lbs") {
                          ListviewTextLeftRight newProfileItem = calculations.ConvertPoundsToKg(Convert.ToDouble(profileItem.HiddenTextForConversion));
                          profileItem.TextRightSide = newProfileItem.TextRightSide;
                          profileItem.HiddenTextForConversion = newProfileItem.HiddenTextForConversion;
                        } else {
                          profileItem.TextRightSide = " kg";
                        }
                      }
                    }
                    if (profileItem.TextLeftSide == "Height") {
                      if (profileItem.TextRightSide.Contains("ft")) {
                        ListviewTextLeftRight newProfileItem = calculations.ConvertHeightToCm(profileItem.HiddenTextForConversion);
                        profileItem.TextRightSide = newProfileItem.TextRightSide;
                        profileItem.HiddenTextForConversion = newProfileItem.HiddenTextForConversion;
                      }
                    }
                    if (profileItem.TextLeftSide == "Goal Weight") {
                      if (profileItem.TextRightSide.Contains("lbs")) {
                        if (profileItem.TextRightSide != " lbs") {
                          ListviewTextLeftRight newProfileItem = calculations.ConvertPoundsToKg(Convert.ToDouble(profileItem.HiddenTextForConversion));
                          profileItem.TextRightSide = newProfileItem.TextRightSide;
                          profileItem.HiddenTextForConversion = newProfileItem.HiddenTextForConversion;
                        } else {
                          profileItem.TextRightSide = " kg";
                        }
                      }
                    }
                  } else { //user selected imperial
                    if (profileItem.TextLeftSide == "Weight") {
                      if (profileItem.TextRightSide.Contains("kg")) {
                        if (profileItem.TextRightSide != " kg") {
                          ListviewTextLeftRight newProfileItem = calculations.ConvertKgToPounds(Convert.ToDouble(profileItem.HiddenTextForConversion));
                          profileItem.TextRightSide = newProfileItem.TextRightSide;
                          profileItem.HiddenTextForConversion = newProfileItem.HiddenTextForConversion;
                        } else {
                          profileItem.TextRightSide = " lbs";
                        }
                      }
                    }
                    if (profileItem.TextLeftSide == "Height") {
                      if (profileItem.TextRightSide.Contains("cm")) {
                        ListviewTextLeftRight newProfileItem = calculations.ConvertCmToFtIn(Convert.ToDouble(profileItem.HiddenTextForConversion));
                        profileItem.TextRightSide = newProfileItem.TextRightSide;
                        profileItem.HiddenTextForConversion = newProfileItem.HiddenTextForConversion;
                      }
                    }
                    if (profileItem.TextLeftSide == "Goal Weight") {
                      if (profileItem.TextRightSide.Contains("kg")) {
                        if (profileItem.TextRightSide != " kg") {
                          ListviewTextLeftRight newProfileItem = calculations.ConvertKgToPounds(Convert.ToDouble(profileItem.HiddenTextForConversion));
                          profileItem.TextRightSide = newProfileItem.TextRightSide;
                          profileItem.HiddenTextForConversion = newProfileItem.HiddenTextForConversion;
                        } else {
                          profileItem.TextRightSide = " lbs";
                        }
                      }
                    }
                  }
                }
              }).Show();
            break;
          #endregion
          #region WEIGHT OPTION
          case 2:
            //verify user has selected a measurement system type first
            List<ListviewTextLeftRight> ListviewTextLeftRightsWeight = adapter.GetItems();
            foreach (ListviewTextLeftRight profileItem in ListviewTextLeftRightsWeight) {
              if (profileItem.TextLeftSide == "System") {
                if (profileItem.TextRightSide == "Metric") {
                  #region METRIC
                  View weightView = inflater.Inflate(Resource.Layout.dialog_textbox, container, false);
                  EditText editTextWeight = weightView.FindViewById<EditText>(Resource.Id.dialog_tiet_textbox);
                  editTextWeight.InputType = Android.Text.InputTypes.ClassNumber;
                  editTextWeight.Hint = "kg";

                  new MaterialAlertDialogBuilder(Activity).SetView(weightView)
                   .SetTitle("What's your current weight?")
                   .SetNegativeButton("Cancel", (s, e) => { })
                   .SetPositiveButton("OK", (sender, e) => {
                     adapter.SetSelectedTextValue(eLV.Position, editTextWeight.Text + " kg", editTextWeight.Text);
                   })
                   .Show();
                  #endregion
                } else {
                  #region IMPERIAL
                  View weightView = inflater.Inflate(Resource.Layout.dialog_textbox, container, false);
                  EditText editTextWeight = weightView.FindViewById<EditText>(Resource.Id.dialog_tiet_textbox);
                  editTextWeight.InputType = Android.Text.InputTypes.ClassNumber;
                  editTextWeight.Hint = "lbs";

                  new MaterialAlertDialogBuilder(Activity).SetView(weightView)
                   .SetTitle("What's your current weight?")
                   .SetNegativeButton("Cancel", (s, e) => { })
                   .SetPositiveButton("OK", (sender, e) => {
                     adapter.SetSelectedTextValue(eLV.Position, editTextWeight.Text + " lbs", editTextWeight.Text);
                   })
                   .Show();
                  #endregion
                }
              }
            }

            break;
          #endregion
          #region HEIGHT OPTION
          case 3://HEIGHT OPTION
            //verify user has selected a measurement system type first
            List<ListviewTextLeftRight> ListviewTextLeftRightsHeight = adapter.GetItems();
            foreach (ListviewTextLeftRight profileItem in ListviewTextLeftRightsHeight) {
              if (profileItem.TextLeftSide == "System") {

                if (profileItem.TextRightSide == "Metric") {
                  #region METRIC
                  View heightView = inflater.Inflate(Resource.Layout.dialog_textbox, container, false);
                  EditText editTextWeight = heightView.FindViewById<EditText>(Resource.Id.dialog_tiet_textbox);
                  editTextWeight.InputType = Android.Text.InputTypes.ClassNumber;
                  editTextWeight.Hint = "cm";

                  new MaterialAlertDialogBuilder(Activity).SetView(heightView)
                   .SetTitle("What's your height?")
                   .SetNegativeButton("Cancel", (s, e) => { })
                   .SetPositiveButton("OK", (sender, e) => {
                     adapter.SetSelectedTextValue(eLV.Position, editTextWeight.Text + " cm", editTextWeight.Text);
                   })
                   .Show();
                  #endregion
                } else {
                  #region IMPERIAL
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
                  //pckHeightFtNum.Value = profile != null ? Convert.ToInt32(profile.HEIGHT.Split(".")?[0]) : 5; //set the start value
                  pckHeightFtNum.Value = 5; //set the start value
                  pckHeightFtNum.SetDisplayedValues(heightFeetNumbers);

                  //set the whole weight number
                  string[] weightInchNumbers = Enumerable.Range(0, 13).Select(x => x.ToString()).ToArray(); //create an array to 400 lbs
                  pckHeightInNum.MinValue = 1;
                  pckHeightInNum.MaxValue = weightInchNumbers.Length - 1;
                  //pckHeightInNum.Value = profile != null ? Convert.ToInt32(profile.HEIGHT.Split(".")?[1]) + 1 : 1; //set the start value
                  pckHeightInNum.Value = 1; //set the start value
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
                  #endregion
                }
              }
            }

            break;
          #endregion
          #region GENDER OPTION
          case 4:
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
          case 5:
            //verify user has selected a measurement system type first
            List<ListviewTextLeftRight> GoalListviewTextLeftRightsWeight = adapter.GetItems();
            foreach (ListviewTextLeftRight profileItem in GoalListviewTextLeftRightsWeight) {
              if (profileItem.TextLeftSide == "System") {
                if (profileItem.TextRightSide == "Metric") {
                  #region METRIC
                  View weightView = inflater.Inflate(Resource.Layout.dialog_textbox, container, false);
                  EditText editTextWeight = weightView.FindViewById<EditText>(Resource.Id.dialog_tiet_textbox);
                  editTextWeight.InputType = Android.Text.InputTypes.ClassNumber;
                  editTextWeight.Hint = "kg";

                  new MaterialAlertDialogBuilder(Activity).SetView(weightView)
                   .SetTitle("What's your goal weight?")
                   .SetNegativeButton("Cancel", (s, e) => { })
                   .SetPositiveButton("OK", (sender, e) => {
                     adapter.SetSelectedTextValue(eLV.Position, editTextWeight.Text + " kg", editTextWeight.Text);
                   })
                   .Show();
                  #endregion
                } else {
                  #region IMPERIAL
                  View weightView = inflater.Inflate(Resource.Layout.dialog_textbox, container, false);
                  EditText editTextWeight = weightView.FindViewById<EditText>(Resource.Id.dialog_tiet_textbox);
                  editTextWeight.InputType = Android.Text.InputTypes.ClassNumber;
                  editTextWeight.Hint = "lbs";

                  new MaterialAlertDialogBuilder(Activity).SetView(weightView)
                   .SetTitle("What's your goal weight?")
                   .SetNegativeButton("Cancel", (s, e) => { })
                   .SetPositiveButton("OK", (sender, e) => {
                     adapter.SetSelectedTextValue(eLV.Position, editTextWeight.Text + " lbs", editTextWeight.Text);
                   })
                   .Show();
                  #endregion
                }
              }
            }

            break;
          #endregion
          #region GOAL DATE OPTION
          case 6:
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
      //string userId = pref.GetString("UserId", String.Empty);
      //profile = profileDao.GetProfileByUserId(Convert.ToInt32(userId));
      string profileId = pref.GetString("ProfileId", String.Empty);
      profile = profileDao.GetProfile(Convert.ToInt32(profileId));

      List<ListviewTextLeftRight> profileItems;

      if (profile == null) {
        profileItems = new List<ListviewTextLeftRight>() {
          new ListviewTextLeftRight{ Id = 1, TextLeftSide = "Name", TextRightSide = "N/a" },
          new ListviewTextLeftRight{ Id = 2, TextLeftSide = "System", TextRightSide = "N/a" },
          new ListviewTextLeftRight{ Id = 3, TextLeftSide = "Weight", TextRightSide = "N/a" },
          new ListviewTextLeftRight{ Id = 4, TextLeftSide = "Height", TextRightSide = "N/a" },
          new ListviewTextLeftRight{ Id = 5, TextLeftSide = "Gender", TextRightSide = "N/a" },
          new ListviewTextLeftRight{ Id = 6, TextLeftSide = "Goal Weight", TextRightSide = "N/a" },
          new ListviewTextLeftRight{ Id = 7, TextLeftSide = "Goal Date", TextRightSide = "N/a" }
        };
      } else {
        if (profile.MEASUREMENT_SYSTEM == "Metric") {
          profileItems = new List<ListviewTextLeftRight>() {
            new ListviewTextLeftRight{
              Id = 1, TextLeftSide = "Name",
              TextRightSide = profile.NAME,
              HiddenTextForConversion = profile.NAME.ToString() },
            new ListviewTextLeftRight{
              Id = 2, TextLeftSide = "System",
              TextRightSide = profile.MEASUREMENT_SYSTEM,
              HiddenTextForConversion = profile.MEASUREMENT_SYSTEM.ToString() },
            new ListviewTextLeftRight{ 
              //I set strings to always save with a "." so there shouldn't be an error here (unless record doesn't save properly)
              Id = 3, TextLeftSide = "Weight",
              TextRightSide = profile.START_WEIGHT + " kg",
              HiddenTextForConversion = profile.START_WEIGHT.ToString() },
            new ListviewTextLeftRight{
              Id = 4, TextLeftSide = "Height",
              TextRightSide = profile.HEIGHT + " cm",
              HiddenTextForConversion = profile.HEIGHT.ToString() },
            new ListviewTextLeftRight{
              Id = 5, TextLeftSide = "Gender",
              TextRightSide = profile.GENDER,
              HiddenTextForConversion = profile.GENDER.ToString() },
            new ListviewTextLeftRight{
              Id = 6, TextLeftSide = "Goal Weight",
              TextRightSide = profile.TARGET_WEIGHT + " kg",
              HiddenTextForConversion = profile.TARGET_WEIGHT.ToString() },
            new ListviewTextLeftRight{
              Id = 7, TextLeftSide = "Goal Date",
              TextRightSide = profile.TARGET_DATE.ToShortDateString(),
              HiddenTextForConversion = profile.TARGET_DATE.ToShortDateString() }
          };
        } else {
          //string[] weightSplit = profile.START_WEIGHT.ToString().Split(".");
          string[] heightSplit = profile.HEIGHT.ToString().Split(".");
          //string[] goalWeightSplit = profile.TARGET_WEIGHT.ToString().Split(".");
          profileItems = new List<ListviewTextLeftRight>() {
            new ListviewTextLeftRight{
              Id = 1, TextLeftSide = "Name",
              TextRightSide = profile.NAME,
              HiddenTextForConversion = profile.NAME.ToString() },
            new ListviewTextLeftRight{
              Id = 2, TextLeftSide = "System",
              TextRightSide = profile.MEASUREMENT_SYSTEM,
              HiddenTextForConversion = profile.MEASUREMENT_SYSTEM.ToString() },
            new ListviewTextLeftRight{ 
              //I set strings to always save with a "." so there shouldn't be an error here (unless record doesn't save properly)
              Id = 3, TextLeftSide = "Weight",
              //TextRightSide = weightSplit[0] + " lbs " + weightSplit[1] + " oz",
              TextRightSide = profile.START_WEIGHT + " lbs",
              HiddenTextForConversion = profile.START_WEIGHT.ToString() },
            new ListviewTextLeftRight{
              Id = 4, TextLeftSide = "Height",
              TextRightSide = heightSplit[0] + " ft " + heightSplit[1] + " in",
              HiddenTextForConversion = profile.HEIGHT.ToString() },
            new ListviewTextLeftRight{
              Id = 5, TextLeftSide = "Gender",
              TextRightSide = profile.GENDER,
              HiddenTextForConversion = profile.GENDER.ToString() },
            new ListviewTextLeftRight{
              Id = 6, TextLeftSide = "Goal Weight",
              //TextRightSide = goalWeightSplit[0] + " lbs " + goalWeightSplit[1] + " oz",
              TextRightSide = profile.TARGET_WEIGHT + " lbs",
              HiddenTextForConversion = profile.TARGET_WEIGHT.ToString() },
            new ListviewTextLeftRight{
              Id = 7, TextLeftSide = "Goal Date",
              TextRightSide = profile.TARGET_DATE.ToShortDateString(),
              HiddenTextForConversion = profile.TARGET_DATE.ToShortDateString() }
          };
        }
      }

      adapter = new ListViewTextLeftRightAdapter(this, profileItems);

      // set new items
      listView.Adapter = adapter;
    }
  }
}