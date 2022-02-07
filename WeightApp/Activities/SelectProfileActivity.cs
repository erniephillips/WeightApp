using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using DataAccessLayer.Dao;
using DataAccessLayer.Models;
using System.Collections.Generic;
using WeightApp.Adapters;

/*
* Ernie Phillips III : 02/06/2022
* Purpose: Handle the selection of user profile 
* Function: Main page for app access, displays all existing profiles and allows for adding a new profile
*/

namespace WeightApp.Activities {
  [Activity(Label = "SelectProfileActivity")]
  public class SelectProfileActivity : Activity {

    ////set the variables
    ListView listView;
    List<Profile> profiles = new List<Profile>();
    ProfileListViewAdapter adapter;

    protected override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);

      //find the xml view to set
      SetContentView(Resource.Layout.activity_select_profile);

      //set the user listview
      listView = FindViewById<ListView>(Resource.Id.select_profile_list_view);

      //load data into listview
      LoadData(-1);

      //handle item click event
      listView.ItemClick += (s, e) => {
        
        //retreive key/value pairs in userinfo from shared prefs
        ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
        ISharedPreferencesEditor edit = pref.Edit(); //establish edit mode

        Profile selectedProfile = adapter.GetProfileInfo(e.Position);
        if (selectedProfile.NAME == "+ Add New Profile") { //add new profile
          edit.PutString("ProfileId", "-1");
          edit.PutString("ProfileName", "");
        } else { //user profile selected
          edit.PutString("ProfileId", selectedProfile.PROFILE_ID.ToString());
          edit.PutString("ProfileName", selectedProfile.NAME.ToString());
        }
        edit.Apply();
        StartActivity(typeof(MainActivity)); //send user to main applicaiton logic
      };
    }

    private void LoadData(int position) {

      //instantiate the user dao
      ProfileDao profileDao = new ProfileDao();

      profiles = profileDao.GetProfiles();
      profiles.Add(new Profile() { NAME = "+ Add New Profile" });
      adapter = new ProfileListViewAdapter(this, profiles);
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