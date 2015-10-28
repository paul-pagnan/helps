using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using helps.Shared;
using helps.Shared.DataObjects;
using helps.Droid.Helpers;
using helps.Droid.Adapters;
using Android.Support.V4.Widget;
using System.Threading.Tasks;
using System.Threading;
using Android.Graphics;
using Java.Lang;
using helps.Droid.Adapters.DataObjects;

namespace helps.Droid
{
    [Activity(Label = "Settings", Theme = "@style/AppTheme.MyToolbar")]
    public class SettingsActivity : Main
    {
        private ListView settingsListView;
        private SettingsCategoryListAdapter settingsListAdapter;

        protected override int LayoutResource
        {
            get { return Resource.Layout.Activity_Settings; }
        }
        public void UpdateMyInformation()
        {
            var intent = new Intent(this, typeof(DetailsInputActivity));
            intent.PutExtra("Skip", true);
            StartActivity(intent);
        }
        
        private void InitLists()
        {
            settingsListAdapter = new SettingsCategoryListAdapter(this.LayoutInflater, Resources);
            settingsListView = FindViewById<ListView>(Resource.Id.settingsList);
            settingsListView.Adapter = settingsListAdapter;
            var list = new List<MyList>();
            list.Add(new MyList() { Id = 1, title = "Notifications Enabled", hideArrow = true });
            list.Add(new MyList() { Id = 2, title = "Update My Information" });
            list.Add(new MyList() { Id = 3, title = "Font Size" });
            list.Add(new MyList() { Id = 4, title = "Change Skin" });
            list.Add(new MyList() { Id = 5, title = "Logout", hideArrow = true });
            settingsListAdapter.AddAllSettings(list);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetDisplayShowHomeEnabled(true);
            
            InitLists();

            settingsListView.ItemClick += (sender, e) =>           
            {
                switch (e.Id)
                {
                    case 2:
                        UpdateMyInformation();
                        break;
                    case 3:
                        FontSize();
                        break;
                    case 4:
                        //Change Skin
                        break;
                    case 5:
                        Logout();
                        break;
                    default:
                        break;
                }
            };            
        }
        public void FontSize()
        {
            var notifier = new NotificationHelper();
            notifier.ShowFontSize(this);
        }

        [Java.Interop.Export()]
        public void enableNotifications(View view)
        {
            SettingService.toggleNotifications(FindViewById<CheckBox>(Resource.Id.notificationsEnabled).Checked);
        }

        public void Logout()
        {
            Services.Auth.Logout();
            CurrentUser = null;
            var intent = new Intent(this, typeof(SignInActivity));
            intent.PutExtra("logout", true);
            StartActivity(intent);
            Finish();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return base.OnOptionsItemSelected(item);
        }
    }
}