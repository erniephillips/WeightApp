using Android.OS;
using Android.Views;
using Android.Widget;
using DataAccessLayer.Dao;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using WeightApp.Helpers;

/*
* Ernie Phillips III : 12/09/2021
* Main fragment to display when user first opens app
*/

namespace WeightApp.Fragments {
  public class MainFragment : AndroidX.Fragment.App.Fragment {

    List<User> users = null;
    ListView listView;

    public override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);

      // Create your fragment here
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
      // Use this to return your custom view for this Fragment
      View view = inflater.Inflate(Resource.Layout.fragment_main, container, false); 
      UserDao userDao = new UserDao();

      //User u = new User() {
      //  USERNAME = "katietest",
      //  PASSWORD = "test123",
      //  EMAIL = "test@test.com",
      //  NAME = "Katie Phillips",
      //  CREATED_DATE = DateTime.Now,
      //  LAST_LOGIN_DATE = DateTime.Now,
      //  FAILED_LOGIN_ATTEMPT = 0,
      //  IS_LOCKED = false
      //};
      //userDao.AddUser(u);

      listView = view.FindViewById<ListView>(Resource.Id.person_listView);

      users = userDao.GetUsers();

      LoadData(-1);

      return view;
    }

    private void LoadData(int position) {
      PersonListViewAdapter adapter = new PersonListViewAdapter(this, users);
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