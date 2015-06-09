using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.FunctionalTests.TestModels.Northwind;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.SqlServer.FunctionalTests;

namespace AsyncRepro
{
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
            Console.WriteLine("Await Done");

            for (var index = 0; index < int.MaxValue; index++)
            {
                try
                {
                    Console.WriteLine("Constructing {0}", index);
                    using (var fixture = new NorthwindQuerySqlServerFixture())
                    {
                        using (var context = fixture.CreateContext())
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
                catch (Exception e)
                {
                    Debugger.Launch();
                }
            }

            resetEvent.Set();
        }
    }
}




