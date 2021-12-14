using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DataAccessLayer.Dao;
using DataAccessLayer.Models;
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

        if(txtUsername == "") {
          txtLoginErrors.Text = "Username required";
          return;
        }

        User user = userDao.GetUserByUsername(txtUsername);
        if (user == null) {
          txtLoginErrors.Text = "Username not found";
          return;
        }

        //set username in storage and navigate to security question activity
        ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
        ISharedPreferencesEditor edit = pref.Edit();
        edit.PutString("Username", user.USERNAME.Trim());
        StartActivity(typeof(SecurityQuestionActivity));

      };
    }
  }
}