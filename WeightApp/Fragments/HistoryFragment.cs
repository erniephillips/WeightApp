using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Percent;
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

    ListView listView;
    List<Weight> weightList = new List<Weight>();
    WeightDao weightDao = new WeightDao();

    public override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);

      // Create your fragment here
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
      // Use this to return your custom view for this Fragment
      View view = inflater.Inflate(Resource.Layout.fragment_history, container, false);

      listView = view.FindViewById<ListView>(Resource.Id.history_listView);

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

    private void LoadData(int position) {

      ProfileDao profileDao = new ProfileDao();
      ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
      string userId = pref.GetString("UserId", String.Empty);
      Profile profile = profileDao.GetProfileByUserId(Convert.ToInt32(userId));

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
        weightList = weightDao.GetWeightsByProfileIdOrderByDateDesc(profile.PROFILE_ID);

        HistoryListViewAdapter adapter = new HistoryListViewAdapter(this, weightList);
        adapter.SetSelectedId(position);

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