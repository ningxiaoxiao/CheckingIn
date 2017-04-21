using Dos.ORM;

namespace oatoolDBhelper
{
    public class DB
    {

        public static readonly DbSession Context = new DbSession(DatabaseType.Sqlite3, "Data Source=db.db");
    }
}