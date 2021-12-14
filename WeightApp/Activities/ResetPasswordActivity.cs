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
        TextView txtPassword = FindViewById<TextView>(Resource.Id.et_rp_password);
        TextView txtConfirmPassword = FindViewById<TextView>(Resource.Id.et_rp_confirm_password);
        TextView txtErrors = FindViewById<TextView>(Resource.Id.txt_rp_errors);

        #region VALIDATION
        txtErrors.Text = "";
        if (String.IsNullOrEmpty(txtPassword.Text) || String.IsNullOrEmpty(txtConfirmPassword.Text)) {
          if(String.IsNullOrEmpty(txtPassword.Text)) {
            txtErrors.Append("Password is required\n");
          }
          if (String.IsNullOrEmpty(txtConfirmPassword.Text)) {
            txtErrors.Append("Confirm password is required");
          }
          return;
        }

        if (txtPassword.Text != txtConfirmPassword.Text) {
          txtErrors.Text = "Passwords must match";
          return;
        }
        #endregion

        //update the user's password in the database
        User user = JsonConvert.DeserializeObject<User>(Intent.GetStringExtra("User"));
        user.PASSWORD = txtPassword.Text;

        UserDao userDao = new UserDao();
        userDao.UpdateUser(user);

        AlertDialog.Builder dialog = new AlertDialog.Builder(this);
        AlertDialog alert = dialog.Create();
        alert.SetTitle("Weight App Alert");
        alert.SetMessage("Password successfully reset");
        alert.SetButton("OK", (c, ev) => {
          StartActivity(typeof(LoginActivity));  
        });
        alert.Show();
      };

    }
  }
}