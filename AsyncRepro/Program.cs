using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.FunctionalTests;
using Microsoft.Data.Entity.FunctionalTests.TestModels.Northwind;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.SqlServer.FunctionalTests;
using Microsoft.Framework.DependencyInjection;

namespace AsyncRepro
{
    class MyContext : NorthwindContext
    {
        public MyContext(IServiceProvider serviceProvider, EntityOptions options)
            : base(serviceProvider, options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>();

            modelBuilder.Entity<Employee>(e =>
            {
                e.Ignore(em => em.Address);
                e.Ignore(em => em.BirthDate);
                e.Ignore(em => em.Extension);
                e.Ignore(em => em.HireDate);
                e.Ignore(em => em.HomePhone);
                e.Ignore(em => em.LastName);
                e.Ignore(em => em.Notes);
                e.Ignore(em => em.Photo);
                e.Ignore(em => em.PhotoPath);
                e.Ignore(em => em.PostalCode);
                e.Ignore(em => em.Region);
                e.Ignore(em => em.TitleOfCourtesy);
            });

            modelBuilder.Entity<Product>(e =>
            {
                e.Ignore(p => p.CategoryID);
                e.Ignore(p => p.QuantityPerUnit);
                e.Ignore(p => p.ReorderLevel);
                e.Ignore(p => p.UnitPrice);
                e.Ignore(p => p.UnitsOnOrder);
            });

            modelBuilder.Entity<Order>(e =>
            {
                e.Ignore(o => o.Freight);
                e.Ignore(o => o.RequiredDate);
                e.Ignore(o => o.ShipAddress);
                e.Ignore(o => o.ShipCity);
                e.Ignore(o => o.ShipCountry);
                e.Ignore(o => o.ShipName);
                e.Ignore(o => o.ShipPostalCode);
                e.Ignore(o => o.ShipRegion);
                e.Ignore(o => o.ShipVia);
                e.Ignore(o => o.ShippedDate);
            });

            modelBuilder.Entity<OrderDetail>(e => { e.Key(od => new { od.OrderID, od.ProductID }); });


            modelBuilder.Entity<Customer>().Table("Customers");
            modelBuilder.Entity<Employee>().Table("Employees");
            modelBuilder.Entity<Product>().Table("Products");
            modelBuilder.Entity<Product>().Ignore(p => p.SupplierID);
            modelBuilder.Entity<Order>().Table("Orders");
            modelBuilder.Entity<OrderDetail>().Table("Order Details");

            modelBuilder.ForSqlServer().UseIdentity();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Main");
            var reset = new ManualResetEventSlim(false);

            var instance = new Program();

            Console.WriteLine("Calling Start");
            var spinTask = Task.Factory.StartNew(async () => { await instance.Spin(reset); });

            Console.WriteLine("Start Waiting on reset event");
            reset.Wait();

            Console.WriteLine("Waiting on task");
            spinTask.Wait();

            Console.Write("Done");
            Console.ReadLine();
        }

        public async Task Spin(ManualResetEventSlim resetEvent)
        {
            Console.WriteLine("Setup");

            var connectionString = new SqlConnectionStringBuilder
            {
                DataSource = @"(localdb)\MSSQLLocalDB",
                //MultipleActiveResultSets = new Random().Next(0, 2) == 1,
                MultipleActiveResultSets = false,
                InitialCatalog = "Northwind",
                IntegratedSecurity = true,
                ConnectTimeout = 30
            }.ConnectionString;

            var serviceProvider = new ServiceCollection()
                .AddEntityFramework()
                .AddSqlServer()
                .ServiceCollection()
                .BuildServiceProvider();

            var optionsBuilder = new EntityOptionsBuilder();
            optionsBuilder.UseSqlServer(connectionString);
            var options = optionsBuilder.Options;



            Console.WriteLine("Await Done");

            try
            {
                //using (var fixture = new NorthwindQuerySqlServerFixture())
                {
                    for (var index = 0; index < int.MaxValue; index++)
                    {
                        Console.WriteLine("Constructing {0}", index);
                        //using (var context = fixture.CreateContext())
                        using (var context = new MyContext(serviceProvider, options))
                        {
                            Console.WriteLine("Await Starting");
                            var result = await context.Set<Customer>()
                                .Where(c => context.Set<Order>().Select(o => o.CustomerID).Contains(c.CustomerID))
                                .ToArrayAsync();

                            //var result = await context.Set<Order>().GroupBy(o => o.CustomerID, (k, g) =>
                            //    new
                            //    {
                            //        Sum = g.Sum(o => o.OrderID),
                            //        MinAsync = g.Min(o => o.OrderID),
                            //        MaxAsync = g.Max(o => o.OrderID),
                            //        Avg = g.Average(o => o.OrderID)
                            //    }).ToArrayAsync();

                            //var result = await context.Set<Customer>()
                            //    .Where(c => context.Set<Order>().AnyAsync(CancellationToken.None).Result)
                            //    .ToArrayAsync();

                            //var result = context.Set<Customer>()
                            //    .Where(c => context.Set<Order>().Any())
                            //    .ToArray();

                            //var result = await context.Set<Customer>().ToArrayAsync();

                            Console.WriteLine("Await Done");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debugger.Launch();
            }

            resetEvent.Set();
        }
    }
}




