using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeightApp.Activities {
  [Activity(Label = "LoginActivity")]
  public class LoginActivity : Activity {
    protected override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);
      SetContentView(Resource.Layout.activity_login);

      ImageButton btnBack = FindViewById<ImageButton>(Resource.Id.btn_login_back);
      btnBack.Click += delegate {
        Finish();
      };
    }
  }
}