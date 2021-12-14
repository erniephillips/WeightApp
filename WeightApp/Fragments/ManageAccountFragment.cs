using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using DataAccessLayer.Dao;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WeightApp.Activities;

namespace WeightApp.Fragments {
  public class ManageAccountFragment : AndroidX.Fragment.App.Fragment {
    public override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);

      // Create your fragment here
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
      View view = inflater.Inflate(Resource.Layout.fragment_manage_account, container, false);

      //get current user's information
      UserDao userDao = new UserDao();
      ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
      string userId = pref.GetString("UserId", String.Empty);
      User user = userDao.GetUser(Convert.ToInt32(userId));

      TextView txtUsername = view.FindViewById<TextView>(Resource.Id.tv_ma_user_username);
      TextView txtName = view.FindViewById<TextView>(Resource.Id.tv_ma_user_name);
      TextView txtEmail = view.FindViewById<TextView>(Resource.Id.tv_ma_user_email);
      TextView txtLastLogin = view.FindViewById<TextView>(Resource.Id.tv_ma_user_last_login);
      ScrollView scrollView = view.FindViewById<ScrollView>(Resource.Id.ma_scroll);
      Button btnUpdateInfo = view.FindViewById<Button>(Resource.Id.btn_ma_update_info);
      Button btnUpdatePassword = view.FindViewById<Button>(Resource.Id.btn_ma_change_password);
      Button btnUpdateSecurityInfo = view.FindViewById<Button>(Resource.Id.btn_ma_update_sec_info);
      EditText etName = view.FindViewById<EditText>(Resource.Id.et_ma_name);
      EditText etEmail = view.FindViewById<EditText>(Resource.Id.et_ma_email);
      EditText etCurrentPassword = view.FindViewById<EditText>(Resource.Id.et_ma_current_password);
      EditText etNewPassword = view.FindViewById<EditText>(Resource.Id.et_ma_password);
      EditText etConfirmPassword = view.FindViewById<EditText>(Resource.Id.et_ma_confirm_password);
      EditText etSecurityAnswer = view.FindViewById<EditText>(Resource.Id.et_ma_sec_answer);
      TextView txtInfoErrors = view.FindViewById<TextView>(Resource.Id.txt_ma_info_errors);
      TextView txtPasswordErrors = view.FindViewById<TextView>(Resource.Id.txt_ma_password_errors);
      TextView txtSecInfoErrors = view.FindViewById<TextView>(Resource.Id.txt_ma_sec_info_errors);

      AlertDialog.Builder dialog = new AlertDialog.Builder(Activity);

      //set the user's information for display
      txtUsername.Text = user.USERNAME;
      txtName.Text = user.NAME;
      txtEmail.Text = user.EMAIL;
      txtLastLogin.Text = user.LAST_LOGIN_DATE.ToString();

      //set the spinner, load all values first then loop each value and match with stored question, then set index of spinner when matched
      Spinner spinner = view.FindViewById<Spinner>(Resource.Id.spinner_ma_sec_question);
      ArrayAdapter adapter = ArrayAdapter.CreateFromResource(Activity, Resource.Array.security_question_array, Android.Resource.Layout.SimpleSpinnerItem);
      adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
      spinner.Adapter = adapter;
      for (int i = 0; i < spinner.Count; i++) {
        if (spinner.GetItemAtPosition(i).ToString().Equals(user.SECURITY_QUESTION)) {
          spinner.SetSelection(i);
        }
      }

      //set button click events

