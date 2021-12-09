using Android.OS;
using Android.Views;

/*
* Ernie Phillips III : 12/09/2021
* Statistics page, will pull by user's stored records
*/

namespace WeightApp.Fragments {
  public class StatisticsFragment : AndroidX.Fragment.App.Fragment {
    public override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);

      // Create your fragment here
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
      // Use this to return your custom view for this Fragment
      return inflater.Inflate(Resource.Layout.fragment_statistics, container, false);
    }
  }
}