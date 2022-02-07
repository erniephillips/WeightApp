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
using WeightApp.Adapters;
using static Android.Support.Constraints.Constraints;

/*
* Ernie Phillips III : 12/09/2021
* History page, will pull from sqlite and allow user to modify/delete entries displayed in listview
*/

namespace WeightApp.Fragments {
  public class HistoryFragment : AndroidX.Fragment.App.Fragment {

    //set the variables
    ListView listView;
    List<Weight> weightList = new List<Weight>();
    
    //instantiate the weight dao
    WeightDao weightDao = new WeightDao();

    public override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);
      // Create your fragment here
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
      // Use this to return your custom view for this Fragment
      View view = inflater.Inflate(Resource.Layout.fragment_history, container, false);

      //load the listview
      listView = view.FindViewById<ListView>(Resource.Id.history_listView);

      //dynamically create a textview to display if listview is empty
      TextView emptyText = new TextView(Context) {
        Text = "No Weight Entries Found",
        TextSize = 20,
        Visibility = ViewStates.Gone,
        Gravity = GravityFlags.CenterHorizontal | GravityFlags.CenterVertical,
        LayoutParameters = new ViewGroup.LayoutParams(LayoutParams.FillParent, LayoutParams.FillParent),
      };

      ((ViewGroup)listView.Parent).AddView(emptyText);
      listView.EmptyView = emptyText;

      LoadData(-1); //clear any position index

      return view;
    }

    //loads the listview
    private void LoadData(int position) {
      //instantiate the profile dao
      ProfileDao profileDao = new ProfileDao();

      //get stored user info
      ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
      //string userId = pref.GetString("UserId", String.Empty);
      string profileId = pref.GetString("ProfileId", String.Empty);

      //get user's profile info by account user id
      //Profile profile = profileDao.GetProfileByUserId(Convert.ToInt32(userId));
      Profile profile = profileDao.GetProfile(Convert.ToInt32(profileId));

      //check if profile is null and prevent user from interacting with page by showing modal that redirects
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
      } else { //profile not null
        //get list of weight entries by user's profile id and order descending
        weightList = weightDao.GetWeightsByProfileIdOrderByDateDesc(profile.PROFILE_ID);

        //pass to list view adapter
        HistoryListViewAdapter adapter = new HistoryListViewAdapter(this, weightList);
        adapter.SetSelectedId(position); //set the index

        // Save the ListView state (= includes scroll position) as a Parceble
        //IParcelable state = listView.OnSaveInstanceState();

        // set new items
        listView.Adapter = adapter;

        // Restore previous state (including selected item index and scroll position)
        //listView.OnRestoreInstanceState(state);
      }

    }
  }
}