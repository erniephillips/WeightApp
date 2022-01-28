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

      //find the xml view to set
      SetContentView(Resource.Layout.activity_user_access);

      //set the register and login buttons
      Button btnLogin = FindViewById<Button>(Resource.Id.aua_login);
      Button btnRegister = FindViewById<Button>(Resource.Id.aua_register);
      
      //set event handlers for buttons
      btnLogin.Click += delegate {
        StartActivity(typeof(LoginActivity));
      };

      btnRegister.Click += delegate {
        StartActivity(typeof(RegisterActivity));
      };
    }
  }
}