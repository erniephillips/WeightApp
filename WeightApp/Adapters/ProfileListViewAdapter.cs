using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using DataAccessLayer.Models;
using System.Collections.Generic;

/*
* Ernie Phillips III : 01/27/2022
* Purpose: (NOT USED IN APP FUNCTIONALITY) Was a test Dummy adapter to prove use of adapter to display a list view early on in development. Might develop into an admin piece at a later phase.
*/
//(NOT CURRENTLY USED BY APPLICATION, MAY USE AT A LATER TIME)
namespace WeightApp.Adapters {
  public class ProfileListViewAdapter : BaseAdapter {

    //set variables 
    private Activity activity;
    private List<Profile> profiles;
    private int selectedId = -1;

    //constructor for calling fragment and list set to variables
    public ProfileListViewAdapter(Activity activity, List<Profile> profiles) {
      this.activity = activity;
      this.profiles = profiles;
    }

    //set the list count
    public override int Count => profiles.Count;

    //get item by position
    public override Java.Lang.Object GetItem(int position) {
      return null;
    }

    //get item ID by position
    public override long GetItemId(int position) {
      return profiles[position].PROFILE_ID;
    }


    public void SetSelectedId(int position) {
      selectedId = position;
    }

    //return the profile items for validation
    public List<Profile> GetItems() {
      return profiles;
    }

    public Profile GetProfileInfo(int position) {
      return profiles[position];
    }


    //get the listview and set XML elements to list object values
    public override View GetView(int position, View convertView, ViewGroup parent) {
      var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.listview_select_profile, parent, false);

      //get the list index
      TextView profileName = view.FindViewById<TextView>(Resource.Id.txtView_profile_name);

      
      profileName.Text = profiles[position].NAME.ToString();

      profileName.TextSize = 22;
      profileName.Typeface = Typeface.Default;

      var lastIndex = profiles.Count - 1;
      if (position == lastIndex) {
        profileName.TextSize = 26;
        profileName.Typeface = Typeface.DefaultBold;
      }
      


      if (selectedId == position) {
        view.SetBackgroundColor(Color.LightGray);
      } else {
        view.SetBackgroundColor(Color.Transparent);
      }

      return view;
    }
  }
}