      //Update Info btn click
      btnUpdateInfo.Click += (s, e) => {
        #region VALIDATION
        txtInfoErrors.Text = ""; //clear errors
        //check for null
        if(String.IsNullOrEmpty(etName.Text) || String.IsNullOrEmpty(etEmail.Text)) {
          if (String.IsNullOrEmpty(etName.Text))
            txtInfoErrors.Append("Name is required\n");
          if (String.IsNullOrEmpty(etEmail.Text))
            txtInfoErrors.Append("Email is required\n");
          //scrollView.FullScroll(FocusSearchDirection.Down);
          return;
        }

        //check email
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        var regex = new Regex(pattern);
        if (!regex.IsMatch(etEmail.Text)) {
          txtInfoErrors.Append("Please enter a valid email address\n");
          //scrollView.FullScroll(FocusSearchDirection.Down);
          return;
        }
        #endregion

        //update user info
        user.NAME = etName.Text;
        user.EMAIL = etEmail.Text;
        userDao.UpdateUser(user);

        AlertDialog alertNameEmail = dialog.Create();
        alertNameEmail.SetTitle("Weight App Alert");
        alertNameEmail.SetMessage("Name and email have been updated");
        alertNameEmail.SetButton("OK", (c, ev) => {
          //reload page
          this.FragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, new ManageAccountFragment(), "Fragment").Commit();
        });
        alertNameEmail.Show();
      };

      //Password btn click
      btnUpdatePassword.Click += (s, e) => {
        #region VALIDATION
        txtPasswordErrors.Text = ""; //clear errors
        if (String.IsNullOrEmpty(etCurrentPassword.Text) || String.IsNullOrEmpty(etNewPassword.Text) || String.IsNullOrEmpty(etConfirmPassword.Text)) {
          if (String.IsNullOrEmpty(etCurrentPassword.Text))
            txtPasswordErrors.Append("Current password is required\n");
          if (String.IsNullOrEmpty(etNewPassword.Text))
            txtPasswordErrors.Append("New password is required\n");
          if (String.IsNullOrEmpty(etConfirmPassword.Text))
            txtPasswordErrors.Append("Password confirmation is required\n");
          //scrollView.FullScroll(FocusSearchDirection.Down);
          return;
        }

        //check the current password against db store
        if (etCurrentPassword.Text != user.PASSWORD) {
          txtPasswordErrors.Text = "Current password does not match your entry\n";
          //scrollView.FullScroll(FocusSearchDirection.Down);
          return;
        }

        if(etNewPassword.Text != etConfirmPassword.Text) {
          txtPasswordErrors.Text = "New password and confirm password do not match\n";
          //scrollView.FullScroll(FocusSearchDirection.Down);
          return;
        }
        #endregion


        //update the user's password
        user.PASSWORD = etNewPassword.Text;
        userDao.UpdateUser(user);

        //confirmation
        AlertDialog alertPasswordUpdated = dialog.Create();
        alertPasswordUpdated.SetTitle("Weight App Alert");
        alertPasswordUpdated.SetMessage("Password has been updated. Please login again.");
        alertPasswordUpdated.SetButton("OK", (c, ev) => {
          //log the user out and have them log back in
          ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
          ISharedPreferencesEditor edit = pref.Edit();
          edit.Clear();
          edit.Commit();
          Activity.StartActivity(typeof(LoginActivity));
        });
        alertPasswordUpdated.Show();
      };
      
      //Security info btn click
      btnUpdateSecurityInfo.Click += (s, e) => {
        #region VALIDATION
        txtSecInfoErrors.Text = ""; //clear errors
        if (String.IsNullOrEmpty(etSecurityAnswer.Text)) {
          txtSecInfoErrors.Text = "Security answer is required\n";
          //scrollView.FullScroll(FocusSearchDirection.Down);
          return;
        }
        #endregion

        string selectedSpinnerValue = spinner.SelectedItem.ToString();

        user.SECURITY_QUESTION = selectedSpinnerValue;
        user.SECURITY_ANSWER = etSecurityAnswer.Text; ;
        userDao.UpdateUser(user);

        //confirmation
        AlertDialog alertSecurityInfo = dialog.Create();
        alertSecurityInfo.SetTitle("Weight App Alert");
        alertSecurityInfo.SetMessage("Security info has been updated");
        alertSecurityInfo.SetButton("OK", (c, ev) => {
          //reload page
          this.FragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, new ManageAccountFragment(), "Fragment").Commit();
        });
        alertSecurityInfo.Show();
      };

      return view;
    }
  }
}