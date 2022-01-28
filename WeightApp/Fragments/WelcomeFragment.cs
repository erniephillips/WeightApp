using Android.OS;
using Android.Views;
using Android.Widget;

/*
* Ernie Phillips III : 01/27/2022
* Purpose: Handle the page displayed to first time user
* Function: Page displays information about the app on visit if the profile has not been filled out.
*/

namespace WeightApp.Fragments {
  public class WelcomeFragment : AndroidX.Fragment.App.Fragment {
    public override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);
      // Create your fragment here
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
      View view = inflater.Inflate(Resource.Layout.fragment_welcome, container, false);

      //get the button
      Button btnMyProfile = view.FindViewById<Button>(Resource.Id.btn_go_to_profile);

      //ImageView img = view.FindViewById<ImageView>(Resource.Id.veggies);
      //DisplayMetrics metrics = new DisplayMetrics();
      //IWindowManager wm = Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
      //wm.DefaultDisplay.GetMetrics(metrics);
      //int height = metrics.HeightPixels;
      //int width = metrics.WidthPixels;

      //img.LayoutParameters.Height = height - 50;
      //img.LayoutParameters.Width = width - 50;

      //set the button click event
      btnMyProfile.Click += delegate { 
        //load the profile fragment
        this.FragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, new ProfileFragment(), "Fragment").Commit();
      };

      return view;
    }
  }
}