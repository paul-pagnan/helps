using SQLite;

namespace helps.Shared.DataObjects
{
    public class Campus
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string campus { get; set; }
    }
}
