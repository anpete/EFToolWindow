using System;
using System.Linq;
using Microsoft.EFClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TestClient.Northwind;

namespace TestClient
{
    internal class Program
    {
        private static void Main()
        {
            var serviceProvider
                = new ServiceCollection()
                    .AddEntityFrameworkSqlServer()
                    .BuildServiceProvider();

            var loggerFactory
                = serviceProvider.GetRequiredService<ILoggerFactory>();

            loggerFactory.AddProvider(new DiagnosticsLoggerProvider());

            var options
                = new DbContextOptionsBuilder()
                    .UseInternalServiceProvider(serviceProvider)
                    .UseSqlServer(
                        connectionString: "Data Source=(localdb)\\MSSQLLocalDB;Database=Northwind;Integrated Security=True")
                    .Options;

            using (var context = new NorthwindContext(options))
            {
                var customers = context.Customers.ToList();

                foreach (var customer in customers)
                {
                    Console.WriteLine(customer.CompanyName);
                }
            }

            Console.ReadLine();
        }
    }
}