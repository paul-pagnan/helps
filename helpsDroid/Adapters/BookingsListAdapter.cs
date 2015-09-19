using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Provider;
using Android.Views;
using Android.Widget;
using helps.Droid.Adapters.DataObjects;
using Android.Graphics;

namespace helps.Droid.Adapters
{
    public class BookingsListAdapter : SessionListBaseAdapter
    { 
        public BookingsListAdapter(LayoutInflater inflater) : base(inflater)
        {
            PopulateList();
        }

        public void PopulateList()
        {
            int count = 0;
            for(int i = 0; i <= 3; i++)
            {
                SessionList.Add(new Session
                {
                    Id = 111,
                    Name = "Writing an Essay",
                    WorkshopSet = count++,
                    WorkshopSetName = "Essay Skills",
                    Time = "9 - 10am",
                    DateHumanFriendly = "Tomorrow",
                    Location = "CB11.05.401",
                });
                SessionList.Add(new Session
                {
                    Id = 111,
                    Name = "Presenting Skills 101",
                    WorkshopSet = count++,
                    WorkshopSetName = "Reading and Writing Skills",
                    Time = "2 - 4pm",
                    DateHumanFriendly = "25/09/15",
                    Location = "CB11.08.401",
                });
                SessionList.Add(new Session
                {
                    Id = 111,
                    Name = "Writing a literature review",
                    WorkshopSet = count++,
                    WorkshopSetName = "Writing Skills",
                    Time = "9 - 10am",
                    DateHumanFriendly = "06/10/15",
                    Location = "CB06.02.180",
                });
            }
            //Shuffle<Session>(SessionList);
        }        
    }
}