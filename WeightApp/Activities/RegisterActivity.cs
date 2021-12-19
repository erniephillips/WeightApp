using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DataAccessLayer.Dao;
using DataAccessLayer.Models;
using Google.Android.Material.Dialog;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WeightApp.Activities {
  [Activity(Theme = "@style/AppTheme", Label = "RegisterActivity", NoHistory = true)]
  public class RegisterActivity : Activity {
    protected override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);
      SetContentView(Resource.Layout.activity_register);

      ImageButton btnBack = FindViewById<ImageButton>(Resource.Id.btn_register_back);
      Button btnRegister = FindViewById<Button>(Resource.Id.btn_reg_register);
      //ImageView btnInfo = FindViewById<ImageView>(Resource.Id.btn_register_info);
      TextView txtRegister = FindViewById<TextView>(Resource.Id.tv_Register);
      ScrollView scrollView = FindViewById<ScrollView>(Resource.Id.reg_scrollview);

      //populate the drowdown
      AutoCompleteTextView dropdownItems = FindViewById<AutoCompleteTextView>(Resource.Id.reg_populate_dropdown);
      ArrayAdapter adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.security_question_array, Android.Resource.Layout.SimpleSpinnerItem);
      adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
      dropdownItems.Adapter = adapter;

      btnBack.Click += delegate {
        StartActivity(typeof(UserAccessActivity));
      };

      txtRegister.Click += (s, e) => {
        //set a snackbar message for user about data. Data was being truncated. Override the snackbar view max lines
        Snackbar snackbar = Snackbar.Make(txtRegister, "Data is only stored on your phone and cannot be recovered. Please remember your login information and security answer.", 7000);
        View snackbarView = snackbar.View;
        TextView snackTextView = (TextView)snackbarView.FindViewById(Resource.Id.snackbar_text);
        snackTextView.SetMaxLines(3);
        snackbar.Show();
      };

      btnRegister.Click += (s, e) => {
        UserDao userDao = new UserDao();
        TextInputLayout txtIlDropdown = FindViewById<TextInputLayout>(Resource.Id.spinner_reg_sec_question);
        TextInputLayout txtIlUsername = FindViewById<TextInputLayout>(Resource.Id.et_username);
        TextInputLayout txtIlPassword = FindViewById<TextInputLayout>(Resource.Id.et_password);
        TextInputLayout txtIlConfirmPassword = FindViewById<TextInputLayout>(Resource.Id.et_confirm_password);
        TextInputLayout txtIlName = FindViewById<TextInputLayout>(Resource.Id.et_name);
        TextInputLayout txtIlEmail = FindViewById<TextInputLayout>(Resource.Id.et_email);
        TextInputLayout txtIlSecAns = FindViewById<TextInputLayout>(Resource.Id.et_reg_sec_answer);

        TextInputEditText txtUsername = FindViewById<TextInputEditText>(Resource.Id.register_tiet_username);
        TextInputEditText txtPassword = FindViewById<TextInputEditText>(Resource.Id.register_tiet_password);
        TextInputEditText txtConfirmPassword = FindViewById<TextInputEditText>(Resource.Id.register_tiet_confirm_password);
        TextInputEditText txtName = FindViewById<TextInputEditText>(Resource.Id.register_tiet_name);
        TextInputEditText txtEmail = FindViewById<TextInputEditText>(Resource.Id.register_tiet_email);
        TextInputEditText txtSecurityAnswer = FindViewById<TextInputEditText>(Resource.Id.register_tiet_security_answer);
        

        #region VALIDATION
        txtIlUsername.Error = "";
        txtIlPassword.Error = "";
        txtIlConfirmPassword.Error = "";
        txtIlName.Error = "";
        txtIlEmail.Error = "";
        txtIlSecAns.Error = "";
        txtIlDropdown.Error = "";

        bool isDropdownDefaultValue = dropdownItems.Text == "Select an item...";

        //check for nulls
        if (txtUsername.Text == "" || txtPassword.Text == "" || txtConfirmPassword.Text == "" || txtName.Text == "" || txtEmail.Text == "" || txtSecurityAnswer.Text == "" || isDropdownDefaultValue) {
          if (txtUsername.Text == "") {
            txtIlUsername.Error = "Username is required";
            txtIlUsername.SetErrorTextAppearance(Color.Red.ToArgb());
          }
          if (txtPassword.Text == "") {
            txtIlPassword.Error = "Password is required";
            txtIlPassword.SetErrorTextAppearance(Color.Red.ToArgb());
          }
          if (txtConfirmPassword.Text == "") {
            txtIlConfirmPassword.Error = "Password confirmation is required";
            txtIlConfirmPassword.SetErrorTextAppearance(Color.Red.ToArgb());
          }
          if (txtName.Text == "") {
            txtIlName.Error = "Name is required";
            txtIlName.SetErrorTextAppearance(Color.Red.ToArgb());
          }
          if (txtEmail.Text == "") {
            txtIlEmail.Error = "Email is required";
            txtIlEmail.SetErrorTextAppearance(Color.Red.ToArgb());
          }
          if (isDropdownDefaultValue) {
            txtIlDropdown.Error = "Security question selection is required";
            txtIlDropdown.SetErrorTextAppearance(Color.Red.ToArgb());
          }
          if (txtSecurityAnswer.Text == "") {
            txtIlSecAns.Error = "Security answer is required";
            txtIlSecAns.SetErrorTextAppearance(Color.Red.ToArgb());
          }
          return;
        }

        //check passwords
        if (txtPassword.Text != txtConfirmPassword.Text) {
          txtIlPassword.Error = "Passwords do not match";
          txtIlPassword.SetErrorTextAppearance(Color.Red.ToArgb());
          txtIlConfirmPassword.Error = "Passwords do not match";
          txtIlConfirmPassword.SetErrorTextAppearance(Color.Red.ToArgb());
          //scrollView.FullScroll(FocusSearchDirection.Down);
          return;
        }

        //check email
        string pattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,})+)$";
        var regex = new Regex(pattern);
        if (!regex.IsMatch(txtEmail.Text)) {
          txtIlEmail.Error = "Enter a valid email address";
          txtIlEmail.SetErrorTextAppearance(Color.Red.ToArgb());
          return;
        }

        //check username for spaces
        if (txtUsername.Text.Any(x => Char.IsWhiteSpace(x))) {
          txtIlUsername.Error = "Username may not contain any spaces";
          txtIlUsername.SetErrorTextAppearance(Color.Red.ToArgb());
          return;
        }

        //check username does not exist
        User user = userDao.GetUserByUsername(txtUsername.Text);
        if (user != null) {
          txtIlUsername.Error = "Username already exist, please choose another one";
          txtIlUsername.SetErrorTextAppearance(Color.Red.ToArgb());
          return;
        }
        #endregion

        string ddlValue = dropdownItems.Text.ToString();


        //If reached here, all validation passed, time to store the user
        User userToStore = new User() {
          USERNAME = txtUsername.Text,
          PASSWORD = txtPassword.Text,
          EMAIL = txtEmail.Text,
          NAME = txtName.Text,
          IS_LOCKED = false,
          FAILED_LOGIN_ATTEMPT = 0,
          CREATED_DATE = DateTime.Now,
          LAST_LOGIN_DATE = DateTime.Now,
          SECURITY_QUESTION = ddlValue,
          SECURITY_ANSWER = txtSecurityAnswer.Text
        };

        try {
          userDao.AddUser(userToStore);
        } catch (Exception ex) {
          new MaterialAlertDialogBuilder(this)
             .SetTitle("An error has occurred. Please contact the app administrator. Exception: " + ex.Message)
             .SetPositiveButton("OK", (sender, e) => { })
             .Show();
        }
        //redirect to login page
        StartActivity(typeof(LoginActivity)); //send user to main applicaiton logic
      };
    }
  }
}