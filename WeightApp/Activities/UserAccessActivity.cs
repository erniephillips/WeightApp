using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DataAccessLayer.Dao;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeightApp.Activities {
  [Activity(Label = "UserAccessActivity", NoHistory = true)]
  public class UserAccessActivity : Activity {

    protected override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);
      SetContentView(Resource.Layout.activity_user_access);

      Button btnLogin = FindViewById<Button>(Resource.Id.aua_login);
      Button btnRegister = FindViewById<Button>(Resource.Id.aua_register);

      btnLogin.Click += delegate {
        StartActivity(typeof(LoginActivity));
      };

      btnRegister.Click += delegate {
        StartActivity(typeof(RegisterActivity));
      };
    }
  }
}