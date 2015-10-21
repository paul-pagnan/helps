using helps.Shared;
using helps.Shared.Database;
using System.Threading.Tasks;

namespace helps.Droid
{
    public static class Services
    {
        public static AuthService Auth;
        public static StudentService Student;
        public static WorkshopService Workshop;
        public static NotificationService Notification;
        public static SessionService Session;

        static Services()
        {
            helpsDatabase.InitDatabase();
            Auth = new AuthService();
            Student = new StudentService();
            Workshop = new WorkshopService();
            Notification = new NotificationService();
            Session = new SessionService();
            Task.Factory.StartNew(HelpsService.Purge);
        }
    }
}