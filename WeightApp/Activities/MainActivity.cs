using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using DataAccessLayer.Dao;
using DataAccessLayer.Models;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Navigation;
using Google.Android.Material.Snackbar;
using WeightApp.Activities;
using WeightApp.Fragments;
using Xamarin.Essentials;

/*
* Ernie Phillips III : 12/09/2021
* Handle the main activity and navigation drawer features
*/

namespace WeightApp {
  //[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
  //Remove Main launcher since splash screen is activity to be launched
  [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
  public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener {
    protected override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);
      Xamarin.Essentials.Platform.Init(this, savedInstanceState);
      SetContentView(Resource.Layout.activity_main);
      Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
      SetSupportActionBar(toolbar);

      FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
      fab.Click += FabOnClick;

      DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
      ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
      drawer.AddDrawerListener(toggle);
      toggle.SyncState();

      NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
      navigationView.SetNavigationItemSelectedListener(this);

      //set the user's name through nav view, then find txtView
      ISharedPreferences prefs = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
      string userName = prefs.GetString("Username", String.Empty);
      string name = prefs.GetString("Name", String.Empty);
      NavigationView nv = FindViewById<NavigationView>(Resource.Id.nav_view);
      Android.Widget.TextView txtUsername = nv.GetHeaderView(0).FindViewById<Android.Widget.TextView>(Resource.Id.txt_username);
      txtUsername.Text = String.IsNullOrEmpty(name) ? "Welcome" : "Welcome, " + name;


      //Check if the user account has an associated profile. 
      //If not send them to the welcome screen which will have a welcome message and link to the profile page ELSE send to the My Stats page
      UserDao userDao = new UserDao();
      User user = userDao.GetUserByUsername(userName); 
      ProfileDao profileDao = new ProfileDao();
      Profile profile = profileDao.GetProfileByUserId(user.USER_ID);

      if(profile != null)
        SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, new StatisticsFragment(), "Fragment").Commit();
      else
        SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, new WelcomeFragment(), "Fragment").Commit();
    }

    public override void OnBackPressed() {
      DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
      if (drawer.IsDrawerOpen(GravityCompat.Start)) {
        drawer.CloseDrawer(GravityCompat.Start);
      } else {
        base.OnBackPressed();
      }
    }

    //public override bool OnCreateOptionsMenu(IMenu menu) {
    //  MenuInflater.Inflate(Resource.Menu.menu_main, menu);
    //  return true;
    //}

    //public override bool OnOptionsItemSelected(IMenuItem item) {
    //  int id = item.ItemId;
    //  if (id == Resource.Id.action_manage_accounts) {
    //    Android.Widget.Toast.MakeText(this, "Manage accounts functionality to be wired in future version", Android.Widget.ToastLength.Long).Show();
    //    return true;
    //  }

    //  return base.OnOptionsItemSelected(item);
    //}

    private void FabOnClick(object sender, EventArgs eventArgs) {
      //send user to contact page
      SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, new ContactFragment(), "Fragment").Commit();
      //View view = (View)sender;
      //Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
      //    .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
    }

    public bool OnNavigationItemSelected(IMenuItem item) {

      int id = item.ItemId;

      if (id == Resource.Id.nav_statistics) {
        SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, new StatisticsFragment(), "Fragment").Commit();
      } else if (id == Resource.Id.nav_weight_entry) {
        SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, new WeightEntryFragment(), "Fragment").Commit();
      } else if (id == Resource.Id.nav_history) {
        SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, new HistoryFragment(), "Fragment").Commit();
      } else if (id == Resource.Id.nav_profile) {
        SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, new ProfileFragment(), "Fragment").Commit();
      } else if (id == Resource.Id.nav_share) {
        //https://docs.microsoft.com/en-us/xamarin/essentials/share?tabs=android
        Share.RequestAsync(new ShareTextRequest {
          Uri = "https://github.com/erniephillips/WeightApp/",
          Text = "Eventually the below link will link to this app in the Play Store. For now, please visit my repository.",
          Title = "Check out this weight app"
        });
        //Android.Widget.Toast.MakeText(this, "Share button functionality to be wired in future version", Android.Widget.ToastLength.Long).Show();
      } else if (id == Resource.Id.nav_contact) {
        SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, new ContactFragment(), "Fragment").Commit();
      } else if (id == Resource.Id.nav_manage_account) {
        SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, new ManageAccountFragment(), "Fragment").Commit();
      } else if (id == Resource.Id.nav_logout) {
        //clear any login stored creds
        ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
        ISharedPreferencesEditor edit = pref.Edit();
        edit.Clear();
        edit.Commit();

        //Set main activity to no history to prevent user from going back after logout
        StartActivity(typeof(UserAccessActivity));
      }

      DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
      drawer.CloseDrawer(GravityCompat.Start);
      return true;
    }
    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults) {
      Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

      base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }
  }
}

