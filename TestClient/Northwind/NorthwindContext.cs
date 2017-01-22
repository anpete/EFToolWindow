using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

// ReSharper disable ArgumentsStyleStringLiteral
// ReSharper disable ArgumentsStyleAnonymousFunction

namespace TestClient.Northwind
{
    public class NorthwindContext : DbContext
    {
        public NorthwindContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetail>(e => { e.HasKey(od => new { od.OrderId, od.ProductId }); });

            modelBuilder.Entity<Customer>().ToTable("Customers");
            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<Product>().Ignore(p => p.SupplierId);
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<OrderDetail>().ToTable("Order Details");
        }
    }

    public class MyLoggerFactory : ILoggerFactory
    {
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public ILogger CreateLogger(string categoryName)
        {
            throw new System.NotImplementedException();
        }

        public void AddProvider(ILoggerProvider provider)
        {
            throw new System.NotImplementedException();
        }
    }
}