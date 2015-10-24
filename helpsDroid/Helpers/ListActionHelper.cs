using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Android.App;
using helps.Droid.Adapters;
using helps.Shared.DataObjects;
using Android.Content;
using Android.Content.Res;
using helps.Droid.Adapters.DataObjects;
using Xamarin.Forms;
using ListView = Android.Widget.ListView;
using Android.Views;
using Android.Widget;

namespace helps.Droid.Helpers
{
    public class ListActionHelper
    {
        public BookingsListAdapter listAdapter;
        public ListView listView;
        private Context ctx;
        public List<MyList> items;
        private int selected;
        private bool isWorkshop;
        private AlertDialog filterDialog;
        public static Android.Views.View filterView;

        private static string name;
        private static string location;
        private static DateTime startDate;
        private static DateTime endDate;

        private const string dateFormat = "ddd, dd MMM yyyy";
        private const string timeFormat = "h:mm tt";
        private static DateTime now;

        public ListActionHelper(BookingsListAdapter listAdapter, ListView listView, Context ctx, bool isWorkshop)
        {
            this.listAdapter = listAdapter;
            this.listView = listView;
            this.ctx = ctx;
            this.isWorkshop = isWorkshop;
            InitOptions();
        }

        private void InitOptions()
        {
            items = new List<MyList>();
            items.Add(new MyList() {Id = 0, title = "Date/Time"});
            items.Add(new MyList() {Id = 1, title = "Location" });
            var title = isWorkshop ? "Workshop" : "Tutor";
            items.Add(new MyList() {Id = 2, title = title + " Name" });
            items.Add(new MyList() {Id = 3, title = "Type" });
            items.Add(new MyList() {Id = 4, title = "Workshop Category" });
        }

        public void Sort(List<WorkshopPreview> list)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(ctx);
            builder.SetTitle("Sort By");
            builder.SetSingleChoiceItems(items.Select(x => x.title).ToArray(), selected, new MultiClickListener(this));
            var clickListener = new ActionClickListener(listAdapter, listView, list, this, true, isWorkshop);
            builder.SetPositiveButton("Sort", clickListener);
            builder.SetNegativeButton("Cancel", clickListener);
            builder.Create().Show();
        }
  
        public void Filter(List<WorkshopPreview> list, LayoutInflater inflater, Activity activity)
        {
            var builder = new AlertDialog.Builder(activity);
            var dialogView = inflater.Inflate(Resource.Layout.AlertDialog_Filter, null);
            builder.SetView(dialogView);
            builder.SetTitle("Filter");
            var clickListener = new ActionClickListener(listAdapter, listView, list, this, false, isWorkshop, dialogView);
            builder.SetPositiveButton("Filter", clickListener);
            builder.SetNegativeButton("Clear", clickListener);
            SetDefaultVals(dialogView, isWorkshop);
            filterView = dialogView;
            builder.Show();
        }

        private static void SetDefaultVals(Android.Views.View dialogView, bool isWorkshop)
        {
            now = DateTime.Now;
            var currentStartDate = (startDate > DateTime.MinValue) ? startDate : now.AddMinutes(now.Minute * -1);
            var currentEndDate = (endDate > DateTime.MinValue) ? endDate : now.AddDays(7).AddMinutes(now.Minute * -1);

            dialogView.FindViewById<TextView>(Resource.Id.startDate).Text = currentStartDate.ToString(dateFormat);
            dialogView.FindViewById<TextView>(Resource.Id.endDate).Text = currentEndDate.ToString(dateFormat);
            dialogView.FindViewById<TextView>(Resource.Id.startTime).Text = currentStartDate.ToString(timeFormat);
            dialogView.FindViewById<TextView>(Resource.Id.endTime).Text = currentEndDate.ToString(timeFormat);
            dialogView.FindViewById<EditText>(Resource.Id.txtFilter_name).Text = name;
            dialogView.FindViewById<EditText>(Resource.Id.txtFilter_name).Hint = (isWorkshop) ? "Workshop Name" : "Tutor Name";
            dialogView.FindViewById<EditText>(Resource.Id.txtFilter_location).Text = location;
        }

