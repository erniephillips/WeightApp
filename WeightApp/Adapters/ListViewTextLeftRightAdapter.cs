using Android.Views;
using Android.Widget;
using DataAccessLayer.Models;
using System.Collections.Generic;


/*
* Ernie Phillips III : 01/27/2022
* Purpose: Display profile and weight entry page listview
* Function: Adapter is passed a list of a custom object for displaying left & right side text that user can click which loads a modal
*/


namespace WeightApp.Adapters {
  internal class ListViewTextLeftRightAdapter : BaseAdapter {

    private AndroidX.Fragment.App.Fragment fragment;
    List<ListviewTextLeftRight> items;
    private int selectedId = -1;

    public ListViewTextLeftRightAdapter(AndroidX.Fragment.App.Fragment fragment, List<ListviewTextLeftRight> items) {
      this.fragment = fragment;
      this.items = items;
    }

    public override int Count => items.Count;

    public override Java.Lang.Object GetItem(int position) {
      return position;
    }

    public override long GetItemId(int position) {
      return items[position].Id;
    }

    //experimenting if I can set the object text. Doesn't work for me to set text for listview item and maintain right and left alignment
    public void SetSelectedTextValue(int position, string value, string textToSave) {
      View view = this.fragment.View;
      items[position].HiddenTextForConversion = textToSave;
      items[position].TextRightSide = value;
      view.FindViewById<TextView>(Resource.Id.listview_item_right).Text = items[position].TextRightSide;
      NotifyDataSetChanged(); //had to add this to notify observer of change in data. Only first value was updating
    }

    //return the profile items for validation
    public List<ListviewTextLeftRight> GetItems() {
      return items;
    }

    public override View GetView(int position, View convertView, ViewGroup parent) {
      var view = convertView ?? fragment.LayoutInflater.Inflate(Resource.Layout.listview_left_right_text, parent, false);

      view.FindViewById<TextView>(Resource.Id.listview_item_left).Text = items[position].TextLeftSide;
      view.FindViewById<TextView>(Resource.Id.listview_item_right).Text = items[position].TextRightSide;

      return view;
    }
  }
}