using Android.Graphics;
using Android.Views;
using Android.Widget;
using DataAccessLayer.Models;
using System.Collections.Generic;

namespace WeightApp.Helpers {
  public class UserListViewAdapter : BaseAdapter {

    private AndroidX.Fragment.App.Fragment fragment;
    private List<User> users;
    private int selectedId = -1;

    public UserListViewAdapter(AndroidX.Fragment.App.Fragment fragment, List<User> users) {
      this.fragment = fragment;
      this.users = users;
    }

    public override int Count => users.Count;

    public override Java.Lang.Object GetItem(int position) {
      return null;
    }

    public override long GetItemId(int position) {
      return users[position].USER_ID;
    }

    public void SetSelectedId(int position) {
      selectedId = position;
    }

    public static void TestMethod() { }

    public override View GetView(int position, View convertView, ViewGroup parent) {
      var view = convertView ?? fragment.LayoutInflater.Inflate(Resource.Layout.list_view_user, parent, false);
      TextView txtUserID = view.FindViewById<TextView>(Resource.Id.txtView_UserID);
      TextView txtUserName = view.FindViewById<TextView>(Resource.Id.txtView_UserName);
      TextView txtPassword = view.FindViewById<TextView>(Resource.Id.txtView_Password);
      TextView txtName = view.FindViewById<TextView>(Resource.Id.txtView_Name);
      TextView txtIsLocked = view.FindViewById<TextView>(Resource.Id.txtView_IsLocked);
      TextView txtFailedLoginAttempts = view.FindViewById<TextView>(Resource.Id.txtView_FailedLoginAttemptCount);
      TextView txtCreatedDate = view.FindViewById<TextView>(Resource.Id.txtView_CreatedDate);
      TextView txtLastLoginDate = view.FindViewById<TextView>(Resource.Id.txtView_LastLoginDate);

      txtUserID.Text = users[position].USER_ID.ToString();
      txtUserName.Text = users[position].USERNAME;
      txtPassword.Text = users[position].PASSWORD;
      txtName.Text = users[position].NAME;
      txtIsLocked.Text = users[position].IS_LOCKED.ToString();
      txtFailedLoginAttempts.Text = users[position].FAILED_LOGIN_ATTEMPT.ToString();
      txtCreatedDate.Text = users[position].CREATED_DATE.ToString();
      txtLastLoginDate.Text = users[position].LAST_LOGIN_DATE.ToString();

      if (selectedId == position) {
        view.SetBackgroundColor(Color.LightGray);
      } else {
        view.SetBackgroundColor(Color.Transparent);
      }

      return view;
    }
  }
}