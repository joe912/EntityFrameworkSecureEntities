using System.Collections;
using System.Data.Entity;
using System.Linq;

namespace EntityFwkConsoleApp
{
    class Program
    {
        const string _connectionString = "Server=.;Database=Testing123;Trusted_Connection=True;Workstation Id=eftest";

        private static MyDbContext _db = new MyDbContext(_connectionString);

        static void Main(string[] args)
        {
//            using (var db = new MyDbContext(_connectionString))
//            {
//                var widget = new Widget {Name = "widget 1"};
//
//                db.Widgets.Add(widget);
//                db.SaveChanges();
//            }
            var securityProvider = new SecurityProvider(_db);
            var query = securityProvider.JoinSecureEntities<Widget>(_db.Widgets);

            query = query.Where(sw => sw.Entity.Name.Contains("w"));

            var result = query.ToList();
        }
    }

    public class MyDbContext : DbContext
    {
        public MyDbContext(string connection) : base(connection)
        {
            
        }

        public DbSet<Widget> Widgets { get; set; }
    }

    public class Widget
    {
        public int Id { get; set; }

        public string  Name { get; set; }
    }


    public class Secure<T> where T : class
    {
        public T Entity { get; set; }

        public MySecurityContext SecurityContext { get; set; }

    }

    public class MySecurityContext
    {
        public bool HasAccess  { get; set; }
    }

    public class SecurityProvider
    {
        private MyDbContext _db;

        public SecurityProvider(MyDbContext db)
        {
            _db = db;
        }


        public IQueryable<Secure<T>> JoinSecureEntities<T>(IQueryable<T> queryDbSet) where T : class
        {
            return queryDbSet.Select(e => new Secure<T>
            {
                Entity = e,
                SecurityContext = new MySecurityContext {HasAccess = true}
            });
        }
    }
}
