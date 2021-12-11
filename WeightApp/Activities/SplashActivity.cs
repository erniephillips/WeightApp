using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using AndroidX.AppCompat.App;
using System.Threading.Tasks;
using WeightApp;

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
      Log.Debug(TAG, "Startup work is finished - starting MainActivity.");
      StartActivity(new Intent(Application.Context, typeof(MainActivity))); //start the main activity
    }
  }
}