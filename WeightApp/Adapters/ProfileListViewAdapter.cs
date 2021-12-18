using Android.Views;
using Android.Widget;
using DataAccessLayer.Models;
using System.Collections.Generic;

namespace WeightApp.Adapters {
  internal class ProfileListViewAdapter : BaseAdapter {

    private AndroidX.Fragment.App.Fragment fragment;
    List<ProfileListview> profileItems;
    private int selectedId = -1;

    public ProfileListViewAdapter(AndroidX.Fragment.App.Fragment fragment, List<ProfileListview> profileItems) {
      this.fragment = fragment;
      this.profileItems = profileItems;
    }

    public override int Count => profileItems.Count;

    public override Java.Lang.Object GetItem(int position) {
      return position;
    }

    public override long GetItemId(int position) {
      return profileItems[position].Id;
    }

    //experimenting if I can set the object text. Doesn't work for me to set text for listview item and maintain right and left alignment
    public void SetSelectedTextValue(int position, string value) {
      View view = this.fragment.View;
      profileItems[position].TextRightSide = value;
      view.FindViewById<TextView>(Resource.Id.profile_listview_item_right).Text = profileItems[position].TextRightSide;
    }

    //return the profile items for validation
    public List<ProfileListview> ValidateProfile() {
      return profileItems;
    }

    public override View GetView(int position, View convertView, ViewGroup parent) {
      var view = convertView ?? fragment.LayoutInflater.Inflate(Resource.Layout.listview_profile, parent, false);

      view.FindViewById<TextView>(Resource.Id.profile_listview_item_left).Text = profileItems[position].TextLeftSide;
      view.FindViewById<TextView>(Resource.Id.profile_listview_item_right).Text = profileItems[position].TextRightSide;

      return view;
    }
  }
}