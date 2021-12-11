﻿using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Navigation;
using Google.Android.Material.Snackbar;
using WeightApp.Fragments;

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

      //Load the main fragment after creating nav drawer and nav view
      SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, new MainFragment(), "Fragment").Commit();
    }

    public override void OnBackPressed() {
      DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
      if (drawer.IsDrawerOpen(GravityCompat.Start)) {
        drawer.CloseDrawer(GravityCompat.Start);
      } else {
        base.OnBackPressed();
      }
    }

    public override bool OnCreateOptionsMenu(IMenu menu) {
      MenuInflater.Inflate(Resource.Menu.menu_main, menu);
      return true;
    }

    public override bool OnOptionsItemSelected(IMenuItem item) {
      int id = item.ItemId;
      if (id == Resource.Id.action_manage_accounts) {
        Android.Widget.Toast.MakeText(this, "Manage accounts functionality to be wired in future version", Android.Widget.ToastLength.Long).Show();
        return true;
      }

      return base.OnOptionsItemSelected(item);
    }

    private void FabOnClick(object sender, EventArgs eventArgs) {
      View view = (View)sender;
      Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
          .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
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
        Android.Widget.Toast.MakeText(this, "Share button functionality to be wired in future version", Android.Widget.ToastLength.Long).Show();
      } else if (id == Resource.Id.nav_contact) {
        SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, new ContactFragment(), "Fragment").Commit();
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

