using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using helps.Droid.Adapters;
using helps.Shared.DataObjects;
using Android.Content;
using helps.Droid.Adapters.DataObjects;
using Xamarin.Forms;
using ListView = Android.Widget.ListView;

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
        }

        public void Sort(List<WorkshopPreview> list)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(ctx);
            builder.SetTitle("Sort By");
            builder.SetSingleChoiceItems(items.Select(x => x.title).ToArray(), selected, new MultiClickListener(this));
            var clickListener = new ActionClickListener(listAdapter, listView, list, this);
            builder.SetPositiveButton("Sort", clickListener);
            builder.SetNegativeButton("Cancel", clickListener);
            builder.Create().Show();
        }

        public void Filter()
        {
            throw new NotImplementedException();
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

            public ActionClickListener(BookingsListAdapter listAdapter, ListView listView, List<WorkshopPreview> list, ListActionHelper actionHelper) : base()
            {
                this.listAdapter = listAdapter;
                this.listView = listView;
                this.list = list;
                this.actionHelper = actionHelper;
            }

            public void OnClick(IDialogInterface dialog, int which)
            {
                if (list.Count > 0)
                {
                    if (which == -1)
                    {
                        var sortedList = GetSortedList(list);
                        listAdapter.AddAll(sortedList);
                        listAdapter.NotifyDataSetChanged();
                    }
                }
                else
                    actionHelper.selected = 0;
            }

            private List<WorkshopPreview> GetSortedList(List<WorkshopPreview> list)
            {
                var a = list.OrderByDescending(x => x.Date).ToList();
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
                    default:
                        return list;
                }
            }
        }

    }
}