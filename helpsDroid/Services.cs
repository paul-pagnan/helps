using helps.Shared;

namespace helps.Droid
{
    public static class Services
    {
        public static AuthService Auth;
        public static StudentService Student;
        public static WorkshopService Workshop;

        static Services()
        {
            Auth = new AuthService();
            Student = new StudentService();
            Workshop = new WorkshopService();
        }
    }
}