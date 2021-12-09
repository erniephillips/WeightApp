using Android.OS;
using Android.Views;

/*
* Ernie Phillips III : 12/09/2021
* History page, will pull from sqlite and allow user to modify/delete entries displayed in listview
*/

namespace WeightApp.Fragments {
  public class HistoryFragment : AndroidX.Fragment.App.Fragment {
    public override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);

      // Create your fragment here
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
      // Use this to return your custom view for this Fragment
      return inflater.Inflate(Resource.Layout.fragment_history, container, false);
    }
  }
}