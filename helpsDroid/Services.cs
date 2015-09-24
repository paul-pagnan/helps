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

        static Services()
        {
            helpsDatabase.InitDatabase();
            Auth = new AuthService();
            Student = new StudentService();
            Workshop = new WorkshopService();
            Task.Factory.StartNew(HelpsService.Purge);
        }
    }
}