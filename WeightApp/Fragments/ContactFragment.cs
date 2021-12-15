using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;

/*
 * Ernie Phillips III : 12/09/2021
 * Contact me page that will have text entry fields and open in default mail app on phone
 */

namespace WeightApp.Fragments {
  public class ContactFragment : AndroidX.Fragment.App.Fragment {
    public override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);

    // send email using native phone app
    //https://docs.microsoft.com/en-us/xamarin/essentials/email?tabs=android

    //why I shouldn't use SMTP in the app (should build web service instead)
    //https://social.msdn.microsoft.com/Forums/en-US/f0f661d2-24db-47b1-9fc7-6be02685f8f1/how-to-send-smtp-email-using-xamarinforms-without-user-interaction?forum=xamarinforms
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
      // Use this to return your custom view for this Fragment
      View view = inflater.Inflate(Resource.Layout.fragment_contact, container, false);

      ScrollView scrollView = view.FindViewById<ScrollView>(Resource.Id.contact_scrollview);
      Button btnSubmit = view.FindViewById<Button>(Resource.Id.btn_contact_submit);
      TextView txtName = view.FindViewById<TextView>(Resource.Id.et_contact_your_name);
      TextView txtComments = view.FindViewById<TextView>(Resource.Id.et_contact_comments);
      TextView txtContactErrors = view.FindViewById<TextView>(Resource.Id.txt_contact_errors);


      //populate the drowdown
      Spinner spinner = view.FindViewById<Spinner>(Resource.Id.spinner_contact_reason);
      ArrayAdapter adapter = ArrayAdapter.CreateFromResource(Activity, Resource.Array.contact_page_dropdown_array, Android.Resource.Layout.SimpleSpinnerItem);
      adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
      spinner.Adapter = adapter;

      btnSubmit.Click += (s, e) => {
        #region VALIDATION
        txtContactErrors.Text = "";
        if(string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtComments.Text)) {
          if (string.IsNullOrEmpty(txtName.Text))
            txtContactErrors.Append("Name is required\n");
          if (string.IsNullOrEmpty(txtComments.Text))
            txtContactErrors.Append("Comments are required\n");
          scrollView.FullScroll(FocusSearchDirection.Down);
          return;
        }
        #endregion

        //send an email to myself
        //https://docs.microsoft.com/en-us/xamarin/essentials/email?tabs=android
        List<string> emailRecipients = new List<string>() {
          new string("erniephillips.android@gmail.com")
        };
        try {
          var message = new EmailMessage {
            Subject = spinner.SelectedItem.ToString() + " - " + txtName.Text,
            Body = txtComments.Text,
            To = emailRecipients
          };
          Email.ComposeAsync(message);
        } catch (FeatureNotSupportedException fbsEx) {
          // Email is not supported on this device
          AlertDialog.Builder dialog = new AlertDialog.Builder(Activity);
          AlertDialog alert = dialog.Create();
          alert.SetTitle("Weight App Alert");
          alert.SetMessage("Email is not supported on this device. Please email me directly at erniephillips.android@gmail.com. Thank you.");
          alert.SetButton("OK", (c, ev) => {
            // Ok button click task  
          });
          alert.Show();
        } catch (Exception ex) {
          // Some other exception occurred
          AlertDialog.Builder dialog = new AlertDialog.Builder(Activity);
          AlertDialog alert = dialog.Create();
          alert.SetTitle("Weight App Alert");
          alert.SetMessage("An unexpected error occurred. Please email me directly at erniephillips.android@gmail.com. Thank you.");
          alert.SetButton("OK", (c, ev) => {
            // Ok button click task  
          });
          alert.Show();
        }
      };

      return view;
    }
  }
}