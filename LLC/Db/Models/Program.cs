using DbUp;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace Db.Models
{
    class Program
    {
        static int Main2(string[] args)
        {
            var connectionString =
                args.FirstOrDefault()
                ?? System.Configuration.ConfigurationManager.
                        ConnectionStrings["DefaultConnection"].ConnectionString;

            EnsureDatabase.For.SqlDatabase(connectionString);

            var builder = new SqlConnectionStringBuilder(connectionString);
            var upgrader =
                DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithVariable("DBName", builder.InitialCatalog)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToConsole()
                    .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();

#if DEBUG

                Console.ReadLine();

#endif

                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();

            return 0;
        }
    }
}