using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using AndroidX.AppCompat.App;
using DataAccessLayer.Dao;
using DataAccessLayer.Models;
using System;
using System.Threading.Tasks;
using WeightApp;
using WeightApp.Activities;

/*
* Ernie Phillips III : 12/09/2021
* Creating a splash activity : https://docs.microsoft.com/en-us/xamarin/android/user-interface/splash-screen
*/

namespace AndroidApp {
  //Theme points to styles.xml
  //Tell Application this is the main activity to be launched
  //NoHistory means activity will not leave a historical trace and will not remain in activity stack for the task so user cannot return to it
  [Activity(Theme = "@style/AppTheme.Splash", MainLauncher = true, NoHistory = true)]
  public class SplashActivity : AppCompatActivity {

    static readonly string TAG = "X:" + typeof(SplashActivity).Name;

    //initialize the activity
    public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState) {
      base.OnCreate(savedInstanceState, persistentState);
      Log.Debug(TAG, "SplashActivity.OnCreate");
    }

    // Launches the startup task
    protected override void OnResume() {
      base.OnResume();
      Task startupWork = new Task(() => { SimulateStartup(); });
      startupWork.Start();
    }

    // Simulates background work that happens behind the splash screen
    async void SimulateStartup() {
      Log.Debug(TAG, "Performing some startup work that takes a bit of time.");
      await Task.Delay(1000); // Simulate a bit of startup work.
      Log.Debug(TAG, "Startup work is finished - checking login.");

      //check users last login date, if within two weeks and logged in flag exists
      //https://www.c-sharpcorner.com/article/shared-preferences-in-xamarin-android/
      ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);

      string userId = pref.GetString("UserId", String.Empty);
      string userName = pref.GetString("Username", String.Empty);
      string password = pref.GetString("Password", String.Empty);
      string lastLogin = pref.GetString("LastLogin", String.Empty);

      if (userId == null || userName == String.Empty || password == String.Empty || lastLogin == null) {
        //No saved credentials, take user to login screen  
        StartActivity(typeof(UserAccessActivity));
      } else {
        UserDao userDao = new UserDao();
        DateTime parsedDate = DateTime.Parse(lastLogin);
        bool loginLessThanTwoWeeks = (parsedDate > DateTime.Now.AddDays(-14));
        bool userCanLogin = userDao.VerifyLogin(userName, password);
        bool isAccountLocked = userDao.GetUserByUsername(userName).IS_LOCKED;

        if (userCanLogin && loginLessThanTwoWeeks && !isAccountLocked) { //verify correct password and last login not older than 2 weeks and account not locked
          StartActivity(typeof(MainActivity)); //send user to main applicaiton logic
        } else { //user did not authenticate properly, send to UserAccess login page
          StartActivity(typeof(UserAccessActivity));
        }
      }
    }
  }
}