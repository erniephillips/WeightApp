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
  [Activity(Label = "RegisterActivity")]
  public class RegisterActivity : Activity {
    protected override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);
      SetContentView(Resource.Layout.activity_register);

      ImageButton btnBack = FindViewById<ImageButton>(Resource.Id.btn_register_back);
      btnBack.Click += delegate {
        Finish();
      };
    }
  }
}