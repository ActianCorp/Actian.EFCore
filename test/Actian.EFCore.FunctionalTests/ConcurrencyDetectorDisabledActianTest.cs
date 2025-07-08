using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Actian.EFCore.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace Actian.EFCore
{
    [Collection("Test Collection")]
    public class ConcurrencyDetectorDisabledActianTest : ConcurrencyDetectorDisabledRelationalTestBase<
        ConcurrencyDetectorDisabledActianTest.ConcurrencyDetectorActianFixture>
    {
        public ConcurrencyDetectorDisabledActianTest(ConcurrencyDetectorActianFixture fixture)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
        }

        protected override async Task ConcurrencyDetectorTest(Func<ConcurrencyDetectorDbContext, Task<object>> test)
        {
            await base.ConcurrencyDetectorTest(test);

            Assert.NotEmpty(Fixture.TestSqlLoggerFactory.SqlStatements);
        }

        public override Task Any(bool async)
        {
            _ = ConcurrencyDetectorTest(
            async c => async
                ? await c.Products.AnyAsync(p => p.Id < 10)
                : c.Products.Any(p => p.Id < 10));
            return Task.CompletedTask;
        }

        public override Task Count(bool async)
        {
            _ = ConcurrencyDetectorTest(async c => async ? await c.Products.CountAsync() : c.Products.Count());
            return Task.CompletedTask;
        }

        public override Task Find(bool async)
        {
            _ = ConcurrencyDetectorTest(async c => async ? await c.Products.FindAsync(1) : c.Products.Find(1));
            return Task.CompletedTask;
        }

        public override Task First(bool async)
        {
            _ = ConcurrencyDetectorTest(
            async c => async
                ? await c.Products.OrderBy(p => p.Id).FirstAsync()
                : c.Products.OrderBy(p => p.Id).First());
            return Task.CompletedTask;
        }

        public override Task FromSql(bool async)
        {
            _ = ConcurrencyDetectorTest(
            async c => async
                ? await c.Products.FromSqlRaw(NormalizeDelimitersInRawString("select * from [Products]")).ToListAsync()
                : c.Products.FromSqlRaw(NormalizeDelimitersInRawString("select * from [Products]")).ToList());
            return Task.CompletedTask;
        }

        public override Task Last(bool async)
        {
            _ = ConcurrencyDetectorTest(
            async c => async
                ? await c.Products.OrderBy(p => p.Id).LastAsync()
                : c.Products.OrderBy(p => p.Id).Last());
            return Task.CompletedTask;
        }

        [Theory(Skip = "Fails when ran in block...")]
        [MemberData(nameof(IsAsyncData))]
        public override Task SaveChanges(bool async)
        {
            _ = ConcurrencyDetectorTest(
                async c =>
                {
                    c.Products.Add(new Product { Id = 3, Name = "Unicorn Horseshoe Protection Pack" });
                    return async ? await c.SaveChangesAsync() : c.SaveChanges();
                });

            using var ctx = CreateContext();
            var newProduct = ctx.Products.Find(3);
            Assert.NotNull(newProduct);
            ctx.Products.Remove(newProduct);
            ctx.SaveChangesAsync();
            return Task.CompletedTask;
        }

        public override Task Single(bool async)
        {
            _ = ConcurrencyDetectorTest(
            async c => async
                ? await c.Products.SingleAsync(p => p.Id == 1)
                : c.Products.Single(p => p.Id == 1));
            return Task.CompletedTask;
        }

        public override Task ToList(bool async)
        {
            _ = ConcurrencyDetectorTest(async c => async ? await c.Products.ToListAsync() : c.Products.ToList());
            return Task.CompletedTask;
        }

        [Collection("Test Collection")]
        public class ConcurrencyDetectorActianFixture : ConcurrencyDetectorFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory
                => ActianTestStoreFactory.Instance;

            public TestSqlLoggerFactory TestSqlLoggerFactory
                => (TestSqlLoggerFactory)ListLoggerFactory;

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                => builder.EnableThreadSafetyChecks(enableChecks: false);
        }
    }

}
