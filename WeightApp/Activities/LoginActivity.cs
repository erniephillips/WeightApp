using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using DataAccessLayer.Dao;
using DataAccessLayer.Models;
using Google.Android.Material.Dialog;
using Google.Android.Material.TextField;
using System;
using System.Drawing;

/*
* Ernie Phillips III : 01/27/2022
* Purpose: Handle the login page
* Function: User is prompted for a username and password, if successful, user moves to statistics or welcome page depending on existing profile
*/

namespace WeightApp.Activities {
  [Activity(Label = "LoginActivity", NoHistory = true)]
  public class LoginActivity : Activity {

    protected override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);
      SetContentView(Resource.Layout.activity_login);

      ImageButton btnBack = FindViewById<ImageButton>(Resource.Id.btn_login_back);
      Button btnLogin = FindViewById<Button>(Resource.Id.btn_log_login);
      TextView txtForgotPassword = FindViewById<TextView>(Resource.Id.txt_forgot_password);

      btnBack.Click += delegate {
        StartActivity(typeof(UserAccessActivity));
      };

      txtForgotPassword.Click += (s, e) => {
        //send to forgot password screen
        StartActivity(typeof(ForgotPasswordActivity));
      };

      btnLogin.Click += (s, e) => {
        UserDao userDao = new UserDao();

        TextInputLayout txtIlUsername = FindViewById<TextInputLayout>(Resource.Id.et_login_username);
        TextInputLayout txtIlPassword = FindViewById<TextInputLayout>(Resource.Id.et_login_password);
        TextInputEditText txtUsername = FindViewById<TextInputEditText>(Resource.Id.login_tiet_username);
        TextInputEditText txtPassword = FindViewById<TextInputEditText>(Resource.Id.login_tiet_password);
        CheckBox chkRememberMe = FindViewById<CheckBox>(Resource.Id.chk_remember_me);

        #region VALIDATION
        txtIlUsername.Error = "";
        txtIlPassword.Error = "";

        if (txtUsername.Text == "" || txtPassword.Text == "") {
          if (txtUsername.Text == "") {
            txtIlUsername.Error = "Username is required";
            txtIlUsername.SetErrorTextAppearance(Color.Red.ToArgb());
          }
          if (txtPassword.Text == "") {
            txtIlPassword.Error = "Password is required";
            txtIlPassword.SetErrorTextAppearance(Color.Red.ToArgb());
          }
          return;
        }
        #endregion

        //store user input
        ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
        ISharedPreferencesEditor edit = pref.Edit();

        //verify the passed in username and password
        bool userCanLogin = userDao.VerifyLogin(txtUsername.Text, txtPassword.Text);
        if (userCanLogin) { //user gave correct username and password
          User user = userDao.GetUserByUsername(txtUsername.Text); //find the user
          user.LAST_LOGIN_DATE = DateTime.Now; //set last logged in date to now

          try {
            userDao.UpdateUser(user); //update the user with last logged in date
          } catch (Exception ex) {
            new MaterialAlertDialogBuilder(this)
            .SetTitle("An error has occurred. Please contact the app administrator. Exception: " + ex.Message)
            .SetPositiveButton("OK", (sender, e) => { })
            .Show();
          }
          //set a session for the user
          if (chkRememberMe.Checked) { //set a session (sort of)
            edit.PutString("UserId", user.USER_ID.ToString());
            edit.PutString("Username", user.USERNAME.Trim());
            edit.PutString("Name", user.NAME.Trim());
            edit.PutString("Password", user.PASSWORD.Trim());
            edit.PutString("LastLogin", user.LAST_LOGIN_DATE.ToString());
            edit.Apply();
          } else { //save the username and PK for app reference
            edit.PutString("UserId", user.USER_ID.ToString());
            edit.PutString("Username", user.USERNAME.Trim());
            edit.PutString("Name", user.NAME.Trim());
            edit.Apply();
          }
          StartActivity(typeof(MainActivity)); //send user to main applicaiton logic
        } else { //user did not authenticate properly, show alert
          new MaterialAlertDialogBuilder(this)
          .SetTitle("Weight App Alert")
          .SetIcon(Resource.Drawable.ic_info)
          .SetMessage("Invalid Login Attempt. Please try again.")
          .SetPositiveButton("OK", (sender, e) => { })
          .Show();
        }
      };
    }
  }
}