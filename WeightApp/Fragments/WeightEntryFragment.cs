using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
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
using Java.IO;
using Newtonsoft.Json;
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
    ImageButton btnImage;

    //creation of fragment
    public override void OnCreate(Bundle savedInstanceState) {
      base.OnCreate(savedInstanceState);

      //show the options menu
      HasOptionsMenu = true;
    }

    //creation of menu. Set to not display delete button if not incoming record
    public override void OnCreateOptionsMenu(Android.Views.IMenu menu, MenuInflater inflater) {
      if (this.Arguments != null) {
        if (this.Arguments.GetString("HistoryFragmentKey") != null) {
          inflater.Inflate(Resource.Menu.menu_save_delete_back, menu);
        }
      } else {
        inflater.Inflate(Resource.Menu.menu_save_back, menu);
      }

      base.OnCreateOptionsMenu(menu, inflater);
    }

    //handle the menu click
    public override bool OnOptionsItemSelected(IMenuItem menu) {
      menu.SetChecked(true);
      switch (menu.ItemId) {
        #region SAVE BUTTON CLICK
        case Resource.Id.menu_save:

          List<ListviewTextLeftRight> ListviewTextLeftRights = adapter.GetItems();
          string error = "";
          foreach (ListviewTextLeftRight profileItem in ListviewTextLeftRights) {
            if (profileItem.TextRightSide == "N/a") {
              error += profileItem.TextLeftSide + " is required.\n";
            }
          }
          if (error != "") {
            new MaterialAlertDialogBuilder(Activity)
               .SetTitle("Weight App Alert")
               .SetMessage(error)
               .SetPositiveButton("OK", (sender, e) => { })
               .Show();
            return true;
          }

          //no errrors detected, get text
          Weight newWeight = new Weight();

          ImageButton image = this.View.FindViewById<ImageButton>(Resource.Id.we_camera_icon_click);
          foreach (ListviewTextLeftRight weightItem in ListviewTextLeftRights) {
            if (weightItem.TextLeftSide == "Weight")
              newWeight.WEIGHT_ENTRY = weightItem.HiddenTextForConversion;
            if (weightItem.TextLeftSide == "Date")
              newWeight.DATE_ENTRY = DateTime.Parse(weightItem.HiddenTextForConversion);
          }

          //check if image exists
          string imageTag = (string)image.Tag;
          if (imageTag == "CAMERA_IMAGE" || imageTag == "GALLERY_IMAGE") {
            Bitmap bitmap = ((BitmapDrawable)image.Drawable).Bitmap;
            MemoryStream stream = new MemoryStream();
            bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
            byte[] imageInBytes = stream.ToArray();

            newWeight.IMAGE = imageInBytes;
            newWeight.IMAGE_NAME = imageTag + "-" + Guid.NewGuid();
            newWeight.IMAGE_SIZE = imageInBytes.Length;
            newWeight.IMAGE_TYPE = "png";

          }

          if (this.Arguments != null) {
            //update record
            if (this.Arguments.GetString("HistoryFragmentKey") != null) {

              //get current weight entry and set newWeight values
              int weightId = JsonConvert.DeserializeObject<int>(this.Arguments.GetString("HistoryFragmentKey"));
              weight = weightDao.GetWeight(weightId);
              weight.WEIGHT_ENTRY = newWeight.WEIGHT_ENTRY;
              weight.DATE_ENTRY = newWeight.DATE_ENTRY;

              if (imageTag == "CAMERA_IMAGE" || imageTag == "GALLERY_IMAGE") {
                weight.IMAGE = newWeight.IMAGE;
                weight.IMAGE_NAME = newWeight.IMAGE_NAME;
                weight.IMAGE_SIZE = newWeight.IMAGE_SIZE;
                weight.IMAGE_TYPE = newWeight.IMAGE_TYPE;
              } else if (imageTag == "ICON_IMAGE") {
                weight.IMAGE = null;
                weight.IMAGE_NAME = null;
                weight.IMAGE_SIZE = 0;
                weight.IMAGE_TYPE = null;
              }
              //update weight
              weightDao.UpdateWeight(weight);
            }
          }
          //create record
          else {
            //everything validated, save weight
            string userId = pref.GetString("UserId", String.Empty);

            ProfileDao profileDao = new ProfileDao();
            Profile profile = profileDao.GetProfileByUserId(Convert.ToInt32(userId));

            newWeight.PROFILE_ID = profile.PROFILE_ID;

            //add the record
            weightDao.AddWeight(newWeight);


          }

          //record saved message to user
          new MaterialAlertDialogBuilder(Activity)
           .SetTitle("Record successfully saved")
           .SetPositiveButton("OK", (sender, e) => {
             //reload the page
             this.FragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, new WeightEntryFragment(), "Fragment").Commit();
           })
           .Show();
          return true;
        #endregion
        #region DELETE BUTTON CLICK
        case Resource.Id.menu_delete:
          new MaterialAlertDialogBuilder(Activity)
             .SetTitle("Are you sure you wish to delete?")
             .SetNegativeButton("Cancel", (sender, e) => { })
             .SetPositiveButton("OK", (sender, e) => {

               if (this.Arguments != null) {
                 if (this.Arguments.GetString("HistoryFragmentKey") != null) {
                   int weightId = JsonConvert.DeserializeObject<int>(this.Arguments.GetString("HistoryFragmentKey"));
                   weight = weightDao.GetWeight(weightId);
                   weightDao.DeleteWeight(weight);
                 }
               }

               new MaterialAlertDialogBuilder(Activity)
              .SetTitle("Record has been deleted")
              .SetPositiveButton("OK", (sender, e) => {
                //reload the page
                this.FragmentManager.BeginTransaction().Replace(Resource.Id.frame_layout, new WeightEntryFragment(), "Fragment").Commit();
              })
              .Show();
             })
             .Show();

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


    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {

      View view = inflater.Inflate(Resource.Layout.fragment_weight_entry, container, false);

      listView = view.FindViewById<ListView>(Resource.Id.weight_entry_listView);
      btnImage = view.FindViewById<ImageButton>(Resource.Id.we_camera_icon_click);

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
        if (imageTag == "ICON_IMAGE") {
          btnDeletePhoto.Visibility = ViewStates.Gone;
        } else {
          btnDeletePhoto.Visibility = ViewStates.Visible;
          btnDeletePhoto.Click += (s, e) => {
            imagePickerDialog.Dismiss();
            btnImage.Tag = "ICON_IMAGE";
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
            intent.SetAction(Intent.ActionPick);
            //intent.SetAction(Intent.ActionGetContent);

            // Start the picture-picker activity (resumes in MainActivity.cs)
            MainActivity.Instance.StartActivityForResult(Intent.CreateChooser(intent, "Select Picture"), MainActivity.WEIGHT_ENTRY_GALLERY_REQUEST);
          }
        };
      };

      return view;
    }

    private void LoadData() {
      if (this.Arguments != null) {
        if (this.Arguments.GetString("HistoryFragmentKey") != null) {
          int weightId = JsonConvert.DeserializeObject<int>(this.Arguments.GetString("HistoryFragmentKey"));
          weight = weightDao.GetWeight(weightId);
        }
      }

      List<ListviewTextLeftRight> weightEntryItems;

      if (weight == null || weight.PROFILE_ID == 0) {
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

        if (weight.IMAGE != null) {
          Bitmap imageBitmap = BitmapFactory.DecodeByteArray(weight.IMAGE, 0, weight.IMAGE.Length);
          btnImage.SetImageBitmap(imageBitmap);
          btnImage.Tag = "CAMERA_IMAGE";
        }
      }

      adapter = new ListViewTextLeftRightAdapter(this, weightEntryItems);

      // set new items
      listView.Adapter = adapter;
    }
  }
}