        private class MultiClickListener : Java.Lang.Object, IDialogInterfaceOnClickListener
        {
            public ListActionHelper actionHelper;

            public MultiClickListener(ListActionHelper actionHelper)
            {
                this.actionHelper = actionHelper;
            }

            public void OnClick(IDialogInterface dialog, int which)
            {
                actionHelper.selected = which;
            }
        }

        private class ActionClickListener : Java.Lang.Object, IDialogInterfaceOnClickListener
        {
            private BookingsListAdapter listAdapter;
            private ListView listView;
            private List<WorkshopPreview> list;
            private ListActionHelper actionHelper;
            private bool sorting;
            private bool isWorkshop;
            private Android.Views.View dialogView;

            public ActionClickListener(BookingsListAdapter listAdapter, ListView listView, List<WorkshopPreview> list, ListActionHelper actionHelper, bool sorting, bool isWorkshop, Android.Views.View dialogView = null) : base()
            {
                this.listAdapter = listAdapter;
                this.listView = listView;
                this.list = list;
                this.actionHelper = actionHelper;
                this.sorting = sorting;
                this.isWorkshop = isWorkshop;
                this.dialogView = dialogView;
            }

            public void OnClick(IDialogInterface dialog, int which)
            {
                if (list.Count > 0)
                {
                    if (which == -1)
                    {
                        var adaptedList = (sorting) ? GetSortedList(list) : GetFilteredList(list);
                        listAdapter.AddAll(adaptedList);
                        listAdapter.NotifyDataSetChanged();
                    }
                    else if (!sorting)
                    {
                        name = null;
                        location = null;
                        startDate = DateTime.MinValue;
                        endDate = DateTime.MinValue;
                        SetDefaultVals(dialogView, isWorkshop);
                        listAdapter.AddAll(list);
                        listAdapter.NotifyDataSetChanged();
                    }

                }
                else
                    actionHelper.selected = 0;
            }

            private List<WorkshopPreview> GetFilteredList(List<WorkshopPreview> list)
            {
                name = dialogView.FindViewById<EditText>(Resource.Id.txtFilter_name).Text;
                location = dialogView.FindViewById<EditText>(Resource.Id.txtFilter_location).Text;
                startDate = ParseDate(dialogView.FindViewById<TextView>(Resource.Id.startDate).Text,
                    dialogView.FindViewById<TextView>(Resource.Id.startTime).Text);
                endDate = ParseDate(dialogView.FindViewById<TextView>(Resource.Id.endDate).Text,
                    dialogView.FindViewById<TextView>(Resource.Id.endTime).Text);

                if (name != "")
                    list = list.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
                if (location != "")
                    list = list.Where(x => x.Location.ToLower().Contains(location.ToLower())).ToList();


                var start = now.AddMinutes(now.Minute*-1);
                var end = now.AddDays(7).AddMinutes(now.Minute*-1);
                var dateChanged = !CompareDate(start, startDate) || !CompareDate(end, endDate);

                if (dateChanged)
                    list = list.Where(x => x.Date > startDate && x.Date < endDate).ToList();
                return list;
            }

            private bool CompareDate(DateTime start, DateTime startDate)
            {
                return start.Year == startDate.Year && start.Month == startDate.Month && start.Day == startDate.Day &&
                       start.Hour == startDate.Hour && start.Minute == startDate.Minute;

            }

            private DateTime ParseDate(string date, string time)
            {
                return DateTime.ParseExact(date + " " + time, "ddd, dd MMM yyyy h:mm tt", CultureInfo.InvariantCulture);
            }

            private List<WorkshopPreview> GetSortedList(List<WorkshopPreview> list)
            {
                switch (actionHelper.selected)
                {
                    case 0:
                        return list.OrderByDescending(x => x.Date).ToList();
                    case 1:
                        return list.OrderBy(x => x.Location).ToList();
                    case 2:
                        return list.OrderBy(x => x.Name).ToList();
                    case 3:
                        return list.OrderBy(x => x.Type).ToList();
                    case 4:
                        return list.OrderBy(x => x.WorkshopSetId).ToList();
                    default:
                        return list;
                }
            }
        }

    }
}