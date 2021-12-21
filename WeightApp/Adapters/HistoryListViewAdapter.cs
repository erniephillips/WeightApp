using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using DataAccessLayer.Models;
using Google.Android.Material.Dialog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeightApp.Fragments;

namespace WeightApp.Adapters {
  internal class HistoryListViewAdapter : BaseAdapter {

    Context context;
    private AndroidX.Fragment.App.Fragment fragment;
    private List<Weight> weightList;
    private int selectedId = -1;

    public HistoryListViewAdapter(AndroidX.Fragment.App.Fragment fragment, List<Weight> weightList) {
      this.fragment = fragment;
      this.weightList = weightList;
    }

    public override int Count => weightList.Count;

    public override Java.Lang.Object GetItem(int position) {
      return position;
    }

    public override long GetItemId(int position) {
      return weightList[position].WEIGHT_ID;
    }

    public void SetSelectedId(int position) {
      selectedId = position;
    }

    public override View GetView(int position, View convertView, ViewGroup parent) {
      var view = convertView ?? fragment.LayoutInflater.Inflate(Resource.Layout.listview_history, parent, false);

      TextView txtWeight = view.FindViewById<TextView>(Resource.Id.listview_history_weight);
      TextView txtDate = view.FindViewById<TextView>(Resource.Id.listview_history_date);
      ImageButton btnEdit = view.FindViewById<ImageButton>(Resource.Id.listview_history_edit);
      ImageButton btnView = view.FindViewById<ImageButton>(Resource.Id.listview_history_view);
      
      txtWeight.Text = weightList[position].WEIGHT_ENTRY.ToString();
      txtDate.Text = weightList[position].DATE_ENTRY.ToShortDateString();

      btnEdit.Click += (s, e) => {
        WeightEntryFragment weightEntryFragment = new WeightEntryFragment();
        Bundle args = new Bundle();
        //Causing errror. Payload is too high. Going to try sending id and lookup on child fragment
        //args.PutString("HistoryFragmentKey", JsonConvert.SerializeObject(weightList[position]));
        args.PutString("HistoryFragmentKey", JsonConvert.SerializeObject(weightList[position].WEIGHT_ID));
        weightEntryFragment.Arguments = args;

        
        ((FragmentActivity)parent.Context).SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, weightEntryFragment, "Fragment").Commit();
      };

      btnView.Click += (s, e) => {
        ViewWeightEntryFragment viewWeightEntryFragment = new ViewWeightEntryFragment();
        Bundle args = new Bundle();
        args.PutString("HistoryFragmentKey", JsonConvert.SerializeObject(weightList[position].WEIGHT_ID));
        viewWeightEntryFragment.Arguments = args;

        ((FragmentActivity)parent.Context).SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, viewWeightEntryFragment, "Fragment").Commit();
      };

      if (selectedId == position) {
        view.SetBackgroundColor(Color.LightGray);
      } else {
        view.SetBackgroundColor(Color.Transparent);
      }

      return view;
    }


  }
}