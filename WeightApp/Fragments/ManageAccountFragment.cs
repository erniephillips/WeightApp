using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using DataAccessLayer.Dao;
using DataAccessLayer.Models;
using Google.Android.Material.Dialog;
using Google.Android.Material.TextField;
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using WeightApp.Activities;

/*
* Ernie Phillips III : 01/27/2022
* Purpose: Handle the user account info
* Function: User can change password, security q & a, and name & email
*/

namespace WeightApp.Fragments {
  public class ManageAccountFragment : AndroidX.Fragment.App.Fragment {
    public override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);
      // Create your fragment here
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
      View view = inflater.Inflate(Resource.Layout.fragment_manage_account, container, false);

      //get current user's information
      ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
      string userId = pref.GetString("UserId", String.Empty);

      //instantiate the user dao
      UserDao userDao = new UserDao();

      //get user from db and store in obj
      User user = userDao.GetUser(Convert.ToInt32(userId));

      //set variables from XML elements
      TextView txtUsername = view.FindViewById<TextView>(Resource.Id.tv_ma_user_username);
      TextView txtName = view.FindViewById<TextView>(Resource.Id.tv_ma_user_name);
      TextView txtEmail = view.FindViewById<TextView>(Resource.Id.tv_ma_user_email);
      TextView txtLastLogin = view.FindViewById<TextView>(Resource.Id.tv_ma_user_last_login);
      ScrollView scrollView = view.FindViewById<ScrollView>(Resource.Id.ma_scroll);
      Button btnUpdateInfo = view.FindViewById<Button>(Resource.Id.btn_ma_update_info);
      Button btnUpdatePassword = view.FindViewById<Button>(Resource.Id.btn_ma_change_password);
      Button btnUpdateSecurityInfo = view.FindViewById<Button>(Resource.Id.btn_ma_update_sec_info);

      TextInputLayout txtIlName = view.FindViewById<TextInputLayout>(Resource.Id.et_ma_name);
      TextInputLayout txtIlEmail = view.FindViewById<TextInputLayout>(Resource.Id.et_ma_email);
      TextInputLayout txtIlCurrentPassword = view.FindViewById<TextInputLayout>(Resource.Id.et_ma_current_password);
      TextInputLayout txtIlNewPassword = view.FindViewById<TextInputLayout>(Resource.Id.et_ma_password);
      TextInputLayout txtIlConfirmPassword = view.FindViewById<TextInputLayout>(Resource.Id.et_ma_confirm_password);
      TextInputLayout txtIlSecurityAnswer = view.FindViewById<TextInputLayout>(Resource.Id.et_ma_sec_answer);

      TextInputEditText etName = view.FindViewById<TextInputEditText>(Resource.Id.ma_tiet_name);
      TextInputEditText etEmail = view.FindViewById<TextInputEditText>(Resource.Id.ma_tiet_email);
      TextInputEditText etCurrentPassword = view.FindViewById<TextInputEditText>(Resource.Id.ma_tiet_current_password);
      TextInputEditText etNewPassword = view.FindViewById<TextInputEditText>(Resource.Id.ma_tiet_new_password);
      TextInputEditText etConfirmPassword = view.FindViewById<TextInputEditText>(Resource.Id.ma_tiet_confirm_password);
      TextInputEditText etSecurityAnswer = view.FindViewById<TextInputEditText>(Resource.Id.ma_tiet_security_answer);


      //set the user's information for display
      txtUsername.Text = user.USERNAME;
      txtName.Text = user.NAME;
      txtEmail.Text = user.EMAIL;
      txtLastLogin.Text = user.LAST_LOGIN_DATE.ToString();

      //set the spinner, load all values first then loop each value and match with stored question, then set index of spinner when matched
      AutoCompleteTextView dropdownItems = view.FindViewById<AutoCompleteTextView>(Resource.Id.ma_populate_dropdown);
      ArrayAdapter adapter = ArrayAdapter.CreateFromResource(Activity, Resource.Array.security_question_array, Android.Resource.Layout.SimpleSpinnerItem);
      adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
      dropdownItems.Adapter = adapter;

      //set dropdown text
      dropdownItems.SetText(user.SECURITY_QUESTION, false);
      //for (int i = 0; i < dropdownItems.LineCount; i++) {
      //  if (dropdownItems.item(i).ToString().Equals(user.SECURITY_QUESTION)) {
      //    dropdownItems.SetSelection(i);
      //  }
      //}

      //set button click events

