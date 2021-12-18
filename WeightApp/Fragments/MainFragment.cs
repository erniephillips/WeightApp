using Android.OS;
using Android.Views;
using Android.Widget;
using DataAccessLayer.Dao;
using DataAccessLayer.Models;
using SQLite;
using System;
using System.Collections.Generic;
using WeightApp.Adapters;

/*
* Ernie Phillips III : 12/09/2021
* Main fragment to display when user first opens app
*/

namespace WeightApp.Fragments {
  public class MainFragment : AndroidX.Fragment.App.Fragment {

    //ProfileDao profileDao = new ProfileDao();
    //WeightDao weightDao = new WeightDao();
    //List<Profile> profiles = null;
    //List<Weight> weights = null;

    ListView listView;
    List<User> users = new List<User>();
    UserDao userDao = new UserDao();

    public override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
      // Use this to return your custom view for this Fragment
      View view = inflater.Inflate(Resource.Layout.fragment_main, container, false);

      listView = view.FindViewById<ListView>(Resource.Id.user_listView);
      EditText edtName = view.FindViewById<EditText>(Resource.Id.edtName);
      EditText edtPass = view.FindViewById<EditText>(Resource.Id.edtPassword);
      EditText edtUsername = view.FindViewById<EditText>(Resource.Id.edtUsername);

      LoadData(-1); //clear any position index

      #region SNIPPET FOR INSERT OF OTHER TABLES
      //User u = new User() {
      //  USERNAME = "emilytest",
      //  PASSWORD = "2342435",
      //  EMAIL = "test@test.com",
      //  NAME = "Emily Phillips",
      //  CREATED_DATE = DateTime.Now,
      //  LAST_LOGIN_DATE = DateTime.Now,
      //  FAILED_LOGIN_ATTEMPT = 0,
      //  IS_LOCKED = false
      //};
      //userDao.AddUser(u);

      //Profile profile = new Profile() {
      //  USER_ID = 20,
      //  GENDER = "Male",
      //  HEIGHT = 5.7m,
      //  START_WEIGHT = 150.0m,
      //  START_DATE = DateTime.Now,
      //  TARGET_WEIGHT = 130.0m,
      //  TARGET_DATE = DateTime.Now.AddMonths(1)
      //};
      //profileDao.AddProfile(profile);

      //Weight weight = new Weight() { 
      //  PROFILE_ID = 1,
      //  DATE_ENTRY = DateTime.Now,
      //  WEIGHT_ENTRY = 150.0m
      //};
      //weightDao.AddWeight(weight);
      //profiles = profileDao.GetProfiles();
      //weights = weightDao.GetWeights();
      #endregion

      /*
      * For loop was causing OOB, fixed by passing creating new adapter instance with position as highlighted area
      * https://www.tutorialguruji.com/android/changing-the-backgroundresource-of-a-listview-object-while-its-off-screen/
      */
      listView.ItemClick += (s, e) => {
        LoadData(e.Position);
        
        //Binding Data
        TextView txtUserID = e.View.FindViewById<TextView>(Resource.Id.txtView_UserID);
        TextView txtUserName = e.View.FindViewById<TextView>(Resource.Id.txtView_UserName);
        TextView txtPassword = e.View.FindViewById<TextView>(Resource.Id.txtView_Password);
        TextView txtName = e.View.FindViewById<TextView>(Resource.Id.txtView_Name);
        TextView txtIsLocked = e.View.FindViewById<TextView>(Resource.Id.txtView_IsLocked);
        TextView txtFailedLoginAttempts = e.View.FindViewById<TextView>(Resource.Id.txtView_FailedLoginAttemptCount);
        TextView txtCreatedDate = e.View.FindViewById<TextView>(Resource.Id.txtView_CreatedDate);
        TextView txtLastLoginDate = e.View.FindViewById<TextView>(Resource.Id.txtView_LastLoginDate);


        edtName.Tag = e.Id;
        edtName.Text = txtName.Text;
        edtPass.Text = txtPassword.Text;
        edtUsername.Text = txtUserName.Text;
      };

      return view;
    }

    private void LoadData(int position) {

      users = userDao.GetUsers();
      UserListViewAdapter adapter = new UserListViewAdapter(this, users);
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