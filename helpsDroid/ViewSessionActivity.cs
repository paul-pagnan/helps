using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using com.refractored.fab;
using helps.Droid.Adapters;
using helps.Shared.DataObjects;
using Environment = System.Environment;

namespace helps.Droid
{
    [Activity(Label = "", WindowSoftInputMode = SoftInput.AdjustPan, Theme = "@style/AppTheme.MyToolbar")]
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

        private static SessionDetail session;


        protected override int LayoutResource
        {
            get { return Resource.Layout.Activity_ViewSession; }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var extras = Intent.Extras;
            if (extras != null)
            {
                //Get vars from bundle
                var sessionId = extras.GetInt("Id");
                cts.Cancel();
                //Get the Session Details
                session = Services.Session.GetSession(sessionId);
                SetToolbarColor(ListBaseAdapter.GetColor(session.Type.Length));
                UpdateFields();
                cts.Cancel();
            }
        }

        private void UpdateFields()
        {
            ShowNotifications(session.Id, false);
            title.Text = session.Title;
            editTxtNotes.Text = session.Notes;

            FindViewById<TextView>(Resource.Id.textViewRoomValue).Text = session.Room;
            var TimeString = (session.Time != null) ? Environment.NewLine + session.Time : "";
            FindViewById<TextView>(Resource.Id.textViewDateValue).Text = session.DateHumanFriendly + TimeString;
            FindViewById<TextView>(Resource.Id.textViewPlaceAvailableValue).Text = session.FilledPlaces.ToString();
            if(session.IsGroup <= 0)
                FindViewById<RelativeLayout>(Resource.Id.placesContainer).Visibility = ViewStates.Gone;
            FindViewById<TextView>(Resource.Id.textViewEmail).Text = session.LecturerEmail;
            FindViewById<TextView>(Resource.Id.textViewAppointmentType).Text = session.AppointmentType;
            FindViewById<TextView>(Resource.Id.textViewAssignmentType).Text = session.AssignmentType;
            FindViewById<TextView>(Resource.Id.textViewSubject).Text = session.Subject;
            FindViewById<TextView>(Resource.Id.textViewLecturerComment).Text = session.LecturerComment;
            FindViewById<TextView>(Resource.Id.textViewReason).Text = session.Reason;


            if (session.AppointmentType == null)
                FindViewById<RelativeLayout>(Resource.Id.appointmentType).Visibility = ViewStates.Gone;

            if (session.AssignmentType == null)
                FindViewById<RelativeLayout>(Resource.Id.assignmentType).Visibility = ViewStates.Gone;

            if (session.Subject == null)
                FindViewById<RelativeLayout>(Resource.Id.subject).Visibility = ViewStates.Gone;

            if (session.LecturerComment == null)
                FindViewById<RelativeLayout>(Resource.Id.lecturerComment).Visibility = ViewStates.Gone;

            if (session.Reason == null)
                FindViewById<RelativeLayout>(Resource.Id.reason).Visibility = ViewStates.Gone;
        }

        [Java.Interop.Export()]
        public void Edit(View view)
        {
            EditNotes(session);
        }

        [Java.Interop.Export()]
        public void ShowNotificationDialog(View view)
        {
            ShowNotificationDialog(session.Id, session.Date, false);
        }
    }
}