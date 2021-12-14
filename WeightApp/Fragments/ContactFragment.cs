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

    // send email using native phone app
    //https://docs.microsoft.com/en-us/xamarin/essentials/email?tabs=android

    //why I shouldn't use SMTP in the app (should build web service instead)
    //https://social.msdn.microsoft.com/Forums/en-US/f0f661d2-24db-47b1-9fc7-6be02685f8f1/how-to-send-smtp-email-using-xamarinforms-without-user-interaction?forum=xamarinforms
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
      // Use this to return your custom view for this Fragment
      return inflater.Inflate(Resource.Layout.fragment_contact, container, false);
    }
  }
}