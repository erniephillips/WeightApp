using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using DataAccessLayer.Dao;
using DataAccessLayer.Models;
using Google.Android.Material.Dialog;
using Google.Android.Material.TextField;
using Newtonsoft.Json;
using System;
using System.Drawing;

/*
* Ernie Phillips III : 01/27/2022
* Purpose: Handle the password reset page
* Function: User is prompted the new password and confirmation of new pass, after they are redirected to login
*/

namespace WeightApp.Activities {
  [Activity(Label = "ResetPasswordActivity")]
  public class ResetPasswordActivity : Activity {
    protected override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);

      //find the xml view to set
      SetContentView(Resource.Layout.activity_reset_password);

      //XML elements to be stored in variables
      ImageButton btnBack = FindViewById<ImageButton>(Resource.Id.btn_rp_back);
      Button btnResetPassword = FindViewById<Button>(Resource.Id.btn_reset_password);

      //set the button click events
      btnBack.Click += delegate {
        //navigate to login activity
        StartActivity(typeof(LoginActivity));
      };

      btnResetPassword.Click += delegate {
        //XML elements to be stored in variables
        TextInputLayout txtIlPassword = FindViewById<TextInputLayout>(Resource.Id.et_rp_password);
        TextInputLayout txtIlConfirmPassword = FindViewById<TextInputLayout>(Resource.Id.et_rp_confirm_password);
        TextInputEditText txtPassword = FindViewById<TextInputEditText>(Resource.Id.rp_tiet_password);
        TextInputEditText txtConfirmPassword = FindViewById<TextInputEditText>(Resource.Id.rp_tiet_confirm_password);

        #region VALIDATION
        //set errors to empty on button click
        txtIlPassword.Error = "";
        txtIlConfirmPassword.Error = "";

        //check if either password or password confirm are empty or null and set error for each one that is
        if (String.IsNullOrEmpty(txtPassword.Text) || String.IsNullOrEmpty(txtConfirmPassword.Text)) {
          if (txtPassword.Text == "") {
            txtIlPassword.Error = "Password is required";
            txtIlPassword.SetErrorTextAppearance(Color.Red.ToArgb());
          }
          if (txtPassword.Text == "") {
            txtIlConfirmPassword.Error = "Password confirmation is required";
            txtIlConfirmPassword.SetErrorTextAppearance(Color.Red.ToArgb());
          }
          return;
        }

        //verify that passwords match
        if (txtPassword.Text != txtConfirmPassword.Text) {
          txtIlPassword.Error = "Passwords do not match";
          txtIlPassword.SetErrorTextAppearance(Color.Red.ToArgb());
          txtIlConfirmPassword.Error = "Passwords do not match";
          txtIlConfirmPassword.SetErrorTextAppearance(Color.Red.ToArgb());
          return;
        }
        #endregion

        //update the user's password in the database after getting current user object for update
        User user = JsonConvert.DeserializeObject<User>(Intent.GetStringExtra("User"));
        user.PASSWORD = txtPassword.Text;

        //instantiate the user dao
        UserDao userDao = new UserDao();

        try { //update
          userDao.UpdateUser(user);
        } catch (Exception ex) { 
          //throw the exception to the user in a modal
          new MaterialAlertDialogBuilder(this)
            .SetTitle("An error has occurred. Please contact the app administrator. Exception: " + ex.Message)
            .SetPositiveButton("OK", (sender, e) => { })
            .Show();
          return;
        }

        //update the user that password successfully updated
        new MaterialAlertDialogBuilder(this)
          .SetTitle("Weight App Alert")
          .SetIcon(Resource.Drawable.ic_info)
          .SetMessage("Password successfully reset.")
          .SetPositiveButton("OK", (sender, e) => {
            StartActivity(typeof(LoginActivity));
          }).Show();

        //AlertDialog.Builder dialog = new AlertDialog.Builder(this);
        //AlertDialog alert = dialog.Create();
        //alert.SetTitle("Weight App Alert");
        //alert.SetMessage("Password successfully reset");
        //alert.SetButton("OK", (c, ev) => {
        //  StartActivity(typeof(LoginActivity));
        //});
        //alert.Show();
      };

    }
  }
}