﻿using Android.OS;
using Android.Views;
using Android.Widget;
using DataAccessLayer.Dao;
using DataAccessLayer.Models;
using System.Collections.Generic;
using WeightApp.Adapters;

/*
* Ernie Phillips III : 12/09/2021
* History page, will pull from sqlite and allow user to modify/delete entries displayed in listview
*/

namespace WeightApp.Fragments {
  public class HistoryFragment : AndroidX.Fragment.App.Fragment {

    ListView listView;
    List<Weight> weightList = new List<Weight>();
    WeightDao weightDao = new WeightDao();

    public override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);

      // Create your fragment here
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
      // Use this to return your custom view for this Fragment
      View view = inflater.Inflate(Resource.Layout.fragment_history, container, false);

      listView = view.FindViewById<ListView>(Resource.Id.history_listView);

      LoadData(-1); //clear any position index

      return view;
    }

    private void LoadData(int position) {

      weightList = weightDao.GetWeights();
      HistoryListViewAdapter adapter = new HistoryListViewAdapter(this, weightList);
      adapter.SetSelectedId(position);

      // Save the ListView state (= includes scroll position) as a Parceble
      //IParcelable state = listView.OnSaveInstanceState();

      // set new items
      listView.Adapter = adapter;

      // Restore previous state (including selected item index and scroll position)
      //listView.OnRestoreInstanceState(state);

    }
  }
}