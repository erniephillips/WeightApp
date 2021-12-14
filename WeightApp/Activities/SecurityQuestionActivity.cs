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
using WeightApp.Activities;

namespace WeightApp.Fragments {
  [Activity(Label = "SecurityQuestionActivity")]
  public class SecurityQuestionActivity : Activity {
    protected override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);

      ImageButton btnBack = FindViewById<ImageButton>(Resource.Id.btn_sq_back);
      Button btnVerifySA = FindViewById<Button>(Resource.Id.btn_verify_sa);

      UserDao userDao = new UserDao();

      ISharedPreferences prefs = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
      string userName = prefs.GetString("Username", String.Empty);

      userDao.GetUserByUsername(userName);

      btnBack.Click += delegate {
        StartActivity(typeof(ForgotPasswordActivity));
      };

      btnVerifySA.Click += (s, e) => {
        
        //txtSecurityQuestion.Text = user.SECURITY_QUESTION;

      };
    }
  }
}