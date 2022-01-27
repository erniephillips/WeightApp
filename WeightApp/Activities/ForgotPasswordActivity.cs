using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using DataAccessLayer.Dao;
using DataAccessLayer.Models;
using Google.Android.Material.TextField;
using Newtonsoft.Json;
using System.Drawing;
using WeightApp.Fragments;

/*
* Ernie Phillips III : 01/27/2022
* Purpose: Handle the forgot password page
* Function: User is prompted for a username they submit which is then checked against names in db. 
*           If exists, user moves to security question page
*/

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

        TextInputLayout txtIlUsername = FindViewById<TextInputLayout>(Resource.Id.et_fp_username);
        TextInputEditText txtUsername = FindViewById<TextInputEditText>(Resource.Id.fp_tiet_username);

        #region VALIDATION
        txtIlUsername.Error = "";
        if (txtUsername.Text == "") {
          if (txtUsername.Text == "") {
            txtIlUsername.Error = "Username is required";
            txtIlUsername.SetErrorTextAppearance(Color.Red.ToArgb());
          }
          return;
        }

        User user = userDao.GetUserByUsername(txtUsername.Text);
        
        if (user == null) {
          txtIlUsername.Error = "Username not found";
          txtIlUsername.SetErrorTextAppearance(Color.Red.ToArgb());
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