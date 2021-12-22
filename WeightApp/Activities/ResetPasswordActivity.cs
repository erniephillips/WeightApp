using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DataAccessLayer.Dao;
using DataAccessLayer.Models;
using Google.Android.Material.Dialog;
using Google.Android.Material.TextField;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace WeightApp.Activities {
  [Activity(Label = "ResetPasswordActivity")]
  public class ResetPasswordActivity : Activity {
    protected override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);
      SetContentView(Resource.Layout.activity_reset_password);

      ImageButton btnBack = FindViewById<ImageButton>(Resource.Id.btn_rp_back);
      Button btnResetPassword = FindViewById<Button>(Resource.Id.btn_reset_password);

      btnBack.Click += delegate {
        StartActivity(typeof(LoginActivity));
      };

      btnResetPassword.Click += delegate {
        TextInputLayout txtIlPassword = FindViewById<TextInputLayout>(Resource.Id.et_rp_password);
        TextInputLayout txtIlConfirmPassword = FindViewById<TextInputLayout>(Resource.Id.et_rp_confirm_password);
        TextInputEditText txtPassword = FindViewById<TextInputEditText>(Resource.Id.rp_tiet_password);
        TextInputEditText txtConfirmPassword = FindViewById<TextInputEditText>(Resource.Id.rp_tiet_confirm_password);

        #region VALIDATION
        txtIlPassword.Error = "";
        txtIlConfirmPassword.Error = "";

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

        if (txtPassword.Text != txtConfirmPassword.Text) {
          txtIlPassword.Error = "Passwords do not match";
          txtIlPassword.SetErrorTextAppearance(Color.Red.ToArgb());
          txtIlConfirmPassword.Error = "Passwords do not match";
          txtIlConfirmPassword.SetErrorTextAppearance(Color.Red.ToArgb());
          return;
        }
        #endregion

        //update the user's password in the database
        User user = JsonConvert.DeserializeObject<User>(Intent.GetStringExtra("User"));
        user.PASSWORD = txtPassword.Text;

        UserDao userDao = new UserDao();

        try {
          userDao.UpdateUser(user);
        } catch (Exception ex) {
          new MaterialAlertDialogBuilder(this)
            .SetTitle("An error has occurred. Please contact the app administrator. Exception: " + ex.Message)
            .SetPositiveButton("OK", (sender, e) => { })
            .Show();
        }
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