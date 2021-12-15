using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeightApp.Fragments {
  public class WelcomeFragment : AndroidX.Fragment.App.Fragment {
    public override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);

      // Create your fragment here
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
      View view = inflater.Inflate(Resource.Layout.fragment_welcome, container, false);

      Button btnMyProfile = view.FindViewById<Button>(Resource.Id.btn_go_to_profile);
     
      btnMyProfile.Click += delegate {
        this.FragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, new ProfileFragment(), "Fragment").Commit();
      };

      return view;
    }
  }
}