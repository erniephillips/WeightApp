using Android;
using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Hardware;
using Android.OS;
using Android.Provider;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using DataAccessLayer.Dao;
using DataAccessLayer.Models;
using Google.Android.Material.Dialog;
using Google.Android.Material.Snackbar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WeightApp.Adapters;

/*
* Ernie Phillips III : 12/09/2021
* Weight entry page for adding records, will store to sqlite
*/

namespace WeightApp.Fragments {
  public class WeightEntryFragment : AndroidX.Fragment.App.Fragment {

    ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
    WeightDao weightDao = new WeightDao();
    ListViewTextLeftRightAdapter adapter;
    Weight weight = new Weight();
    ListView listView;

    public override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);

      //show the options menu
      HasOptionsMenu = true;
    }

    public override void OnCreateOptionsMenu(Android.Views.IMenu menu, MenuInflater inflater) {
      //MenuInflater.Inflate (Resource.Menu.Action_menu, menu);
      //menu.Clear();
      inflater.Inflate(Resource.Menu.menu_main, menu);

      base.OnCreateOptionsMenu(menu, inflater);
    }

    public override bool OnOptionsItemSelected(IMenuItem menu) {
      menu.SetChecked(true);
      //switch (menu.ItemId) {
      //  case Resource.Id.selecta:
      //    Toast.MakeText(Application.Context, "Top", ToastLength.Long);

      //    return true;
      //  case Resource.Id.selectb:
      //    Toast.MakeText(Application.Context, "New", ToastLength.Long);

      //    return true;
      //}
      return base.OnOptionsItemSelected(menu);

    }
    

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
      View view = inflater.Inflate(Resource.Layout.fragment_weight_entry, container, false);

      listView = view.FindViewById<ListView>(Resource.Id.weight_entry_listView);
      ImageButton btnImage = view.FindViewById<ImageButton>(Resource.Id.we_camera_icon_click);

      LoadData(); //clear any position index

      //set the listview item click
      listView.ItemClick += (s, eLV) => {
        //setting up a swith for the position selected to pull up a dialog box for user to make a selection depending
        switch (eLV.Position) {
          #region WEIGHT OPTION
          case 0:
            View weightView = inflater.Inflate(Resource.Layout.dialog_spinner, container, false);

            //Number picker: https://medium.com/@sc71/android-numberpickers-3ef535c45487

            NumberPicker pckWeightPoundsNum = weightView.FindViewById<NumberPicker>(Resource.Id.dialog_spinner_number_picker_one);
            NumberPicker pckWeightOzNum = weightView.FindViewById<NumberPicker>(Resource.Id.dialog_spinner_number_picker_two);

            TextView txtWeightTextOne = weightView.FindViewById<TextView>(Resource.Id.dialog_spinner_text_one);
            TextView txtWeightTextTwo = weightView.FindViewById<TextView>(Resource.Id.dialog_spinner_text_two);
            txtWeightTextOne.Text = "lbs";
            txtWeightTextTwo.Text = "oz";

            //set the whole weight number
            string[] weightPoundNumbers = Enumerable.Range(1, 400).Select(x => x.ToString()).ToArray(); //create an array to 400 lbs
            pckWeightPoundsNum.MinValue = 1;
            pckWeightPoundsNum.MaxValue = weightPoundNumbers.Length;
            pckWeightPoundsNum.Value = 150; //set the start value
            pckWeightPoundsNum.SetDisplayedValues(weightPoundNumbers);

            //set the whole weight number
            string[] weightOzNumbers = Enumerable.Range(0, 17).Select(x => x.ToString()).ToArray(); //create an array to 400 lbs
            pckWeightOzNum.MinValue = 1;
            pckWeightOzNum.MaxValue = weightOzNumbers.Length - 1;
            pckWeightOzNum.Value = 1; //set the start value
            pckWeightOzNum.SetDisplayedValues(weightOzNumbers);

            new MaterialAlertDialogBuilder(Activity).SetView(weightView)
              .SetTitle("What's your current weight?")
              .SetNegativeButton("Cancel", (s, e) => { })
              .SetPositiveButton("OK", (sender, e) => {

                var selectedLbs = pckWeightPoundsNum.Value;
                var selectedOz = pckWeightOzNum.Value - 1;

                adapter.SetSelectedTextValue(
                  eLV.Position,
                  selectedLbs + " lbs " + selectedOz + " oz",
                  selectedLbs + "." + selectedOz);
              })
              .Show();
            break;
          #endregion
          #region DATE OPTION
          case 1:
            DatePickerDialog datePicker = new DatePickerDialog(Context);
            datePicker.SetButton((int)DialogButtonType.Positive, Context.Resources.GetString(global::Android.Resource.String.Ok), (s, e) => {
              DateTime selectedDate = datePicker.DatePicker.DateTime;
              adapter.SetSelectedTextValue(eLV.Position, selectedDate.ToShortDateString(), selectedDate.ToString());
            });
            datePicker.Show();

            break;
            #endregion
        }
      };

      btnImage.Click += (s, e) => {
        View photoView = inflater.Inflate(Resource.Layout.dialog_photo, container, false);
        Button btnTakePhoto = photoView.FindViewById<Button>(Resource.Id.dp_btn_take_photo);
        Button btnUploadPhoto = photoView.FindViewById<Button>(Resource.Id.dp_btn_gallery);
        Button btnDeletePhoto = photoView.FindViewById<Button>(Resource.Id.dp_btn_delete_photo);


        AndroidX.AppCompat.App.AlertDialog imagePickerDialog = 
          new MaterialAlertDialogBuilder(Activity).SetTitle("Add a Progress Photo").SetView(photoView).Create();
        imagePickerDialog.Show();

        //if image icon click not set to camera
        string imageTag = (string)btnImage.Tag;
        if(imageTag == "IMAGE_ICON") {
          btnDeletePhoto.Visibility = ViewStates.Gone;
        } else {
          btnDeletePhoto.Visibility = ViewStates.Visible;
          btnDeletePhoto.Click += (s, e) => {
            imagePickerDialog.Dismiss();
            btnImage.Tag = "IMAGE_ICON";
            btnImage.SetImageResource(Android.Resource.Drawable.IcMenuCamera);
          };
        }

        

        btnTakePhoto.Click += (s, e) => {
          imagePickerDialog.Dismiss();
          btnImage.Tag = "CAMERA_IMAGE";
          //check that user has granted camera permissions
          if (!ActivityCompat.ShouldShowRequestPermissionRationale(Activity, Manifest.Permission.Camera)) {
            //user has accepted camera permissions, start camera
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            MainActivity.Instance.StartActivityForResult(intent, MainActivity.WEIGHT_ENTRY_CAMERA_REQUEST);
          }
        };
        btnUploadPhoto.Click += (s, e) => {
          imagePickerDialog.Dismiss();
          btnImage.Tag = "GALLERY_IMAGE";
          //check that user has granted camera permissions
          if (!ActivityCompat.ShouldShowRequestPermissionRationale(Activity, Manifest.Permission.ReadExternalStorage)) {
            //user has accepted upload image permissions, start image picker
            //https://www.c-sharpcorner.com/article/xamarin-android-how-to-pick-a-image-from-gallery-in-android-phone-using-visual/
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);

            // Start the picture-picker activity (resumes in MainActivity.cs)
            MainActivity.Instance.StartActivityForResult(Intent.CreateChooser(intent, "Select Picture"), MainActivity.WEIGHT_ENTRY_GALLERY_REQUEST);
          }
        };
      };

      return view;
    }
   
    private void LoadData() {
      string userId = pref.GetString("UserId", String.Empty);

      //need a flag to check if user is modifying a record which will be built in future history page
      //will also need to add a logic dependent delete button
      //pass intent but for fragment?
      //Intent intent = new Intent(this, typeof(SecurityQuestionActivity));
      //intent.PutExtra("User", JsonConvert.SerializeObject(user));
      //StartActivity(intent);
      //Finish();
      //weight = weightDao.GetWeight(get intent weight entry id);

      List<ListviewTextLeftRight> weightEntryItems;
      weight = null;

      if (weight == null) {
        weightEntryItems = new List<ListviewTextLeftRight>() {
          new ListviewTextLeftRight{ Id = 1, TextLeftSide = "Weight", TextRightSide = "N/a" },
          new ListviewTextLeftRight{ Id = 2, TextLeftSide = "Date", TextRightSide = "N/a" }
        };
      } else {
        string[] weightSplit = weight.WEIGHT_ENTRY.ToString().Split(".");
        weightEntryItems = new List<ListviewTextLeftRight>() {
          new ListviewTextLeftRight{
            Id = 1, TextLeftSide = "Weight",
            TextRightSide = weightSplit[0] + " lbs " + weightSplit[1] + " oz",
            HiddenTextForConversion = weight.WEIGHT_ENTRY.ToString() },
          new ListviewTextLeftRight{
            Id = 2, TextLeftSide = "Date",
            TextRightSide = weight.DATE_ENTRY.ToShortDateString(),
            HiddenTextForConversion = weight.DATE_ENTRY.ToShortDateString() }
        };
      }

      adapter = new ListViewTextLeftRightAdapter(this, weightEntryItems);

      // set new items
      listView.Adapter = adapter;
    }
  }
}