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
using System.Globalization;
using helps.Shared.DataObjects;
using helps.Shared.Consts;
using helps.Droid.Helpers;

namespace helps.Droid
{
    [Activity(Label = "My Information", WindowSoftInputMode = SoftInput.AdjustPan, Theme = "@style/AppTheme.MyToolbar")]
    public class ViewSessionActivity : ViewSessionBase
    {
        private TextView lectureName;
        private TextView lectureEmailAddress;
        private TextView sessionType;
        private TextView appointment;
        private TextView assignment;
        private TextView numberOfPeople;
        private TextView lectureComment;
        private TextView subject;
        private TextView appointmentsOther;
        private TextView assistanceText;
        private TextView date;
        private TextView room;

        protected override int LayoutResource
        {
            get { return Resource.Layout.Activity_ViewSession; }
        }


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            InitSessionComponents();
        }

        private void InitSessionComponents()
        {
            InitComponents();
            lectureName = FindViewById<TextView>(Resource.Id.textViewLectureNameValue);

            lectureEmailAddress = FindViewById<TextView>(Resource.Id.textViewLectureEmailValue);

            sessionType = FindViewById<TextView>(Resource.Id.textViewSessionTypeValue);

            appointment = FindViewById<TextView>(Resource.Id.textViewAppointmentValue);

            assignment = FindViewById<TextView>(Resource.Id.textViewAssignmentValue);

            numberOfPeople = FindViewById<TextView>(Resource.Id.textViewNumberOfPeopleValue);

            lectureComment = FindViewById<TextView>(Resource.Id.textViewLectureCommentValue);

            subject = FindViewById<TextView>(Resource.Id.textViewSubjectValue);

            appointmentsOther = FindViewById<TextView>(Resource.Id.textViewAppointmentsOtherValue);

            assistanceText = FindViewById<TextView>(Resource.Id.textViewAssistanceTextValue);

            date = FindViewById<TextView>(Resource.Id.textViewDateValue);

            room = FindViewById<TextView>(Resource.Id.textViewRoomValue);
        }

     
    }
}