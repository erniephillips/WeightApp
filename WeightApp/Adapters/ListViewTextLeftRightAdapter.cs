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

    //set variables 
    private AndroidX.Fragment.App.Fragment fragment;
    List<ListviewTextLeftRight> items;
    private int selectedId = -1;

    //constructor for calling fragment and list set to variables
    public ListViewTextLeftRightAdapter(AndroidX.Fragment.App.Fragment fragment, List<ListviewTextLeftRight> items) {
      this.fragment = fragment;
      this.items = items;
    }

    //set the list count
    public override int Count => items.Count;

    //get item by position
    public override Java.Lang.Object GetItem(int position) {
      return position;
    }

    //get item ID by position
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

    //get the listview and set XML elements to list object values
    public override View GetView(int position, View convertView, ViewGroup parent) {
      var view = convertView ?? fragment.LayoutInflater.Inflate(Resource.Layout.listview_left_right_text, parent, false);

      view.FindViewById<TextView>(Resource.Id.listview_item_left).Text = items[position].TextLeftSide;
      view.FindViewById<TextView>(Resource.Id.listview_item_right).Text = items[position].TextRightSide;

      return view;
    }
  }
}