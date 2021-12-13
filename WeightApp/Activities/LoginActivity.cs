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

namespace WeightApp.Activities {
  [Activity(Label = "LoginActivity", NoHistory = true)]
  public class LoginActivity : Activity {

    protected override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);
      SetContentView(Resource.Layout.activity_login);

      ImageButton btnBack = FindViewById<ImageButton>(Resource.Id.btn_login_back);
      Button btnLogin = FindViewById<Button>(Resource.Id.btn_log_login);

      btnBack.Click += delegate {
        StartActivity(typeof(UserAccessActivity));
      };

      //var users = userDao.GetUsers();

      btnLogin.Click += (s, e) => {
        UserDao userDao = new UserDao();
        string txtUsername = FindViewById<EditText>(Resource.Id.et_login_username).Text;
        string txtPassword = FindViewById<EditText>(Resource.Id.et_login_password).Text;
        CheckBox chkRememberMe = FindViewById<CheckBox>(Resource.Id.chk_remember_me);
        TextView txtLoginErrors = FindViewById<TextView>(Resource.Id.txt_login_errors);

        #region VALIDATION
        txtLoginErrors.Text = ""; //clear textview if any value exists
        if (txtUsername == "" || txtPassword == "") {
          if (txtUsername == "")
            txtLoginErrors.Append("Please enter a username\n");
          if(txtPassword == "")
            txtLoginErrors.Append("Please enter a password\n");
          return;
        }
        #endregion


        //verify the passed in username and password
        bool userCanLogin = userDao.VerifyLogin(txtUsername, txtPassword);
        if (userCanLogin) { //user gave correct username and password
          User user = userDao.GetUserByUsername(txtUsername); //find the user
          user.LAST_LOGIN_DATE = DateTime.Now; //set last logged in date to now
          userDao.UpdateUser(user); //update the user with last logged in date

          //set a session for the user
          if (chkRememberMe.Checked) { //set a session (sort of)
            ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            ISharedPreferencesEditor edit = pref.Edit();
            edit.PutString("UserId", user.USER_ID.ToString());
            edit.PutString("Username", user.USERNAME.Trim());
            edit.PutString("Password", user.PASSWORD.Trim());
            edit.PutString("LastLogin", user.LAST_LOGIN_DATE.ToString());
            edit.Apply();
          }
          StartActivity(typeof(MainActivity)); //send user to main applicaiton logic
        } else { //user did not authenticate properly, show alert
          AlertDialog.Builder dialog = new AlertDialog.Builder(this);
          AlertDialog alert = dialog.Create();
          alert.SetTitle("Weight App Alert");
          alert.SetMessage("Invalid Login Attemp. Please try again.");
          alert.SetButton("OK", (c, ev) => {
            // Ok button click task  
          });
          alert.Show();
        }
      };
    }
  }
}