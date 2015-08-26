using SQLite;
namespace helps.Shared.Database
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}
