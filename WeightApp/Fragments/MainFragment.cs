using Android.OS;
using Android.Views;

namespace WeightApp.Fragments
{
  /*
   * Ernie Phillips III : 12/09/2021
   * Main fragment to display when user first opens app
   */

  public class MainFragment : AndroidX.Fragment.App.Fragment
  {
    public override void OnCreate(Bundle savedInstanceState)
    {
      base.OnCreate(savedInstanceState);

      // Create your fragment here
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
      // Use this to return your custom view for this Fragment
      return inflater.Inflate(Resource.Layout.fragment_main, container, false);
    }
  }
}