      //Update Info btn click
      btnUpdateInfo.Click += (s, e) => {
        #region VALIDATION
        //set errors to empty on button click
        txtIlName.Error = "";
        txtIlEmail.Error = "";

        //check name and email for null or empty and set errors
        if (String.IsNullOrEmpty(etName.Text) || String.IsNullOrEmpty(etEmail.Text)) {
          if (String.IsNullOrEmpty(etName.Text)) {
            txtIlName.Error = "Name is required";
            txtIlName.SetErrorTextAppearance(Color.Red.ToArgb());
          }
          if (String.IsNullOrEmpty(etEmail.Text)) {
            txtIlEmail.Error = "Email is required";
            txtIlEmail.SetErrorTextAppearance(Color.Red.ToArgb());
          }
          return;
        }

        //check validation on email
        string pattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,})+)$";
        var regex = new Regex(pattern);
        if (!regex.IsMatch(etEmail.Text)) {
          txtIlEmail.Error = "Please enter a valid email address";
          txtIlEmail.SetErrorTextAppearance(Color.Red.ToArgb());
          //scrollView.FullScroll(FocusSearchDirection.Down);
          return;
        }
        #endregion

        //update user info
        user.NAME = etName.Text;
        user.EMAIL = etEmail.Text;

        try { //try to update the user name and email
          userDao.UpdateUser(user);
        } catch (Exception ex) {
          new MaterialAlertDialogBuilder(Activity)
            .SetTitle("An error has occurred. Please contact the app administrator. Exception: " + ex.Message)
            .SetPositiveButton("OK", (sender, e) => { })
            .Show();
          return;
        }

        //show success message
        new MaterialAlertDialogBuilder(Activity)
        .SetTitle("Weight App Alert")
        .SetIcon(Resource.Drawable.ic_info)
        .SetMessage("Name and email have been updated.")
        .SetPositiveButton("OK", (sender, e) => {
          this.FragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, new ManageAccountFragment(), "Fragment").Commit();
        }).Show();
      };

      //Password btn click
      btnUpdatePassword.Click += (s, e) => {
        #region VALIDATION
        //set errors to empty on button click
        txtIlCurrentPassword.Error = "";
        txtIlNewPassword.Error = "";
        txtIlConfirmPassword.Error = "";

        //check current password, new pass, and conf pass for null or empty and set error
        if (String.IsNullOrEmpty(etCurrentPassword.Text) || String.IsNullOrEmpty(etNewPassword.Text) || String.IsNullOrEmpty(etConfirmPassword.Text)) {
          if (String.IsNullOrEmpty(etCurrentPassword.Text)) {
            txtIlCurrentPassword.Error = "Current password is required";
            txtIlCurrentPassword.SetErrorTextAppearance(Color.Red.ToArgb());
          }
          if (String.IsNullOrEmpty(etNewPassword.Text)) {
            txtIlNewPassword.Error = "New password is required";
            txtIlNewPassword.SetErrorTextAppearance(Color.Red.ToArgb());
          }
          if (String.IsNullOrEmpty(etConfirmPassword.Text)) {
            txtIlConfirmPassword.Error = "Password confirmation is required";
            txtIlConfirmPassword.SetErrorTextAppearance(Color.Red.ToArgb());
          }
          return;
        }

        //check the current password against db store
        if (etCurrentPassword.Text != user.PASSWORD) {
          txtIlCurrentPassword.Error = "Current password does not match your entry";
          txtIlCurrentPassword.SetErrorTextAppearance(Color.Red.ToArgb());
          return;
        }

        //check that passwords match
        if (etNewPassword.Text != etConfirmPassword.Text) {
          txtIlNewPassword.Error = "Passwords do not match";
          txtIlNewPassword.SetErrorTextAppearance(Color.Red.ToArgb());
          txtIlConfirmPassword.Error = "Passwords do not match";
          txtIlConfirmPassword.SetErrorTextAppearance(Color.Red.ToArgb());
          return;
        }
        #endregion


        //update the user's password
        user.PASSWORD = etNewPassword.Text;

        try { //try to update the user password
          userDao.UpdateUser(user);
        } catch (Exception ex) {//display error to user
          new MaterialAlertDialogBuilder(Activity)
            .SetTitle("An error has occurred. Please contact the app administrator. Exception: " + ex.Message)
            .SetPositiveButton("OK", (sender, e) => { })
            .Show();
          return;
        }

        //display successful message to user
        new MaterialAlertDialogBuilder(Activity)
        .SetTitle("Weight App Alert")
        .SetIcon(Resource.Drawable.ic_info)
        .SetMessage("Password has been updated. Please login again.")
        .SetPositiveButton("OK", (sender, e) => {
          ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
          ISharedPreferencesEditor edit = pref.Edit();
          edit.Clear();
          edit.Commit();
          Activity.StartActivity(typeof(LoginActivity));
        }).Show();
      };

      //Security info btn click
      btnUpdateSecurityInfo.Click += (s, e) => {
        #region VALIDATION
        //set error to empty on button click
        txtIlSecurityAnswer.Error = "";

        //check that answer is not equal to null
        if (String.IsNullOrEmpty(etSecurityAnswer.Text)) {
          txtIlSecurityAnswer.Error = "Security answer is required";
          txtIlSecurityAnswer.SetErrorTextAppearance(Color.Red.ToArgb());
          return;
        }
        #endregion

        //get selected dropdown in case user chose different security question
        string selectedSpinnerValue = dropdownItems.Text;

        //update user sec q & a
        user.SECURITY_QUESTION = selectedSpinnerValue;
        user.SECURITY_ANSWER = etSecurityAnswer.Text;

        try { //update the user
          userDao.UpdateUser(user);
        } catch (Exception ex) { //display error to user
          new MaterialAlertDialogBuilder(Activity)
            .SetTitle("An error has occurred. Please contact the app administrator. Exception: " + ex.Message)
            .SetPositiveButton("OK", (sender, e) => { })
            .Show();
          return;
        }

        //show success message to user
        new MaterialAlertDialogBuilder(Activity)
        .SetTitle("Weight App Alert")
        .SetIcon(Resource.Drawable.ic_info)
        .SetMessage("Security info has been updated.")
        .SetPositiveButton("OK", (sender, e) => {
          this.FragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, new ManageAccountFragment(), "Fragment").Commit();
        }).Show();
      };

      return view;
    }
  }
}