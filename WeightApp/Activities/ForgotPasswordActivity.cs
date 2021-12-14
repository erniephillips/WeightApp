using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DataAccessLayer.Dao;
using DataAccessLayer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeightApp.Fragments;

namespace WeightApp.Activities {
  [Activity(Label = "ForgotPasswordActivity")]
  public class ForgotPasswordActivity : Activity {
    protected override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);
      SetContentView(Resource.Layout.activity_forgot_password);

      ImageButton btnBack = FindViewById<ImageButton>(Resource.Id.btn_fp_back);
      Button btnForgotPassword = FindViewById<Button>(Resource.Id.btn_fp_check_username);

      btnBack.Click += delegate {
        StartActivity(typeof(LoginActivity));
      };

      btnForgotPassword.Click += (s, e) => { 
        UserDao userDao = new UserDao();
        string txtUsername = FindViewById<EditText>(Resource.Id.et_fp_username).Text;
        TextView txtLoginErrors = FindViewById<TextView>(Resource.Id.txt_fp_errors);

        #region VALIDATION
        txtLoginErrors.Text = "";
        if (txtUsername == "") {
          txtLoginErrors.Text = "Username required";
          return;
        }

        User user = userDao.GetUserByUsername(txtUsername);
        if (user == null) {
          txtLoginErrors.Text = "Username not found";
          return;
        }
        #endregion

        //serialize user obj and send to next activity. This will save on making another db call
        Intent intent = new Intent(this, typeof(SecurityQuestionActivity));
        intent.PutExtra("User", JsonConvert.SerializeObject(user));
        StartActivity(intent);
        Finish();
      };
    }
  }
}