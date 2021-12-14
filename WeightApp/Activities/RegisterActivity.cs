using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DataAccessLayer.Dao;
using DataAccessLayer.Models;
using Google.Android.Material.Snackbar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WeightApp.Activities {
  [Activity(Label = "RegisterActivity", NoHistory = true)]
  public class RegisterActivity : Activity {
    protected override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);
      SetContentView(Resource.Layout.activity_register);

      ImageButton btnBack = FindViewById<ImageButton>(Resource.Id.btn_register_back);
      Button btnRegister = FindViewById<Button>(Resource.Id.btn_reg_register);
      ImageView btnInfo = FindViewById<ImageView>(Resource.Id.btn_register_info);

      //populate the drowdown
      Spinner spinner = FindViewById<Spinner>(Resource.Id.spinner_reg_sec_question);
      ArrayAdapter adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.security_question_array, Android.Resource.Layout.SimpleSpinnerItem);
      adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
      spinner.Adapter = adapter;

      btnBack.Click += delegate {
        StartActivity(typeof(UserAccessActivity));
      };

      btnInfo.Click += (s, e) => {
        //set a snackbar message for user about data. Data was being truncated. Override the snackbar view max lines
        Snackbar snackbar = Snackbar.Make(btnInfo, "Data is only stored on your phone and cannot be recovered. Please remember your login information and security answer.", 7000);
        View snackbarView = snackbar.View;
        TextView snackTextView = (TextView)snackbarView.FindViewById(Resource.Id.snackbar_text);
        snackTextView.SetMaxLines(3);
        snackbar.Show();
      };

      btnRegister.Click += (s, e) => {
        UserDao userDao = new UserDao();
        string txtUsername = FindViewById<EditText>(Resource.Id.et_username).Text;
        string txtPassword = FindViewById<EditText>(Resource.Id.et_password).Text;
        string txtConfirmPassword = FindViewById<EditText>(Resource.Id.et_confirm_password).Text;
        string txtName = FindViewById<EditText>(Resource.Id.et_name).Text;
        string txtEmail = FindViewById<EditText>(Resource.Id.et_email).Text;
        string txtSecurityAnswer = FindViewById<EditText>(Resource.Id.et_reg_sec_answer).Text;
        TextView txtRegistrationErrors = FindViewById<TextView>(Resource.Id.txt_reg_errors);


        #region VALIDATION
        txtRegistrationErrors.Text = ""; //clear textview if any value exists

        if (txtUsername == "" || txtPassword == "" || txtConfirmPassword == "" || txtName == "" || txtEmail == "" || txtSecurityAnswer == "") {
          if (txtUsername == "")
            txtRegistrationErrors.Append("Please enter a username\n");
          if (txtPassword == "")
            txtRegistrationErrors.Append("Please enter a password\n");
          if (txtConfirmPassword == "")
            txtRegistrationErrors.Append("Please enter a confirm password\n");
          if (txtName == "")
            txtRegistrationErrors.Append("Please enter a name\n");
          if (txtEmail == "")
            txtRegistrationErrors.Append("Please enter an email\n");
          if (txtSecurityAnswer == "")
            txtRegistrationErrors.Append("Please enter a security answer\n");
          return;
        }

        //check passwords
        if (txtPassword != txtConfirmPassword) {
          txtRegistrationErrors.Append("Passwords do not match");
          return;
        }

        //check email
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        var regex = new Regex(pattern);
        if (!regex.IsMatch(txtEmail)) {
          txtRegistrationErrors.Append("Please enter a valid email address");
          return;
        }

        //check username does not exist
        User user = userDao.GetUserByUsername(txtUsername);
        if (user != null)
          txtRegistrationErrors.Append("Username already exist, please choose another one");
        #endregion

        string ddlValue = spinner.SelectedItem.ToString();


        //If reached here, all validation passed, time to store the user
        User userToStore = new User() {
          USERNAME = txtUsername,
          PASSWORD = txtPassword,
          EMAIL = txtEmail,
          NAME = txtName,
          IS_LOCKED = false,
          FAILED_LOGIN_ATTEMPT = 0,
          CREATED_DATE = DateTime.Now,
          LAST_LOGIN_DATE = DateTime.Now,
          SECURITY_QUESTION = ddlValue,
          SECURITY_ANSWER = txtSecurityAnswer
        };

        userDao.AddUser(userToStore);

        //redirect to login page
        StartActivity(typeof(LoginActivity)); //send user to main applicaiton logic
      };
    }
  }
}