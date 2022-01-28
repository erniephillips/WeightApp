using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using DataAccessLayer.Models;
using Google.Android.Material.TextField;
using Newtonsoft.Json;
using System;
using System.Drawing;
using WeightApp.Activities;

/*
* Ernie Phillips III : 01/27/2022
* Purpose: Handle the user's profile security question
* Function: User is prompted with their stored security question. If user answers correct, moved to the password reset screen
*/


namespace WeightApp.Fragments {
  [Activity(Label = "SecurityQuestionActivity")]
  public class SecurityQuestionActivity : Activity {
    protected override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);

      //find the xml view to set
      SetContentView(Resource.Layout.activity_security_question);

      //XML elements to be stored in variables
      ImageButton btnBack = FindViewById<ImageButton>(Resource.Id.btn_sq_back);
      Button btnVerifySA = FindViewById<Button>(Resource.Id.btn_verify_sa);
      TextView txtSecurityQuestion = FindViewById<TextView>(Resource.Id.txt_security_question);

      //An intent is a message passed between componets
      //Get the serialized user string and deserialize into object
      User user = JsonConvert.DeserializeObject<User>(Intent.GetStringExtra("User"));

      //set the security question text
      txtSecurityQuestion.Text = user.SECURITY_QUESTION.ToString();
      //PercentRelativeLayout.LayoutParams lp = new PercentRelativeLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
      //lp.AddRule(LayoutRules.Below, Resource.Id.tv_sec_q);
      //lp.LeftMargin = 50;
      //lp.TopMargin = 100;
      //txtSecurityQuestion.LayoutParameters = lp;

      //set the click events
      btnBack.Click += delegate {
        //navigate back to the forgot password activity
        StartActivity(typeof(ForgotPasswordActivity));
      };

      btnVerifySA.Click += (s, e) => {
        //XML elements to be stored in variables
        TextInputLayout txtIlSecurityAnswer = FindViewById<TextInputLayout>(Resource.Id.sq_security_answer);
        TextInputEditText txtSecurityAnswer = FindViewById<TextInputEditText>(Resource.Id.sq_tiet_sec_answer);

        #region VALIDATION
        //set error to empty on button click
        txtIlSecurityAnswer.Error = ""; 

        //check security answer is not null or empty
        if (String.IsNullOrEmpty(txtSecurityAnswer.Text)) {
          txtIlSecurityAnswer.Error = "Security answer is required";
          txtIlSecurityAnswer.SetErrorTextAppearance(Color.Red.ToArgb());
          return;
        }

        //verify sec answ matches what's stored in db
        if (txtSecurityAnswer.Text.ToLower() != user.SECURITY_ANSWER.ToLower()) {
          txtIlSecurityAnswer.Error = "Incorrect security answer provided";
          txtIlSecurityAnswer.SetErrorTextAppearance(Color.Red.ToArgb());
          return;
        }
        #endregion

        //serialize user obj and send to next activity. This will save on making another db call
        Intent intent = new Intent(this, typeof(ResetPasswordActivity));
        intent.PutExtra("User", JsonConvert.SerializeObject(user));
        StartActivity(intent);
        Finish();
      };
    }
  }
}