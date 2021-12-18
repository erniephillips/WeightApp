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
      LoadData(-1); //clear any position index

      //set the listview item click
      listView.ItemClick += (s, e) => {
        //setting up a swith for the position selected to pull up a dialog box for user to make a selection depending
        switch (e.Position) {
          case 0: //WEIGHT OPTION
            View weightView = inflater.Inflate(Resource.Layout.dialog_weight, container, false);

            //Number picker: https://medium.com/@sc71/android-numberpickers-3ef535c45487
            NumberPicker pckWeightWholeNum = weightView.FindViewById<NumberPicker>(Resource.Id.profile_picker_weight_whole_number);

            //set the whole weight number
            string[] weightWholeNumbers = Enumerable.Range(1, 400).Select(x => x.ToString()).ToArray(); //create an array to 400 lbs
            pckWeightWholeNum.MinValue = 1;
            pckWeightWholeNum.MaxValue = weightWholeNumbers.Length;
            pckWeightWholeNum.Value = 100; //set the start value
            pckWeightWholeNum.SetDisplayedValues(weightWholeNumbers);

            new MaterialAlertDialogBuilder(Activity).SetView(weightView)
              .SetTitle("Input your current weight")
              .SetMessage("")
              .SetPositiveButton("OK", (sender, e) => { })
              .Show();
            break;
          case 1:
            new MaterialAlertDialogBuilder(Activity)
              .SetTitle("Input your height")
              .SetMessage("")
              .SetPositiveButton("OK", (sender, e) => { })
              .Show();
            break;
          case 2:
            new MaterialAlertDialogBuilder(Activity)
              .SetTitle("Select your gender")
              .SetMessage("")
              .SetPositiveButton("OK", (sender, e) => { })
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

      

      return view;
    }

    private void LoadData(int position) {
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

      ProfileListViewAdapter adapter = new ProfileListViewAdapter(this, profileItems);
      adapter.SetSelectedId(position);

      // Save the ListView state (= includes scroll position) as a Parceble
      IParcelable state = listView.OnSaveInstanceState();

      // set new items
      listView.Adapter = adapter;

      // Restore previous state (including selected item index and scroll position)
      listView.OnRestoreInstanceState(state);

    }
  }
}