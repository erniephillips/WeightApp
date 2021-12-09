using Android.OS;
using Android.Views;

/*
 * Ernie Phillips III : 12/09/2021
 * Contact me page that will have text entry fields and open in default mail app on phone
 */

namespace WeightApp.Fragments {
  public class ContactFragment : AndroidX.Fragment.App.Fragment {
    public override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);

      // Create your fragment here
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
      // Use this to return your custom view for this Fragment
      return inflater.Inflate(Resource.Layout.fragment_contact, container, false);
    }
  }
}