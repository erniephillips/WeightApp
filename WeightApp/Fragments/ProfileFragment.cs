using Android.OS;
using Android.Views;

/*
* Ernie Phillips III : 12/09/2021
* Profile page, will pull and store in sqlite
*/

namespace WeightApp.Fragments {
  public class ProfileFragment : AndroidX.Fragment.App.Fragment {
    public override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);

      // Create your fragment here
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
      // Use this to return your custom view for this Fragment
      return inflater.Inflate(Resource.Layout.fragment_profile, container, false);
    }
  }
}