using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using DataAccessLayer.Dao;
using DataAccessLayer.Models;
using Newtonsoft.Json;

/*
* Ernie Phillips III : 01/27/2022
* Purpose: Handle the view weight entry
* Function: Page accepts an existing weight entry and displays the weight, date, and image if applicable
*/

namespace WeightApp.Fragments {
  public class ViewWeightEntryFragment : AndroidX.Fragment.App.Fragment {

    WeightDao weightDao = new WeightDao();
    Weight weight = new Weight();

    //creation of menu. Set to not display delete button if not incoming record
    public override void OnCreateOptionsMenu(Android.Views.IMenu menu, MenuInflater inflater) {
      inflater.Inflate(Resource.Menu.menu_edit_back, menu);
      base.OnCreateOptionsMenu(menu, inflater);
    }

    //handle the menu click
    public override bool OnOptionsItemSelected(IMenuItem menu) {
      menu.SetChecked(true);
      switch (menu.ItemId) {
        #region BACK BUTTON CLICK
        case Resource.Id.menu_edit:
          WeightEntryFragment weightEntryFragment = new WeightEntryFragment();
          Bundle args = new Bundle();
          args.PutString("HistoryFragmentKey", JsonConvert.SerializeObject(weight.WEIGHT_ID));
          weightEntryFragment.Arguments = args;

          this.FragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, weightEntryFragment, "Fragment").Commit();
          return true;
        #endregion
        #region BACK BUTTON CLICK
        case Resource.Id.menu_back:
          this.FragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, new HistoryFragment(), "Fragment").Commit();
          return true;
          #endregion
      }
      return base.OnOptionsItemSelected(menu);

    }

    public override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);
      //show the options menu
      HasOptionsMenu = true;
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
      View view = inflater.Inflate(Resource.Layout.fragment_view_weight_entry, container, false);

      if (this.Arguments != null) {
        if (this.Arguments.GetString("HistoryFragmentKey") != null) {
          int weightId = JsonConvert.DeserializeObject<int>(this.Arguments.GetString("HistoryFragmentKey"));
          weight = weightDao.GetWeight(weightId);
        }
      }

      if (weight != null) {
        TextView txtWeight = view.FindViewById<TextView>(Resource.Id.veiw_weight_entry_weight);
        TextView txtDate = view.FindViewById<TextView>(Resource.Id.veiw_weight_entry_date);

        string[] weightSplit = weight.WEIGHT_ENTRY.ToString().Split(".");
        txtWeight.Text = "Weight: " + weightSplit[0] + " lbs " + weightSplit[1] + " oz";
        txtDate.Text = "Date: " + weight.DATE_ENTRY.ToShortDateString();

        if (weight.IMAGE != null) {
          ImageView image = view.FindViewById<ImageView>(Resource.Id.veiw_weight_entry_image);
          Bitmap imageBitmap = BitmapFactory.DecodeByteArray(weight.IMAGE, 0, weight.IMAGE.Length);
          image.SetImageBitmap(imageBitmap);
        }
      }

      return view;
    }
  }
}