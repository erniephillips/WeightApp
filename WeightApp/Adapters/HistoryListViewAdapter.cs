using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using DataAccessLayer.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using WeightApp.Fragments;

/*
* Ernie Phillips III : 01/27/2022
* Purpose: Display weight entry history
* Function: Adapter is passed a list of weights which are displayed in a list view of the calling page
*/

namespace WeightApp.Adapters {
  internal class HistoryListViewAdapter : BaseAdapter {

    //set variables 
    private Fragment fragment;
    private List<Weight> weightList;
    private int selectedId = -1;

    //constructor for calling fragment and list set to variables
    public HistoryListViewAdapter(Fragment fragment, List<Weight> weightList) {
      this.fragment = fragment;
      this.weightList = weightList;
    }

    //set the list count
    public override int Count => weightList.Count;

    //get item by position
    public override Java.Lang.Object GetItem(int position) {
      return position;
    }

    //get item ID by position
    public override long GetItemId(int position) {
      return weightList[position].WEIGHT_ID;
    }

    //this was initially added to highlight a selected list view item, but I opted for use of buttons in listview
    public void SetSelectedId(int position) {
      selectedId = position;
    }

    //get the listview and set XML elements to list object values
    public override View GetView(int position, View convertView, ViewGroup parent) {
      //set the view as passed in converted view or look up listview if empty
      var view = convertView ?? fragment.LayoutInflater.Inflate(Resource.Layout.listview_history, parent, false);

      TextView txtWeight = view.FindViewById<TextView>(Resource.Id.listview_history_weight);
      TextView txtDate = view.FindViewById<TextView>(Resource.Id.listview_history_date);
      ImageButton btnEdit = view.FindViewById<ImageButton>(Resource.Id.listview_history_edit);
      ImageButton btnView = view.FindViewById<ImageButton>(Resource.Id.listview_history_view);
      
      txtWeight.Text = weightList[position].WEIGHT_ENTRY.ToString();
      txtDate.Text = weightList[position].DATE_ENTRY.ToShortDateString();

      //set the button click events for each listview item
      btnEdit.Click += (s, e) => {
        WeightEntryFragment weightEntryFragment = new WeightEntryFragment();
        Bundle args = new Bundle();
        //Causing errror. Payload is too high. Going to try sending id and lookup on child fragment
        //args.PutString("HistoryFragmentKey", JsonConvert.SerializeObject(weightList[position]));
        args.PutString("HistoryFragmentKey", JsonConvert.SerializeObject(weightList[position].WEIGHT_ID));
        weightEntryFragment.Arguments = args;

        //navigate to the weight entry fragment
        ((FragmentActivity)parent.Context).SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, weightEntryFragment, "Fragment").Commit();
      };

      btnView.Click += (s, e) => {
        ViewWeightEntryFragment viewWeightEntryFragment = new ViewWeightEntryFragment();
        Bundle args = new Bundle(); //send arguments to fragment for check
        args.PutString("HistoryFragmentKey", JsonConvert.SerializeObject(weightList[position].WEIGHT_ID));
        viewWeightEntryFragment.Arguments = args;

        //naviage to view weight entry fragment
        ((FragmentActivity)parent.Context).SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, viewWeightEntryFragment, "Fragment").Commit();
      };

      //here is where the clicked listview item would be highlighted if it were being used
      if (selectedId == position) {
        view.SetBackgroundColor(Color.LightGray);
      } else {
        view.SetBackgroundColor(Color.Transparent);
      }

      return view;
    }


  }
}