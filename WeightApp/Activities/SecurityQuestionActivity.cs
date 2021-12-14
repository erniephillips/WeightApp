using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Percent;
using Android.Views;
using Android.Widget;
using DataAccessLayer.Dao;
using DataAccessLayer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeightApp.Activities;

namespace WeightApp.Fragments {
  [Activity(Label = "SecurityQuestionActivity")]
  public class SecurityQuestionActivity : Activity {
    protected override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);
      SetContentView(Resource.Layout.activity_security_question);

      ImageButton btnBack = FindViewById<ImageButton>(Resource.Id.btn_sq_back);
      Button btnVerifySA = FindViewById<Button>(Resource.Id.btn_verify_sa);
      TextView txtSecurityQuestion = FindViewById<TextView>(Resource.Id.txt_security_question);
      
      //set the security question text
      User user = JsonConvert.DeserializeObject<User>(Intent.GetStringExtra("User"));
      txtSecurityQuestion.Text = user.SECURITY_QUESTION.ToString();
      PercentRelativeLayout.LayoutParams lp = new PercentRelativeLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
      
      lp.AddRule(LayoutRules.Below, Resource.Id.tv_sec_q);
      lp.LeftMargin = 50;
      lp.TopMargin = 100;
      txtSecurityQuestion.LayoutParameters = lp;


      btnBack.Click += delegate {
        StartActivity(typeof(ForgotPasswordActivity));
      };

      btnVerifySA.Click += (s, e) => {
        EditText etSecurityAnswer = FindViewById<EditText>(Resource.Id.et_security_answer);
        TextView txtError = FindViewById<TextView>(Resource.Id.txt_sq_errors);

        #region VALIDATION
        txtError.Text = "";
        if (String.IsNullOrEmpty(etSecurityAnswer.Text)) {
          txtError.Text = "Security answer is required";
          return;
        }

        if (etSecurityAnswer.Text.ToLower() != user.SECURITY_ANSWER.ToLower()) {
          txtError.Text = "Incorrect security answer provided";
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