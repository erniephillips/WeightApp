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

      //find the xml view to set
      SetContentView(Resource.Layout.activity_forgot_password);

      //XML elements to be stored in variables
      ImageButton btnBack = FindViewById<ImageButton>(Resource.Id.btn_fp_back);
      Button btnForgotPassword = FindViewById<Button>(Resource.Id.btn_fp_check_username);

      //set the button and textview click events
      btnBack.Click += delegate {
        //navigate to login activity
        StartActivity(typeof(LoginActivity));
      };

      btnForgotPassword.Click += (s, e) => {
        //XML elements to be stored in variables
        TextInputLayout txtIlUsername = FindViewById<TextInputLayout>(Resource.Id.et_fp_username);
        TextInputEditText txtUsername = FindViewById<TextInputEditText>(Resource.Id.fp_tiet_username);

        #region VALIDATION
        //set error to empty on button click
        txtIlUsername.Error = "";

        //check username textbox for empty or null
        if (txtUsername.Text == "") {
          if (txtUsername.Text == "") {
            txtIlUsername.Error = "Username is required";
            txtIlUsername.SetErrorTextAppearance(Color.Red.ToArgb());
          }
          return;
        }
        
        //instantiate the user dao
        UserDao userDao = new UserDao();

        //check if passed username exists in db
        User user = userDao.GetUserByUsername(txtUsername.Text);
        
        //display user does not exist error to user if none found in db
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