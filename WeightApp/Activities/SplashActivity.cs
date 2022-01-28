using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using AndroidX.AppCompat.App;
using DataAccessLayer.Dao;
using System;
using System.Threading.Tasks;
using WeightApp;
using WeightApp.Activities;

/*
* Ernie Phillips III : 12/09/2021
* Creating a splash activity : https://docs.microsoft.com/en-us/xamarin/android/user-interface/splash-screen
* Function: shows a splash screen for 1 second. Page also checks for ISharedPreferences varibles about user and if remember password was checked. 
*/

namespace AndroidApp {
  //Theme points to styles.xml
  //Tell Application this is the main activity to be launched
  //NoHistory means activity will not leave a historical trace and will not remain in activity stack for the task so user cannot return to it
  [Activity(Theme = "@style/AppTheme.Splash", MainLauncher = true, NoHistory = true)]
  public class SplashActivity : AppCompatActivity {

    //initialize the activity
    public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState) {
      base.OnCreate(savedInstanceState, persistentState);
    }

    // Launches the startup task
    protected override void OnResume() {
      base.OnResume();
      Task startupWork = new Task(() => { SimulateStartup(); });
      startupWork.Start();
    }

    // Simulates background work that happens behind the splash screen
    async void SimulateStartup() {
      
      await Task.Delay(1000); // Simulate a bit of startup work.
      
      //check users last login date, if within two weeks and logged in flag exists
      //https://www.c-sharpcorner.com/article/shared-preferences-in-xamarin-android/
      ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);

      string userId = pref.GetString("UserId", String.Empty);
      string userName = pref.GetString("Username", String.Empty);
      string name = pref.GetString("Name", String.Empty);
      string password = pref.GetString("Password", String.Empty);
      string lastLogin = pref.GetString("LastLogin", String.Empty);

      if (userId == null || userName == String.Empty || name == String.Empty || password == String.Empty || lastLogin == null) {
        //No saved credentials, take user to login screen  
        StartActivity(typeof(UserAccessActivity));
      } else {
        //instantiate user dao
        UserDao userDao = new UserDao();

        //get last login
        DateTime parsedDate = DateTime.Parse(lastLogin);
        bool loginLessThanTwoWeeks = (parsedDate > DateTime.Now.AddDays(-14)); //check last login against today's date
        bool userCanLogin = userDao.VerifyLogin(userName, password); //verify username and password exist
        bool isAccountLocked = userDao.GetUserByUsername(userName).IS_LOCKED; //check if the user's account is locked (NOT CURRENTY SET UP)

        if (userCanLogin && loginLessThanTwoWeeks && !isAccountLocked) { //verify correct password and last login not older than 2 weeks and account not locked
          StartActivity(typeof(MainActivity)); //send user to main applicaiton logic
        } else { //user did not authenticate properly, send to UserAccess login page
          StartActivity(typeof(UserAccessActivity));
        }
      }
    }
  }
}