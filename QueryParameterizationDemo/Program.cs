using Dapper;
using System;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Collections;

namespace QueryParameterizationDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Friendly Query using Dynamic Parameters:");
            SetupDatabase();
            Console.WriteLine("Before Query:");
            SelectAllFromDb();
            QueryUsingDynamicParameters("bob");
            Console.WriteLine("After Query:");
            SelectAllFromDb();


            Console.WriteLine("Hostile Query using Dynamic Parameters:");
            SetupDatabase();
            Console.WriteLine("Before Query:");
            SelectAllFromDb();
            QueryUsingDynamicParameters(" '; delete from testTable; select '");
            Console.WriteLine("After Query:");
            SelectAllFromDb();



            Console.WriteLine("Friendly Query using String Templates:");
            SetupDatabase();
            Console.WriteLine("Before Query:");
            SelectAllFromDb();
            QueryUsingStringTemplates("bob");
            Console.WriteLine("After Query:");
            SelectAllFromDb();


            Console.WriteLine("Hostile Query using String Templates:");
            SetupDatabase();
            Console.WriteLine("Before Query:");
            SelectAllFromDb();
            QueryUsingStringTemplates(" '; delete from testTable; select '");
            Console.WriteLine("After Query:");
            SelectAllFromDb();



            Console.ReadLine();
        }

        public static void QueryUsingDynamicParameters(string name)
        {
            using (var conn = new SQLiteConnection("Data Source=MyDatabase.sqlite"))
            {
                var dynamicParameters = new { name = name };
                var q = @"select * from testTable where name = @name;";
                var results = conn.Query<TestTable>(q, dynamicParameters);
                results.ToList().ForEach(r => Console.WriteLine("{0}, {1}", r.id, r.name));
                Console.WriteLine();
            }
        }

        public static void QueryUsingStringTemplates(string name)
        {
            using (var conn = new SQLiteConnection("Data Source=MyDatabase.sqlite"))
            {
                var dynamicParameters = new { name = name };
                var q = $"select * from testTable where name = '{dynamicParameters.name}';";
                var results = conn.Query<TestTable>(q, dynamicParameters);
                results.ToList().ForEach(r => Console.WriteLine("{0}, {1}", r.id, r.name));
                Console.WriteLine();
            }
        }

        public static void SelectAllFromDb()
        {
            using (var conn = new SQLiteConnection("Data Source=MyDatabase.sqlite"))
            {
                var q = @"select * from testTable;";
                var results = conn.Query<TestTable>(q);
                results.ToList().ForEach(r => Console.WriteLine("{0}, {1}", r.id, r.name));
                Console.WriteLine();
            }
        }

        public static void SetupDatabase()
        {
            using (var conn = new SQLiteConnection("Data Source=MyDatabase.sqlite"))
            {
                var tableExistsQuery = @"SELECT name FROM sqlite_master WHERE type='table' AND name='testTable';";
                var tableExists = conn.Query(tableExistsQuery).Any();

                if (tableExists)
                {
                    var dropTableQuery = @"drop table testTable;";
                    conn.Execute(dropTableQuery);
                }

                var createTableQuery = @"
create table testTable
(
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name varchar(255) not null
);

insert into testTable
(name)
values
('adam'),
('bob'),
('cathy'),
('don');
";
                conn.Execute(createTableQuery);
            }
        }

    }

    public class TestTable
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
