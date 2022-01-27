using Android.App;
using Android.OS;
using Android.Widget;

/*
* Ernie Phillips III : 01/27/2022
* Purpose: Handle the main page
* Function: Main page for app access, displays login and reg buttons
*/

namespace WeightApp.Activities {
  [Activity(Label = "UserAccessActivity", NoHistory = true)]
  public class UserAccessActivity : Activity {

    protected override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);
      SetContentView(Resource.Layout.activity_user_access);

      Button btnLogin = FindViewById<Button>(Resource.Id.aua_login);
      Button btnRegister = FindViewById<Button>(Resource.Id.aua_register);
      
      btnLogin.Click += delegate {
        StartActivity(typeof(LoginActivity));
      };

      btnRegister.Click += delegate {
        StartActivity(typeof(RegisterActivity));
      };
    }
  }
}