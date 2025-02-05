using System.Linq;
using System.Threading.Tasks;
using Actian.EFCore.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace Actian.EFCore
{
    public class SeedingActianTest : SeedingTestBase
    {
        protected override TestStore TestStore
            => ActianTestStore.Create("SeedingTest");

        protected override SeedingContext CreateContextWithEmptyDatabase(string testId)
            => new SeedingActianContext(testId);

        [ConditionalTheory]
        [InlineData(false)]
        [InlineData(true)]
        public override async Task Seeding_does_not_leave_context_contaminated(bool async)
        {
            using var context = CreateContextWithEmptyDatabase(async ? "1A" : "1S");
            await TestStore.CleanAsync(context);
            var _ = async
                ? await context.Database.EnsureCreatedResilientlyAsync()
                : context.Database.EnsureCreatedResiliently();

            Assert.Empty(context.ChangeTracker.Entries());

            var seeds = context.Set<Seed>().OrderBy(e => e.Id).ToList();
            Assert.Equal(2, seeds.Count);
            Assert.Equal(321, seeds[0].Id);
            Assert.Equal("Apple", seeds[0].Species);
            Assert.Equal(322, seeds[1].Id);
            Assert.Equal("Orange", seeds[1].Species);
        }

        protected class SeedingActianContext : SeedingContext
        {
            public SeedingActianContext(string testId)
                : base(testId)
            {
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseActian(TestEnvironment.GetConnectionString($"Seeds{TestId}"));
        }
    }
}
